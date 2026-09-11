namespace Mille {
	public class Program {
		static void Main(string[] args) {
			if (args.Length > 0) {
				string path = args[0];
				string content = File.ReadAllText(path); 
				string[] split = content.Split("\n");
				SortedDictionary<int, string> contents = new SortedDictionary<int, string>();
				for(int i = 0; i < split.Length; i++) {
					contents.Add(i,split[i]);
				}
			}
			else {
				Console.WriteLine("Avaliable Arguements:");
				Console.WriteLine("<filename> The file to open");
			}
		}
	}
}
