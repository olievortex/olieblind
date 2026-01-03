using olieblind.lib.StormPredictionCenter.Interfaces;
using olieblind.lib.StormPredictionCenter.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace olieblind.lib.StormPredictionCenter.Outlooks;

// Partial keyword needed because of ReEx definitions
public partial class OutlookProductParsing : IOutlookProductParsing
{
    public string ExtractNarrativeFromHtml(string html)
    {
        var rx = ExtractNarrativeFromHtmlRegex();
        var match = rx.Match(html);

        if (!match.Success) throw new ApplicationException("Not a SpcProduct");

        return match.Groups[1].Value;
    }

    public List<string> ExtractImageNamesFromHtml(string html, int dayNumber)
    {
        var rx = dayNumber == 3 ? ExtractImageFromDay3HtmlRegex() : ExtractImageFromHtmlRegex();
        var matches = rx.Matches(html);

        if (matches.Count == 0) throw new ApplicationException("Not a SpcProduct");

        var result = new List<string>();
        foreach (Match match in matches)
        {
            var imageName = $"day{dayNumber}{match.Groups[1].Value}.gif";
            result.Add(imageName);
        }

        return result;
    }

    public static OutlookLineTypeEnum CategorizeLine(string text, int dayNumber)
    {
        var effDate = MatchEffectiveDateRegex();
        var valPeriod = MatchValidPeriodRegex();
        if (string.IsNullOrWhiteSpace(text)) return OutlookLineTypeEnum.BlankLine;
        text = text.Trim();

        if (text.Contains("SPC AC", StringComparison.OrdinalIgnoreCase))
            return OutlookLineTypeEnum.MessageStructure;
        if (text.Contains($"Day {dayNumber} Convective Outlook", StringComparison.OrdinalIgnoreCase))
            return OutlookLineTypeEnum.ProductName;
        if (text.Contains("NWS Storm Prediction Center Norman OK", StringComparison.OrdinalIgnoreCase))
            return OutlookLineTypeEnum.MessageStructure;
        if (valPeriod.Match(text).Success) return OutlookLineTypeEnum.MessageStructure;
        if (text.Contains("<a href", StringComparison.OrdinalIgnoreCase))
            return OutlookLineTypeEnum.MessageStructure;
        if (text.Contains("<script ", StringComparison.OrdinalIgnoreCase))
            return OutlookLineTypeEnum.MessageStructure;
        if (text.Contains("note: ", StringComparison.OrdinalIgnoreCase))
            return OutlookLineTypeEnum.MessageStructure;

        // Order of these statements is critical
        if (text.StartsWith("...") && text.EndsWith("...")) return OutlookLineTypeEnum.Header;
        if (text.StartsWith("...")) return OutlookLineTypeEnum.HeaderStart;
        if (text.EndsWith("...")) return OutlookLineTypeEnum.HeaderEnd;
        if (text.StartsWith('.')) return OutlookLineTypeEnum.MessageStructure;
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (effDate.Match(text).Success) return OutlookLineTypeEnum.EffectiveDate;

        return OutlookLineTypeEnum.Sentence;
    }

    public static string ParseHeader(string value)
    {
        value = value.Replace("...", " ");
        var parts = value.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        value = string.Join(' ', parts);
        parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        value = string.Join(' ', parts);

        return value;
    }

    public OutlookProductModel ParseNarrative(string narrative, int dayNumber)
    {
        var result = new OutlookProductModel();
        var lines = narrative.Split('\n', StringSplitOptions.TrimEntries);
        var hasHeadline = false;
        var header = new StringBuilder();
        var paragraph = new StringBuilder();
        var isBuildingHeader = false;

        foreach (var line in lines)
        {
            var category = CategorizeLine(line, dayNumber);

            switch (category)
            {
                case OutlookLineTypeEnum.BlankLine:
                    if (paragraph.Length > 0)
                    {
                        result.Headings.Add(ParseHeader(header.ToString()));
                        result.Paragraphs.Add(ParseHeader(paragraph.ToString()));
                        header.Clear();
                        paragraph.Clear();
                    }

                    break;
                case OutlookLineTypeEnum.ProductName:
                    result.ProductName = line;
                    break;
                case OutlookLineTypeEnum.EffectiveDate:
                    result.EffectiveDate = OlieCommon.ParseSpcEffectiveDate(line);
                    break;
                case OutlookLineTypeEnum.Header when hasHeadline:
                    header.Clear();
                    header.Append(line);
                    break;
                case OutlookLineTypeEnum.Header:
                    hasHeadline = true;
                    result.Headline = ParseHeader(line);
                    break;
                case OutlookLineTypeEnum.HeaderEnd:
                    {
                        isBuildingHeader = false;
                        header.Append($" {line}");

                        if (!hasHeadline)
                        {
                            hasHeadline = true;
                            result.Headline = ParseHeader(header.ToString());
                            header.Clear();
                        }

                        break;
                    }
                case OutlookLineTypeEnum.HeaderStart:
                    isBuildingHeader = true;
                    header.Clear();
                    header.Append(line);
                    break;
                default:
                case OutlookLineTypeEnum.Sentence:
                    if (isBuildingHeader)
                        header.Append($" {line}");
                    else
                        paragraph.Append($" {line}");

                    break;
                case OutlookLineTypeEnum.MessageStructure:
                    break;
            }
        }

        return result;
    }

    #region Regex

    [GeneratedRegex(@"<pre>([\s\S]+?)<\/pre>")]
    private static partial Regex ExtractNarrativeFromHtmlRegex();

    [GeneratedRegex("""<td OnClick="show_tab\('([\s\S]+?)'\)" OnMouseOver="[\s\S]+?">""")]
    private static partial Regex ExtractImageFromHtmlRegex();

    [GeneratedRegex("""<a OnClick="show_tab\('([\s\S]+?)'\)" OnMouseOver="[\s\S]+?">""")]
    private static partial Regex ExtractImageFromDay3HtmlRegex();

    [GeneratedRegex(@"\d{4}\s[a|p]M\sC[s|d]T\s\S{3}\s\S{3}", RegexOptions.IgnoreCase)]
    private static partial Regex MatchEffectiveDateRegex();

    [GeneratedRegex(@"valid\s\d{6}z\s-\s\d{6}z", RegexOptions.IgnoreCase)]
    private static partial Regex MatchValidPeriodRegex();

    #endregion
}