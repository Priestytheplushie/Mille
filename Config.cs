using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mille;

/*
Example YML Format
rules:
  - pattern: '\b(class|struct)\b'
    color: 'BrightCyan'
  - pattern: '#[A-Fa-f0-9]{6}'
    color: '#ff00f2'
*/

public class LanguageConfig {
    public List<HighlightRuleConfig> Rules { get; set; } = new();
}

public class HighlightRuleConfig {
    public string Pattern { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public static class Config {
    public static string GetLanguageFilePath(string language) {
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string configDirectory = Path.Combine(appDataFolder, "mille", "languages");

        Directory.CreateDirectory(configDirectory);
        return Path.Combine(configDirectory, $"{language.ToLowerInvariant()}.yaml");
    }

    public static Rules LoadRulesForLanguage(string language, Rules defaultRules) {
        string filePath = GetLanguageFilePath(language);

        if (!File.Exists(filePath)) {
            CreateDefaultConfig(filePath, language);
        }

        string yamlContent = File.ReadAllText(filePath);

        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        LanguageConfig config = deserializer.Deserialize<LanguageConfig>(yamlContent) ?? new LanguageConfig();

        List<(Regex, string)> compiledRules = new List<(Regex, string)>();

        foreach (HighlightRuleConfig rule in config.Rules ?? new List<HighlightRuleConfig>()) {
            if (rule == null || string.IsNullOrWhiteSpace(rule.Pattern)) continue;

            try {
                compiledRules.Add((new Regex(rule.Pattern), Colors.Resolve(rule.Color)));
            }
            catch (ArgumentException) {

            }
        }

        if (compiledRules.Count == 0) {
            return defaultRules;
        }

        return new Rules(colors: compiledRules, tab_count: defaultRules.TabCount, margin: defaultRules.Margin);
    }

    private static void CreateDefaultConfig(string filePath, string language) {
        string defaultTemplate;

    if (language.ToLowerInvariant() == "csharp") {
        defaultTemplate = 
            @"# Syntax rules for " + language + @"
rules:
    - pattern: '\b(bool|byte|sbyte|char|decimal|double|float|IntPtr|int|uint|long|ulong|object|short|ushort|string|base|this|var|void)\b'
        color: '#1EC832'
    - pattern: '\b(alias|as|case|catch|checked|default|do|dynamic|else|finally|for|fixed|foreach|goto|if|is|lock|new|null|return|switch|throw|try|unchecked|while|abstract|async|class|const|delegate|enum|event|explicit|extern|get|implicit|in|internal|interface|namespace|operator|out|override|params|partial|private|protected|public|readonly|ref|sealed|set|sizeof|stackalloc|static|struct|typeof|unsafe|using|value|virtual|volatile|yield|from|where|select|group|info|orderby|join|let|in|on|equals|by|ascending|descending)\b'
        color: '#1EB4B4'
    - pattern: '\b(true|false)\b'
        color: '#78FFFF'
    - pattern: '\b(break|continue)\b'
        color: '#FF3232'
    - pattern: '[+\-*<=>?:!~%&|]'
        color: '#C8141E'
    - pattern: '\b(0|[1-9][0-9._]+|0x[A-Fa-f0-9_]+|0b[01_]+|0[0-7]+)\b'
        color: '#2332C8'
    - pattern: '^.*?(?:("".*?"").*?)+$'
        color: '#A0821E'
    - pattern: '/{2}.*$'
        color: 'DarkGray'
    - pattern: '\t '
        color: 'Red'
    - pattern: '(TODO:?)'
        color: 'BrightCyan'
    - pattern: ' +$'
        color: 'Green'
";
        defaultTemplate = Regex.Replace(defaultTemplate, "(?m)^\\s+rules:", "rules:");
        defaultTemplate = Regex.Replace(defaultTemplate, "(?m)^\\s+- pattern:", "  - pattern:");
        defaultTemplate = Regex.Replace(defaultTemplate, "(?m)^\\s+color:", "    color:");
    }
    else {
        defaultTemplate = 
            @"# Syntax rules for " + language + @"
            rules:
            # - pattern: '\b(keyword1|keyword2)\b'
            #   color: 'BrightCyan'
            # - pattern: '#.*$'
            #   color: 'DarkGray'
            ";
    }
     File.WriteAllText(filePath, defaultTemplate);
}

    public static string DetectLanguageFromPath(string filePath) {
        string extension = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant();

        return extension switch {
            "cs" => "csharp",
            "js" => "javascript",
            "ts" => "typescript",
            "py" => "python",
            "rs" => "rust",
            "go" => "go",
            "html" => "html",
            "css" => "css",
            "json" => "json",
            "yaml" or "yml" => "yaml",
            _ => extension 
        };
    }
}
