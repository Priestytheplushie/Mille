using System.Text.RegularExpressions;
using System.Reflection;
using System.Drawing;

namespace Mille;

public class Program {
	static string? Msg = null;
	static string? cutbuf = null;

	static void Main(string[] args) {
		string version = Assembly.GetExecutingAssembly()
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
			.InformationalVersion ?? "1.0.0";
		Rules rules = new(colors: new ([
			(new(@"\b(bool|byte|sbyte|char|decimal|double|float|IntPtr|int|uint|long|ulong|object|short|ushort|string|base|this|var|void)\b"), "\x1b[38;2;30;200;50m"),
			(new(@"\b(alias|as|case|catch|checked|default|do|dynamic|else|finally|for|fixed|foreach|goto"
				+@"|if|is|lock|new|null|return|switch|throw|try|unchecked|while|abstract|async|class|const"
				+@"|delegate|enum|event|explicit|extern|get|implicit|in|internal|interface|namespace"
				+@"|operator|out|override|params|partial|private|protected|public|readonly|ref|sealed|set"
				+@"|sizeof|stackalloc|static|struct|typeof|unsafe|using|value|virtual|volatile|yield|from"
				+@"|where|select|group|info|orderby|join|let|in|on|equals|by|ascending|descending)\b"), "\x1b[38;2;30;180;180m"),
			(new(@"\b(true|false)\b"), "\x1b[38;2;120;255;255m\x1b[49m"),
			(new(@"\b(break|continue)\b"), "\x1b[38;2;255;50;50m\x1b[49m"),
			(new(@"[+\-*<=>?:!~%&|]"), "\x1b[38;2;200;20;30m"),
			(new(@"\b(0|[1-9][0-9._]+|0x[A-Fa-f0-9_]+|0b[01_]+|0[0-7]+)\b"), "\x1b[38;2;35;50;200m\x1b[49m"),
			(new("^.*?(?:(\".*?\").*?)+$"), "\x1b[38;2;160;130;30m\x1b[49m"),
			(new(@"/{2}.*$"), "\x1b[38;2;128;128;128m\x1b[49m"),
			(new(@"\t "), "\x1b[41m\x1b[39m"),
			(new(@"(TODO:?)"), "\x1b[37m\x1b[48;2;30;180;180m"),
			(new(@" +$"), "\x1b[42m\x1b[39m")
		]), tab_count: 3);

		try {
			if (args.Length == 0 || args[0] == "--help" || args[0] == "-h") {
				Console.WriteLine($"Mille v{version} - A C# text editor\n");
				Console.WriteLine("Usage:");
				Console.WriteLine("  mille <filepath> [options]");
				Console.WriteLine("  mille [command]\n");
				Console.WriteLine("Examples:");
				Console.WriteLine("  mille <filepath>                      Open or create a file");
				Console.WriteLine("  mille <filepath> --language:<lang>    Open file with explicit syntax rules\n");
				Console.WriteLine("Configuration:");
				Console.WriteLine("  mille --language                      List all installed language configs");
				Console.WriteLine("  mille --language:<lang>               Inspect active regex rules & colors for a language");
				Console.WriteLine("  mille --config --language:<lang>      Open/edit the YAML config for a language\n");
				Console.WriteLine("Flags:");
				Console.WriteLine("  -v, --version                         Check current Mille version");
				Console.WriteLine("  -h, --help                            Show help and usage options\n");
				Console.WriteLine("Keybinds:");
				Console.WriteLine("  [esc]              Save and exit the current editor");
				Console.WriteLine("  [ctrl-s]           Save current file");
				Console.WriteLine("  [ctrl-q]           Exit without saving");
				Console.WriteLine("  [ctrl-b]           Move cursor to matching bracket");
				Console.WriteLine("  [ctrl-backspace]   Delete entire word");
				Console.WriteLine("  [ctrl-del]         Delete entire word");
				Console.WriteLine("  [ctrl-k]           Cut line");
				Console.WriteLine("  [ctrl-u]           Paste line");
				Console.WriteLine("  [ctrl-f]           Find next instance forward");
				Console.WriteLine("  [alt-f]            Find previous instance backward");
				Console.WriteLine("  [ctrl-w]           Display current location in file");
				return;
			}
			if (args.Length > 0 && (args[0] == "--version" || args[0] == "-v")) {
				Console.WriteLine($"Mille v{version}");
				return;
			}

			bool isConfig = false;
			string? selectedConfig = null;

			for (int i = 0; i < args.Length; i++) {
				if (string.Equals(args[i], "--config", StringComparison.OrdinalIgnoreCase)) {
					isConfig = true;
				}
				else if (args[i].StartsWith("--language:", StringComparison.OrdinalIgnoreCase)) {
					selectedConfig = args[i].Substring("--language:".Length);
				}
				else if (string.Equals(args[i], "--language", StringComparison.OrdinalIgnoreCase)) {
					if (i + 1 < args.Length && !args[i + 1].StartsWith("--")) {
						selectedConfig = args[++i];
					} else {
						return;
					}
				}
			}

			if (!isConfig && selectedConfig != null) {
				Rules loadedRules = Config.LoadRulesForLanguage(selectedConfig, rules);
				Console.WriteLine($"Configuration for '{selectedConfig}':\n");
				foreach (var (regex, color) in loadedRules.Colors) {
					Console.WriteLine($"  Pattern: {regex}");
					Console.WriteLine($"  Color:   {color}████\x1b[0m ({color}Sample Text\x1b[0m)\n");
				}
				return;
			}

			if (isConfig && selectedConfig != null) {
				string configPath = Config.GetLanguageFilePath(selectedConfig);

				if (!File.Exists(configPath)) {
					Config.LoadRulesForLanguage(selectedConfig, rules);
				}

				string yamlContent = File.ReadAllText(configPath);
				
				List<string> configContents = new List<string>(yamlContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));
				int configLine = 0;
				int configCol = 0;
				int configWindow = 0;
				int configTrueCol = 0;

				while (true) {
					Display(rules, configContents, configWindow, configLine, configCol);
					ProcessInput(rules, ref configContents, ref configLine, ref configCol, ref configTrueCol, ref configWindow, configPath);
					
					if (configLine < configWindow) {
						configWindow = configLine;
					}
					else if (configLine > configWindow + Console.WindowHeight - rules.Margin - 1) {
						configWindow = configLine - Console.WindowHeight + rules.Margin + 1;
					}
				}
			}
			if (!isConfig && selectedConfig != null) {
				rules = Config.LoadRulesForLanguage(selectedConfig, rules);
			}

			if (isConfig && selectedConfig == null) {
				Console.WriteLine("Error: Please specify a language to configure.");
				Console.WriteLine("Usage: mille --config --language:<lang>\n");
				return;
			}

			if (!isConfig && string.Equals(selectedConfig, "", StringComparison.OrdinalIgnoreCase)) {
				string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "mille", "languages");
				
				Console.WriteLine("Configured languages:");
				if (Directory.Exists(configDir)) {
					var files = Directory.GetFiles(configDir, "*.yaml")
						.Concat(Directory.GetFiles(configDir, "*.yml"));

					bool foundAny = false;
					foreach (var file in files) {
						string langName = Path.GetFileNameWithoutExtension(file);
						
						if (!string.IsNullOrWhiteSpace(langName)) {
							Console.WriteLine($"  - {langName}");
							foundAny = true;
						}
					}

					if (!foundAny) {
						Console.WriteLine("  (No custom language configs found)");
					}
				} else {
					Console.WriteLine("  (No custom language configs found)");
				}
				return;
			}

			string path = args[0];
			if (selectedConfig == null && !string.IsNullOrEmpty(path)) {
				selectedConfig = Config.DetectLanguageFromPath(path);
			}

			// Load custom YAML rules if available, or keep default fallback
			if (selectedConfig != null) {
				rules = Config.LoadRulesForLanguage(selectedConfig, rules);
			}
			string content;
			if (File.Exists(path)) {
				content = File.ReadAllText(path);
			}
			else {
				if (Directory.Exists(path)) {
					Console.Error.WriteLine("The specified path is a directory, not a file.");
					return;
				}
				content = string.Empty;
			}

			List<string> contents = new(content.Split(Environment.NewLine));
			int line = 0;
			int col = 0;
			int window = 0;
			int true_col = 0;

			while (true) {
				Display(rules, contents, window, line, col);
				ProcessInput(rules, ref contents, ref line, ref col, ref true_col, ref window, args[0]);
				if (line < window) window = line;
				else if (line > window + Console.WindowHeight - rules.Margin - 1) window = line - Console.WindowHeight + rules.Margin + 1;
			}
		}
		catch (UnauthorizedAccessException) {
			Console.Error.WriteLine("Permission denied: unable to access the specified path.");
		}
	}

	static void Display(Rules rules, List<string> contents, int window, int cursorline, int cursorcol) {
		string write = "\x1b[H\x1b[3J";
		int lnlen = (int)Math.Log10((double)(contents.Count+1)) + 1;
		for (int line = window; line < Math.Min(window + Console.WindowHeight - rules.Margin, contents.Count); line++) {
			string s = contents[line];

			List<(int, string?)> actions = new();
			foreach (var (r, c) in rules.Colors) {
				var matches = r.Matches(s);
				foreach (Match match in matches) {
					if (match.Groups.Count > 1) foreach (Group loc in match.Groups.Cast<Group>().Skip(1)) foreach (Capture cap in loc.Captures) {
						actions.Add((cap.Index, c));
						actions.Add((cap.Index + cap.Length, null));
					} else {
						actions.Add((match.Index, c));
						actions.Add((match.Index + match.Length, null));
					}
				}
			}
			actions.Sort((k1, k2) => k1.Item1 < k2.Item1 ? -1 : k1.Item1 > k2.Item1 ? 1 : k1.Item2 is null ? k2.Item2 is null ? 0 : -1 : k2.Item2 is null ? 1 : 0 );

			if (actions.Count != 0) {
				string s1 = "";
				Stack<string> fmt = new(["\x1b[0m"]);
				int cur = 0;
				foreach (var (ind, act) in actions) {
					s1 += s[cur..ind];
					cur = ind;
					if (act == null) {
						fmt.Pop();
						s1 += fmt.Peek();
					} else {
						fmt.Push(act);
						s1 += act;
					}
				}
				s = s1 + s[cur..];
			}

			s = s.Replace("\t","".PadLeft(rules.TabCount));

			write += "\x1b[7m" + (line+1).ToString().PadRight(lnlen) + "\x1b[27m " + s + "\x1b[0K\n";
		}
		if (Program.Msg is not null) {
			write += "\x1b[0m                \x1b[7m " + Program.Msg + " \x1b[0m\x1b[J";
			Program.Msg = null;
		}
		int tabs_before_cursor = contents[cursorline].Remove(cursorcol).Count(c => c == '\t');
		write += "\x1b[J\x1b[0m\x1b[" + (cursorline-window+1).ToString() + ";" + (cursorcol+lnlen+2 + (rules.TabCount-1)*tabs_before_cursor).ToString() + "H";
		Console.CursorVisible = false;
		Console.Write(write); // only write once to prevent screen tear and visible cursor movement
		Console.CursorVisible = true;
	}

	static void ProcessInput(Rules rules, ref List<string> contents, ref int line, ref int col, ref int true_col, ref int window, string expath) {
		ConsoleKeyInfo key = Console.ReadKey(true);
		switch (key.Modifiers) {
			case ConsoleModifiers.None: case ConsoleModifiers.Shift: switch (key.Key) {
				case ConsoleKey.Backspace:
					if (col == 0) {
						if (line == 0) {
							Console.Beep();
						} else {
							string append = contents[line];
							contents.RemoveAt(line);
							line--;
							col = contents[line].Length;
							contents[line] += append;
						}
					}
					else {
						contents[line] = contents[line][..(col-1)] + contents[line][col..];
						col--;
					} true_col = col; break;
				case ConsoleKey.Enter:
					contents.Insert(line+1, contents[line][col..]);
					contents[line] = contents[line].Remove(col);
					line++; col = 0; true_col = 0; break;
				case ConsoleKey.PageDown:
					line = Math.Min(line+Console.WindowHeight, contents.Count-1);
					col = Math.Min(col, contents[line].Length); true_col = col;
					window = Math.Min(window+Console.WindowHeight, contents.Count-Console.WindowHeight);
					break;
				case ConsoleKey.PageUp:
					line = Math.Max(line-Console.WindowHeight, 0);
					col = Math.Min(col, contents[line].Length); true_col = col;
					window = Math.Max(window-Console.WindowHeight, 0);
					break;
				case ConsoleKey.End: col = contents[line].Length; true_col = col; break;
				case ConsoleKey.Home: col = 0; true_col = col; break;
				case ConsoleKey.LeftArrow:
					MoveLeft(contents, ref line, ref col);
					true_col = col;
					break;
				case ConsoleKey.RightArrow:
					MoveRight(contents, ref line, ref col);
					true_col = col;
					break;
				case ConsoleKey.UpArrow: MoveUp(contents, ref line, ref col, true_col); break;
				case ConsoleKey.DownArrow: MoveDown(contents, ref line, ref col, true_col); break;
				case ConsoleKey.Delete:
					if (col == contents[line].Length) {
						if (line < contents.Count-1) {
							string append = contents[line+1];
							contents.RemoveAt(line+1);
							contents[line] += append;
						}
					} else contents[line] = contents[line][..col] + contents[line][(col+1)..];
					true_col = col;
					break;
				case ConsoleKey.Escape: Save(contents, expath); Exit(); break;
				default:
					contents[line] = contents[line].Insert(col, ""+key.KeyChar);
					col++; true_col = col; break;
			} break;
			case ConsoleModifiers.Control: switch (key.Key) {
				case ConsoleKey.RightArrow:
					while (MoveRight(contents, ref line, ref col) && !(col == contents[line].Length || Char.IsLetterOrDigit(contents[line][col])));
					while (MoveRight(contents, ref line, ref col) && (col  == contents[line].Length || Char.IsLetterOrDigit(contents[line][col])));
					true_col = col;
					break;
				case ConsoleKey.LeftArrow:
					while (MoveLeft(contents, ref line, ref col) && (col  == contents[line].Length || Char.IsLetterOrDigit(contents[line][col])));
					while (MoveLeft(contents, ref line, ref col) && !(col == contents[line].Length || Char.IsLetterOrDigit(contents[line][col])));
					true_col = col;
					break;
				case ConsoleKey.UpArrow:
					col = 0;
					while (line != 0 && string.IsNullOrWhiteSpace(contents[line])) line--;
					while (line != 0 && !string.IsNullOrWhiteSpace(contents[line])) line--;
					break;
				case ConsoleKey.DownArrow:
					col = 0;
					while (line != contents.Count-1 && !string.IsNullOrWhiteSpace(contents[line])) line++;
					while (line != contents.Count-1 && string.IsNullOrWhiteSpace(contents[line])) line++;
					break;
				case ConsoleKey.Backspace:
					int start = col;
					while (col > 0 && (col == contents[line].Length || Char.IsLetterOrDigit(contents[line][col]))) col--;
					while (col > 0 && (col == contents[line].Length || !Char.IsLetterOrDigit(contents[line][col]))) col--;
					contents[line] = contents[line][..col] + contents[line][start..];
					break;
				case ConsoleKey.Delete:
					int end = col;
					while (end != contents[line].Length && Char.IsLetterOrDigit(contents[line][end])) end--;
					while (end != contents[line].Length && !Char.IsLetterOrDigit(contents[line][end])) end--;
					contents[line] = contents[line][..col] + contents[line][end..];
					break;
				case ConsoleKey.B: // find matching close-bracket
					int saved_col = col;
					int saved_true_col = true_col;
					int saved_line = line;
					if (col >= contents[line].Length) { Message("Not A Bracket"); goto Err; }
					bool forward = true;
					int ind = Array.IndexOf(rules.Brackets.Item1, contents[line][col]);
					if (ind == -1) {
						ind = Array.IndexOf(rules.Brackets.Item2, contents[line][col]);
						if (ind == -1) { Message("Not A Bracket"); goto Err; }
						forward = false;
					}
					char search = (forward ? rules.Brackets.Item2 : rules.Brackets.Item1)[ind];
					char opp = (forward ? rules.Brackets.Item1 : rules.Brackets.Item2)[ind];
					int depth = 0;
					if (forward) MoveRight(contents, ref line, ref col); else MoveLeft(contents, ref line, ref col);
					do {
						if (col == contents[line].Length) continue;
						if (contents[line][col] == search) depth--;
						if (contents[line][col] == opp) depth++;
					} while (depth != -1 && (forward ? MoveRight(contents, ref line, ref col) : MoveLeft(contents, ref line, ref col)));
					if (col == contents[line].Length || contents[line][col] != search) { Message("Paren Unmatched"); goto Err; }
					break;
					Err:
						line = saved_line;
						true_col = saved_true_col;
						col = saved_col;
						Console.Write("\a");
						break;
				case ConsoleKey.S: Save(contents, expath); break;
				case ConsoleKey.Q: Exit(); break;
				case ConsoleKey.W: Message("Line: " + (line+1) + "/" + contents.Count + ", Column: " + (col+1) + "/" + (contents[line].Length+1)); break;
				case ConsoleKey.K:
					cutbuf = contents[line];
					contents.RemoveAt(line);
					if (line == contents.Count) line--;
					col = Math.Min(true_col, contents[line].Length);
					break;
				case ConsoleKey.U:
					if (cutbuf is null) { Message("Cutbuffer is Empty"); Console.Write("\a"); break; }
					contents.Insert(line++, cutbuf);
					break;
				case ConsoleKey.F:
					if (col == contents[line].Length || !Char.IsLetterOrDigit(contents[line][col])) {
						Message("Not on a Word");
						Console.Write("\a");
						break;
					}
					string token = "" + contents[line][col];
					int i;
					for (i = col-1; i>=0 && Char.IsLetterOrDigit(contents[line][i]); i--) token = contents[line][i] + token;
					for (i = col+1; i<contents[line].Length && Char.IsLetterOrDigit(contents[line][i]); i++) token += contents[line][i];

					if (contents[line][i..].Contains(token)) col = i + contents[line][i..].IndexOf(token);
					else for (int new_line = line == contents.Count-1 ? 0 : line+1; new_line != line; new_line = new_line == contents.Count-1 ? 0 : new_line+1) {
						if (new_line == 0) Message("Search Wrapped");
						if (contents[new_line].Contains(token)) {
							line = new_line;
							col = contents[line].IndexOf(token);
							goto success;
						}
					}
					Message("`" + token + "` Not Found");
					success:
					break;
				default: Message("Unrecognized Shortcut: `Ctrl-" + key.Key + "`"); Console.Write("\a"); break;
			} break;
			case ConsoleModifiers.Alt: switch (key.Key) {
				case ConsoleKey.F:
					if (col == contents[line].Length || !Char.IsLetterOrDigit(contents[line][col])) {
						Message("Not on a Word");
						Console.Write("\a");
						break;
					}
					string token = "" + contents[line][col];
					int i;
					for (i = col+1; i<contents[line].Length && Char.IsLetterOrDigit(contents[line][i]); i++) token += contents[line][i];
					for (i = col-1; i>=0 && Char.IsLetterOrDigit(contents[line][i]); i--) token = contents[line][i] + token;

					if (i > 0 && contents[line][..i].Contains(token)) col = contents[line][..i].IndexOf(token);
					else for (int new_line = line == 0 ? contents.Count-1 : line-1; new_line != line; new_line = new_line == 0 ? contents.Count-1 : new_line-1) {
						if (new_line == contents.Count-1) Message("Search Wrapped");
						if (contents[new_line].Contains(token)) {
							line = new_line;
							col = contents[line].IndexOf(token);
							goto success;
						}
					}
					Message("`" + token + "` Not Found");
					success: break;
				default: Message("Unrecognized Shortcut: `Alt-" + key.Key + "`"); Console.Write("\a"); break;
			} break;
			case ConsoleModifiers.Alt | ConsoleModifiers.Control: switch (key.Key) {
				default: Message("Unrecognized Shortcut: `Ctrl-Alt-" + key.Key + "`"); Console.Write("\a"); break;
			} break;
			case ConsoleModifiers.Control | ConsoleModifiers.Shift: switch (key.Key) {
				default: Message("Unrecognized Shortcut: `Ctrl-Shift-" + key.Key + "`"); Console.Write("\a"); break;
			} break;
			case ConsoleModifiers.Alt | ConsoleModifiers.Shift: switch (key.Key) {
				default: Message("Unrecognized Shortcut: `Alt-Shift-" + key.Key + "`"); Console.Write("\a"); break;
			} break;
			case ConsoleModifiers.Control | ConsoleModifiers.Alt | ConsoleModifiers.Shift: switch (key.Key) {
				default: Message("Unrecognized Shortcut: `Ctrl-Alt-Shift-" + key.Key + "`"); Console.Write("\a"); break;
			} break;
			default: Console.Write("\a"); break; // TODO: finish keyboard shortcuts
		}
	}

	static void Message(string msg) {
		Program.Msg = msg;
	}

	static bool MoveLeft(List<string> contents, ref int line, ref int col) {
		if (col == 0) {
			if (MoveUp(contents, ref line, ref col, int.MaxValue)) return true;
			return false;
		} else col--;
		return true;
	}

	static bool MoveRight(List<string> contents, ref int line, ref int col) {
		if (col == contents[line].Length) {
			if (MoveDown(contents, ref line, ref col, 0)) return true;
			return false;
		} else col++;
		return true;
	}

	static bool MoveUp(List<string> contents, ref int line, ref int col, int true_col) {
		if (line <= 0) { line = 0; col = 0; return false; }
		else {
			line--;
			col = Math.Min(true_col, contents[line].Length);
			return true;
		}
	}

	static bool MoveDown(List<string> contents, ref int line, ref int col, int true_col) {
		if (line == contents.Count - 1) { col = contents[line].Length; return false; }
		else {
			line++;
			col = Math.Min(true_col, contents[line].Length);
			return true;
		}
	}

	static void Save(List<string> lines, string fp) {
		File.WriteAllText(fp, string.Join(Environment.NewLine, lines));
	}

	static void Exit() {
		Console.Write("\x1b[2J\x1b[H\x1b[3J");
		Environment.Exit(0);
	}
}

public class Rules {
	public List<(Regex, string)> Colors { get; set; }
	public int TabCount { get; set; }
	public int Margin { get; set; }
	public (char[], char[]) Brackets { get; set; }

	public Rules(List<(Regex, string)>? colors = null, int tab_count = 4, int margin = 1, char[]? left_bracket = null, char[]? right_bracket = null) {
		this.Colors = colors is null ? new() : colors;
		this.TabCount = tab_count;
		this.Margin = margin;
		this.Brackets = (
			left_bracket is null ? ['[', '(', '{', '<'] : left_bracket,
			right_bracket is null ? [']', ')', '}', '>'] : right_bracket
		);
	}
}
