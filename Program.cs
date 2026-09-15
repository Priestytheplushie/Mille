using System.Text.RegularExpressions;

namespace Mille;

public class Program {
	static void Main(string[] args) {
		if (args.Length > 0) {
			string path = args[0];
			string content = File.ReadAllText(path);
			List<string> contents = new(content.Split("\n"));

			int line = 0;
			int col = 0;
			int window = 0;

			while (true) {
				Display(contents, window, line, col);
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

	static void Display(List<string> contents, int window, int cursorline, int cursorcol, List<(Regex, string)>? color_rules = null) {
		if (color_rules == null) color_rules = new();
		Console.Write("\x1b[H");
		int lnlen = (int)Math.Log10((double)contents.Count) + 1;
		for (int line = window; line < Math.Min(window + Console.WindowHeight, contents.Count); line++) {
			string s = contents[line].Replace("\t","    ");

			List<(int, string?)> actions = new();
			foreach (var (r, c) in color_rules) {
				var matches = r.Matches(s);
				foreach (Match match in matches) foreach (Group loc in match.Groups) foreach (Capture cap in loc.Captures) {
					actions.Add((cap.Index, c));
					actions.Add((cap.Index + cap.Length, null));
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

			Console.WriteLine("\x1b[7m" + line.ToString().PadRight(lnlen) + "\x1b[27m " + s + "\x1b[0K");
		}
		Console.Write("\x1b[J\x1b[0m\x1b[" + (cursorline+1).ToString() + ";" + (cursorcol+lnlen+2).ToString() + "H");
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
				case ConsoleKey.LeftArrow:
					if (col == 0) { line = Math.Max(line-1, 0); col = contents[line].Length; }
					else col--;
					break;
				case ConsoleKey.RightArrow:
					if (col == contents[line].Length) { line = Math.Min(line+1, contents.Count-1); col = 0; }
					else col++;
					break;
				case ConsoleKey.UpArrow:
					line = Math.Max(line-1, 0);
					col = Math.Min(col, contents[line].Length);
					break;
				case ConsoleKey.DownArrow:
					line = Math.Min(line+1, contents.Count-1);
					col = Math.Min(col, contents[line].Length);
					break;
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
			default: Console.Write("\a"); break;
		}
	}

	static void Exit(List<string> lines, string fp) {
		File.WriteAllLines(fp, lines);
		Console.Write("\x1b[2J\x1b[H\x1b[3J");
		Environment.Exit(0);
	}
}
