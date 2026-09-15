namespace Mille;
public class Program {
	static void Main(string[] args) {
		if (args.Length > 0) {
			string path = args[0];
			string content = File.ReadAllText(path);
			List<string> contents = new(content.Split("\n"));

			int line = 0;
			int col = 0;

			while (true) {
				Display(contents);
				ProcessInput(ref contents, ref line, ref col);
			}

		}
		else {
			Console.WriteLine("Avaliable Arguements:");
			Console.WriteLine("<filename> The file to open");
		}
	}

	static void Display(List<string> contents) {
		// TODO
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
						contents[line].Remove(col-1);
						col--;
					} break;
				case ConsoleKey.Enter:
					contents[++line] = ""; break;
				case ConsoleKey.PageDown: line = Math.Min(line+40, contents.Count-1); break;
				case ConsoleKey.PageUp: line = Math.Max(line-40, 0); break;
				case ConsoleKey.End: col = contents[line].Length; break;
				case ConsoleKey.Home: col = 0; break;
				case ConsoleKey.LeftArrow:
					if (col == 0) { line = Math.Max(line-1, 0); col = contents[line].Length; }
					else col--; break;
				case ConsoleKey.RightArrow:
					if (col == contents[line].Length) { line = Math.Min(line+1, contents.Count-1); col = 0; }
					else col++; break;
				case ConsoleKey.UpArrow: line = Math.Max(line-1, 0); break;
				case ConsoleKey.DownArrow: line = Math.Min(line+1, contents.Count-1); break;
				case ConsoleKey.Delete:
					if (col == contents[line].Length) {
						if (line < contents.Count-1) {
							string append = contents[line+1];
							contents.RemoveAt(line+1);
							contents[line] += append;
						}
					} else contents[line].Remove(col); break;
				default:
					contents[line].Insert(col, ""+key.KeyChar);
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
