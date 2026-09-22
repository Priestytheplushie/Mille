# Mille 
A C# text editor using Regular Expressions for syntax highlighting. 

<img width="1074" height="640" alt="Screenshot 2026-09-18 at 9 06 30 AM" src="https://github.com/user-attachments/assets/1d139fcf-b66e-4a38-b9d3-2f1fb7bb9dd0" />

## QuickStart

### Prerequisites
* [.NET SDK](https://dotnet.microsoft.com/download)

### Installation
Install Mille by cloning the repository and building from source:
```bash
# Clone the Repository
git clone https://github.com/Priestytheplushie/Mille.git

# Build from source
dotnet pack -c Release

# Install globally
dotnet tool install --global --add-source ./bin/Release Mille
```
Or check out the [Releases](https://github.com/Priestytheplushie/Mille/releases) tab for precompiled builds for all platforms.
### Usage 
Open a file and auto-detect language:

```bash
mille <filepath>
```
Open a file with explicit language config:
```bash
mille <filepath> --language:<lang>
```
Configure a language's regex rules: 
```bash
mille --config --language<lang>
```

## Documentation 
Code documentation is defined in the [`docs`](https://github.com/Priestytheplushie/Mille/tree/docs) branch.

## License 
This project is licensed under the [GNU General Public License v3.0](LICENSE). For more information, check the `LICENSE` file.
