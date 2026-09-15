using System.Text.RegularExpressions;

namespace Mille;

public class Program {

	static void Main(string[] args) {
		Rules rules = new(tab_count: 3);

		if (args.Length > 0) {
			string path = args[0];
			string content = File.ReadAllText(path);
			List<string> contents = new(content.Split("\n"));

			int line = 0;
			int col = 0;
			int window = 0;

			while (true) {
				Display(rules, contents, window, line, col);
				ProcessInput(ref contents, ref line, ref col, args[0]);
				if (line < window) window = line;
				else if (line > window + Console.WindowHeight) window = line - Console.WindowHeight;
			}

		}
		else {
			Console.WriteLine("Avaliable Arguements:");
			Console.WriteLine("<filename> The file to open");
		}
	}

	static void Display(Rules rules, List<string> contents, int window, int cursorline, int cursorcol) {
		string write = "\x1b[H";
		int lnlen = (int)Math.Log10((double)contents.Count) + 1;
		for (int line = window; line < Math.Min(window + Console.WindowHeight, contents.Count); line++) {
			string s = contents[line].Replace("\t","".PadLeft(rules.TabCount));

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
			actions.Sort((k1, k2) => k1.Item1.CompareTo(k2.Item1));

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

			write += "\x1b[7m" + line.ToString().PadRight(lnlen) + "\x1b[27m " + s + "\x1b[0K\n";
		}
		int tabs_before_cursor = contents[cursorline].Remove(cursorcol).Count(c => c == '\t');
		write += "\x1b[J\x1b[0m\x1b[" + (cursorline+1).ToString() + ";" + (cursorcol+lnlen+2 + (rules.TabCount-1)*tabs_before_cursor).ToString() + "H";
		Console.Write(write); // only write once to prevent screen tear and visible cursor movement
	}

	static void ProcessInput(ref List<string> contents, ref int line, ref int col, string expath) {
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
					while (MoveRight(contents, ref line, ref col) && (col  == contents[line].Length || Char.IsWhiteSpace(contents[line][col])));
					while (MoveRight(contents, ref line, ref col) && !(col == contents[line].Length || Char.IsWhiteSpace(contents[line][col])));
					break;
				case ConsoleKey.LeftArrow:
					while (MoveLeft(contents, ref line, ref col) && (col  == contents[line].Length || Char.IsWhiteSpace(contents[line][col])));
					while (MoveLeft(contents, ref line, ref col) && !(col == contents[line].Length || Char.IsWhiteSpace(contents[line][col])));
					break;
				default: Console.Write("\a"); break;
			} break;
			default: Console.Write("\a"); break;
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
		if (line == 0) { col = 0; return false; }
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

	public Rules(List<(Regex, string)>? colors = null, int tab_count = 4) {
		this.Colors = colors is null ? new() : colors;
		this.TabCount = tab_count;
	}
}
