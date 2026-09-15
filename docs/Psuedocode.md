# Psuedocode
>This is basically just python code thats supposed to be similar to the main code in the repo. It may not be 100% accurate and should not be treated as runable
## Reading Files
Opens the passed file (first argument) into a list for use in the rendering loop 
```python
def main(*args)
  # Checks if any arguments are passed 
  if args.length > 0:
    path = args[0] # Get the file path as the first passed argument 
    content = File.ReadAllText()
    split = content.Split("\n")
  else: # No arguments passed
      print("Avaliable Arguements:")
      print("<filename> The file to open")
```
