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

`mille --config` 

`mille --config --language:<lang>`

Passing the `--config` flag bare allows you to edit general configuration. Passing `--config` with the `--language` flag lets you confgiure a language's Syntax Highlighting. If you pass `--language` and a configuration isn't created, the following Default configuration will be created
```yaml
# Syntax rules for <language>
rules:
	- pattern: '\b(keyword1|keyword2)\b'
	  color: 'BrightCyan'
    - pattern: '#.*$'
      color: 'DarkGray'
```
You can than add your own rules and press `[esc]` to save the changes. The next time you open a editor the rules will be applied automatically. 
