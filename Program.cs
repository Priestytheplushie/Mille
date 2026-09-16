namespace Mille;
public class Program {
	static void Main(string[] args) {
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
            if (Directory.Exists(path))
            {
                Console.Error.WriteLine("The specified path is a directory, not a file.");
                return;
            }

            content = string.Empty;
        }
        OpenFileLoop(content); }
		catch (UnauthorizedAccessException) {
			Console.Error.WriteLine("Permission denied: unable to access the specified path.");
		}
	}
	static void OpenFileLoop(string content) {
		List<string> contents = new(content.Split("\n"));
		int line = 0;
		int col = 0;
		int window = 0;

		while (true) {
			Display(contents, window, line, col);
			ProcessInput(ref contents, ref line, ref col);
			if (line < window) window = line;
			else if (line > window + Console.WindowHeight) window = line - Console.WindowHeight;
		}
	}
	static void Display(List<string> contents, int window, int cursorline, int cursorcol) {
		Console.Write("\x1b[H");
		int lnlen = (int)Math.Log10((double)contents.Count) + 1;
		for (int line = window; line < Math.Min(window + Console.WindowHeight, contents.Count); line++) {
			string s = new(contents[line]);
			Console.WriteLine("\x1b[7m" + line.ToString().PadRight(lnlen) + "\x1b[27m " + s + "\x1b[0K");
		}
		Console.Write("\x1b[0m\x1b[" + (cursorline+1).ToString() + ";" + (cursorcol+lnlen+2).ToString() + "H");
	}

	static void ProcessInput(ref List<string> contents, ref int line, ref int col) {
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
				case ConsoleKey.Enter: contents[++line] = ""; break;
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
				default:
					contents[line] = contents[line].Insert(col, ""+key.KeyChar);
					col++; break;
			} break;
			case ConsoleModifiers.Control: Console.Write("\a"); break; // TODO
			case ConsoleModifiers.Control | ConsoleModifiers.Shift: Console.Write("\a"); break; // TODO
			case ConsoleModifiers.Alt: Console.Write("\a"); break; // TODO
			case ConsoleModifiers.Alt | ConsoleModifiers.Shift: Console.Write("\a"); break; // TODO
			case ConsoleModifiers.Control | ConsoleModifiers.Alt: Console.Write("\a"); break; // TODO
			case ConsoleModifiers.Control | ConsoleModifiers.Alt | ConsoleModifiers.Shift: Console.Write("\a"); break; // TODO
		}
	}
}
