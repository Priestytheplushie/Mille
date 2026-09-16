using System.Text.RegularExpressions;

namespace Mille;

public class Program {
	static void Main(string[] args) {
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
        if (args.Length == 0) {
            Console.WriteLine("Available Arguments:");
            Console.WriteLine("<filename> The file to open");
            return;
        }

        string path = args[0];
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

		List<string> contents = new(content.Split("\n"));

			int line = 0;
			int col = 0;
			int window = 0;
			int true_col = 0;

			while (true) {
				Display(rules, contents, window, line, col);
				ProcessInput(ref contents, ref line, ref col, ref true_col, args[0]);
				if (line < window) window = line;
				else if (line > window + Console.WindowHeight - rules.Margin - 1) window = line - Console.WindowHeight + rules.Margin + 1;
			}
		} catch (UnauthorizedAccessException) {
			Console.Error.WriteLine("Permission denied: unable to access the specified path.");
		}
	}
	static void Display(Rules rules, List<string> contents, int window, int cursorline, int cursorcol) {
		string write = "\x1b[H\x1b[3J";
		int lnlen = (int)Math.Log10((double)contents.Count) + 1;
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
					}
					else {
						fmt.Push(act);
						s1 += act;
					}
				}
				s = s1 + s[cur..];
			}

			s = s.Replace("\t","".PadLeft(rules.TabCount));

			write += "\x1b[7m" + line.ToString().PadRight(lnlen) + "\x1b[27m " + s + "\x1b[0K\n";
		}
		int tabs_before_cursor = contents[cursorline].Remove(cursorcol).Count(c => c == '\t');
		write += "\x1b[J\x1b[0m\x1b[" + (cursorline-window+1).ToString() + ";" + (cursorcol+lnlen+2 + (rules.TabCount-1)*tabs_before_cursor).ToString() + "H";
		Console.Write(write); // only write once to prevent screen tear and visible cursor movement
	}

	static void ProcessInput(ref List<string> contents, ref int line, ref int col, ref int true_col, string expath) {
		ConsoleKeyInfo key = Console.ReadKey(true);
		switch (key.Modifiers) {
			case ConsoleModifiers.None: case ConsoleModifiers.Shift: switch (key.Key) {
				case ConsoleKey.Backspace:
					if (col == 0) {
						string append = contents[line];
						contents.RemoveAt(line);
						line--;
						col = contents[line].Length;
						contents[line] += append;
					}
					else {
						contents[line] = contents[line][..(col-1)] + contents[line][col..];
						col--;
					} break;
				case ConsoleKey.Enter:
					contents.Insert(line+1, contents[line][col..]);
					contents[line] = contents[line].Remove(col);
					line++; col = 0; break;
				case ConsoleKey.PageDown: line = Math.Min(line+40, contents.Count-1); break;
				case ConsoleKey.PageUp: line = Math.Max(line-40, 0); break;
				case ConsoleKey.End: col = contents[line].Length; break;
				case ConsoleKey.Home: col = 0; break;
				case ConsoleKey.LeftArrow: MoveLeft(contents, ref line, ref col); break;
				case ConsoleKey.RightArrow: MoveRight(contents, ref line, ref col); break;
				case ConsoleKey.UpArrow: MoveUp(contents, ref line, ref col); break;
				case ConsoleKey.DownArrow: MoveDown(contents, ref line, ref col); break;
				case ConsoleKey.Delete:
					if (col == contents[line].Length) {
						if (line < contents.Count-1) {
							string append = contents[line+1];
							contents.RemoveAt(line+1);
							contents[line] += append;
						}
					} else contents[line] = contents[line][..col] + contents[line][(col+1)..]; break;
				case ConsoleKey.Escape: Exit(contents, expath); break;
				default:
					contents[line] = contents[line].Insert(col, ""+key.KeyChar);
					col++; break;
			} break;
			case ConsoleModifiers.Control: switch (key.Key) {
				case ConsoleKey.RightArrow:
					while (MoveRight(contents, ref line, ref col) && !(col == contents[line].Length || Char.IsWhiteSpace(contents[line][col])));
					while (MoveRight(contents, ref line, ref col) && (col  == contents[line].Length || Char.IsWhiteSpace(contents[line][col])));
					break;
				case ConsoleKey.LeftArrow:
					while (MoveLeft(contents, ref line, ref col) && (col  == contents[line].Length || Char.IsWhiteSpace(contents[line][col])));
					while (MoveLeft(contents, ref line, ref col) && !(col == contents[line].Length || Char.IsWhiteSpace(contents[line][col])));
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
				default: Console.Write("\a"); break;
			} break;
			default: Console.Write("\a"); break; // TODO: finish keyboard shortcuts
		}
	}

	static bool MoveLeft(List<string> contents, ref int line, ref int col) {
		if (col == 0) {
			if (MoveUp(contents, ref line, ref col)) { col = contents[line].Length; return true; }
			return false;
		} else col--;
		return true;
	}

	static bool MoveRight(List<string> contents, ref int line, ref int col) {
		if (col == contents[line].Length) {
			if (MoveDown(contents, ref line, ref col)) { col = 0; return true; }
			return false;
		} else col++;
		return true;
	}

	static bool MoveUp(List<string> contents, ref int line, ref int col) {
		if (line <= 0) { line = 0; col = 0; return false; }
		else {
			line--;
			col = Math.Min(col, contents[line].Length);
			return true;
		}
	}

	static bool MoveDown(List<string> contents, ref int line, ref int col) {
		if (line == contents.Count - 1) { col = contents[line].Length; return false; }
		else {
			line++;
			col = Math.Min(col, contents[line].Length);
			return true;
		}
	}

	static void Exit(List<string> lines, string fp) {
		File.WriteAllText(fp, string.Join(Environment.NewLine, lines));
		Console.Write("\x1b[2J\x1b[H\x1b[3J");
		Environment.Exit(0);
	}
}

class Rules {
	public List<(Regex, string)> Colors { get; set; }
	public int TabCount { get; set; }
	public int Margin { get; set; }

	public Rules(List<(Regex, string)>? colors = null, int tab_count = 4, int margin = 1) {
		this.Colors = colors is null ? new() : colors;
		this.TabCount = tab_count;
		this.Margin = margin;
	}
}
