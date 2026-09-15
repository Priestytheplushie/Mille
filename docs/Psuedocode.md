# Psuedocode
>This is basically just python code thats supposed to be similar to the main code in the repo. It may not be 100% accurate and should not be treated as runable
## Reading Files
Opens the passed file (first argument) into a list for use in the rendering loop 
```python
def main(sys.argv)
  # Checks if any arguments are passed 
  if args.length > 0:
    path = args[0] # Get the file path as the first passed argument 
    content = File.ReadAllText(path)
    split = content.Split("\n")
  else: # No arguments passed
      print("Avaliable Arguements:")
      print("<filename> The file to open")
```
## ProcessInput Function
This function processes input
```python
def process_input(contents, line, col):
    key = Console.ReadKey(true)
    match key.Modifiers:
    case ConsoleModifiers.None: case ConsoleModifiers.Shift: switch (key.Key):
      case ConsoleKey.Backspace:
        if (col == 0):
            append = contents[line]
						contents.RemoveAt(line)
						line--
						col = contents[line].Length
						contents[line] += append
        else:
            contents[line].Remove(col-1)
						col--
        break
      case ConsoleKey.Enter:
        contents[++line] = ""; break;
      case ConsoleKey.PageDown:
          line = Math.Min(line+40, contents.Count-1)
          break
      case ConsoleKey.PageUp:
          line = Math.Max(line-40, 0)
          break
      case ConsoleKey.End:
          col = contents[line].Length
          break
      case ConsoleKey.Home:
          col = 0
          break
      case ConsoleKey.LeftArrow:
        if (col == contents[line].Length):
          line = Math.Min(line+1, contents.Count-1)
          col = 0
        else col++
          break
      case ConsoleKey.UpArrow:
          line = Math.Max(line-1, 0)
          break
      case ConsoleKey.DownArrow:
          line = Math.Min(line+1, contents.Count-1)
          break
      case ConsoleKey.Delete:
        if (col == contents[line].Length)
          if (line < contents.Count-1)
              append = contents[line+1];
							contents.RemoveAt(line+1);
							contents[line] += append;
          else contents[line].Remove(col)
            break
      default:
        contents[line].Insert(col, ""+key.KeyChar);
        col++; break;
```

        
