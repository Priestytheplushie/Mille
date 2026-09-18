# Data Structures
Mille uses some very simple data structures, such as a `list<>`, `YAML files` and `classes`. Despite being primitive, it allows the entire editor to function

`list<>`

We use a list to store all of the lines of code in the buffer, this allows us to traverse it and provide our syntax highlighting as well as manipulate it. Strings in C# being immutable provided advantages because we don't have to clone everything, which is efficent 

`Classes` 

We used classes to store our methods because they are the C# norm. The `Colors` class was used to provide a human readable way of adding regex colors, besides hex codes. The Program class stores the main program, the Rules class stores a regex rule, and the Config class handles configuration. They are all under the `mille` namespace, meaning we can access them from any file using their names. They are all static classes 

`YAML Files` 
We used YAML files to store our config because they are human readalbe and easier to read and parse. They provide a way to store our regex rules and colors. We than parse them and detect if they are colors from the `Colors` class or a HEX code. These are than applied to the code.