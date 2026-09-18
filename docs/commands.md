# Commands and Flags
Mille contains a minimal amount of flags and commands which you can use. These are listed and explained below

## Opening a File
You can use the following syntax to open any file 

Usage: 

`mille <filepath>` 

This opens the file in the editor and auto detects the language and loads that language's config from it's config file

You can pass the `--language` flag to force the editor to open with a specific configuration file

Usage:

`mille <filepath> --language:<lang>`

An example would be opening any file with the built in C# syntax highlighting rules
`mille <filepath> --language:csharp`

## Configuration
The `--config` flag allows you to edit configuration files for the Mille CLI

Usage: 

`mille --config --language:<lang>`

Passing `--config` with the `--language` flag lets you confgiure a language's Syntax Highlighting. If you pass `--language` and a configuration isn't created, the following Default configuration will be created
```yaml
# Syntax rules for <language>
rules:
	- pattern: '\b(keyword1|keyword2)\b'
	  color: 'BrightCyan'
    - pattern: '#.*$'
      color: 'DarkGray'
```
You can than add your own rules and press `[esc]` to save the changes. The next time you open a editor the rules will be applied automatically. 

## Other Flags
The `-v` flag or `--version` displays the current version of Mille and the Commit Hash 
```bash
Mille v1.0.1+6ac75e03b18b93fb57a6d35a82aefa9d6e9ce2af
```
The `-h` or `--help` flag displays CLI help text and instructions 
```bash 
Mille v1.0.1+6ac75e03b18b93fb57a6d35a82aefa9d6e9ce2af - A C# text editor

Usage:
  mille <filepath> [options]
  mille [command]

Examples:
  mille <filepath>                      Open or create a file
  mille <filepath> --language:<lang>    Open file with explicit syntax rules

Configuration:
  mille --language                      List all installed language configs
  mille --language:<lang>               Inspect active regex rules & colors for a language
  mille --config --language:<lang>      Open/edit the YAML config for a language

Flags:
  -v, --version                         Check current Mille version
  -h, --help                            Show help and usage options

Keybinds:
  [esc]              Save and exit the current editor
  [ctrl-s]           Save current file
  [ctrl-q]           Exit without saving
  [ctrl-b]           Move cursor to matching bracket
  [ctrl-backspace]   Delete entire word
  [ctrl-del]         Delete entire word
  [ctrl-k]           Cut line
  [ctrl-u]           Paste line
  [ctrl-f]           Find next instance forward
  [alt-f]            Find previous instance backward
  [ctrl-w]           Display current location in file
  ```
  Passing the `--language` flag bare will print out all installed language configurations in a list. Passing the `--language` flag with a language without `--config` will display all the rules for that language