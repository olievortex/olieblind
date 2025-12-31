namespace olieblind.lib.StormPredictionCenter.Models;

public class OutlookProductModel
{
    public string ProductName { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public string Headline { get; set; } = string.Empty;
    public List<string> Headings { get; } = [];
    public List<string> Paragraphs { get; } = [];
}