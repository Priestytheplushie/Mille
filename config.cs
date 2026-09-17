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