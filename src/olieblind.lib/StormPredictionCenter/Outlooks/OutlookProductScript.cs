using olieblind.data;
using olieblind.lib.StormPredictionCenter.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace olieblind.lib.StormPredictionCenter.Outlooks;

public partial class OutlookProductScript : IOutlookProductScript
{
    #region Definitions

    private static readonly Dictionary<string, string> Acronyms = new()
    {
        // Weather Terms
        { "AGL", "above ground level" },
        { "AOB", "at or above" },
        { "C", "celsius" },
        { "CAM", "cam" },
        { "CONUS", "Contiguous United States" },
        { "D2", "day 2" },
        { "EL", "equilibrium level" },
        { "ENH", "Enhanced" },
        { "ESRH", "effective storm relative helicity" },
        { "F", string.Empty },
        { "HREF", "h-ref" },
        { "HRRR", "high resolution rapid refresh" },
        { "LLJ", "low level jet" },
        { "MCD", "Mesoscale Discussion" },
        { "MLCAPE", "mixed layer cape" },
        { "MLCIN", "mixed layer sin" },
        { "MPAS", "m-pass" },
        { "MRGL", "marginal" },
        { "MUCAPE", "moo cape" },
        { "NAM", "n a m" },
        { "PW", "precipitable water" },
        { "QLCS", "q l c s" },
        { "RAOB", "ray ob" },
        { "RAP", "rapid refresh" },
        { "SBCAPE", "surface based cape" },
        { "SIG", "significant" },
        { "SRH", "storm relative helicity" },
        { "STP", "significant tornado parameter" },
        { "U.S.", "United States" },
        { "UTC", "universal time" },
        { "VWP", "vertical wind profile" },

        // Weather Forecast Offices
        { "DVN", "Quad Cities" },
        { "FWD", "Fort Worth, Dallas" },
        { "LOT", "Chicago" },
        { "OUN", "Norman, Oklahoma" },

        // Provinces
        { "MB", "Manitoba" },
        { "SK", "Saskatchewan" }
    };

    private static readonly char[] NotALetter =
    [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        ',', '.', '/', '?', '!', '-', '$', '<', '>', '(', ')', '+',
        ' ', '\n', '\r', '\t'
    ];

    #endregion

    public string CreateDefaultScript(OutlookProductModel model, int dayNumber)
    {
        if (model.Headings.Count != model.Paragraphs.Count)
            throw new ArgumentException("Length of headline and paragraphs must be equal");

        var result = new StringBuilder(8192);
        result.AppendLine($"{CreateHeadline(model, dayNumber)} {TransformHeadline(model.Headline)}");

        foreach (var (heading, paragraph) in model.Headings.Zip(model.Paragraphs))
        {
            var header = RephraseHeading(ReplaceAcronyms(heading));

            if (!string.IsNullOrWhiteSpace(header)) result.AppendLine(header);

            result.AppendLine(ReplaceAcronyms(paragraph));
        }

        return result.ToString();
    }

    public string CreateDefaultTranscript(OutlookProductModel model, int dayNumber)
    {
        if (model.Headings.Count != model.Paragraphs.Count)
            throw new ArgumentException("Length of headline and paragraphs must be equal");

        var result = new StringBuilder(8192);
        result.AppendLine($"{CreateHeadline(model, dayNumber)} {model.Headline}");

        foreach (var (heading, paragraph) in model.Headings.Zip(model.Paragraphs))
        {
            result.AppendLine(heading);
            result.AppendLine(paragraph);
        }

        return result.ToString();
    }

    private static string TransformHeadline(string headline)
    {
        headline = headline.Replace(" IN ", " in ");

        return ReplaceAcronyms(headline);
    }

    public string CreateHeadline(OutlookProductModel model, int dayNumber)
    {
        var tzOffset = OlieCommon.GetCurrentTimeZoneOffset(model.EffectiveDate);
        var ed = model.EffectiveDate.AddHours(tzOffset);
        var hour = ed.Hour % 12;
        if (hour == 0) hour = 12;
        var stormDay = model.EffectiveDate.AddDays(dayNumber - 1);

        return $"{hour}{ed:tt} {model.ProductName} for {stormDay.DayOfWeek}, {stormDay:MMMM d}.";
    }

    public static string RephraseHeading(string heading)
    {
        if (heading.Equals("summary", StringComparison.OrdinalIgnoreCase)) return string.Empty;
        if (heading.Equals("discussion", StringComparison.OrdinalIgnoreCase)) return string.Empty;
        if (heading.Equals("synopsis", StringComparison.OrdinalIgnoreCase)) return string.Empty;
        if (heading.Equals("synopsis/discussion", StringComparison.OrdinalIgnoreCase)) return string.Empty;
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (heading.Equals("synopsis and discussion", StringComparison.OrdinalIgnoreCase)) return string.Empty;

        return heading;
    }

    #region ReplaceAcronym

    private static string ReplaceAcronym(string text, string key, string value)
    {
        return ReplaceAcronym(text, new KeyValuePair<string, string>(key, value), StringComparison.OrdinalIgnoreCase);
    }

    private static string ReplaceAcronym(string text, KeyValuePair<string, string> acronym)
    {
        return ReplaceAcronym(text, acronym, StringComparison.Ordinal);
    }

    public static string ReplaceAcronym(string text, KeyValuePair<string, string> acronym, StringComparison mode)
    {
        var start = 0;

        do
        {
            var kLen = acronym.Key.Length;

            var index = text.IndexOf(acronym.Key, start, mode);
            if (index == -1) break;

            var startGood = index == 0;
            if (!startGood && NotALetter.Contains(text[index - 1])) startGood = true;

            var endGood = index + kLen == text.Length;
            if (!endGood && NotALetter.Contains(text[index + kLen])) endGood = true;

            if (startGood && endGood)
                text = text[..index] + acronym.Value + text[(index + acronym.Key.Length)..];

            start = index + 1;
        } while (true);

        return text;
    }

    #endregion

    public static string ReplaceAcronyms(string text)
    {
        // Regions
        text = text.Replace("Gulf of Mexico (GOM)", "Gulf of Mexico");
        text = text.Replace("GOM", "Gulf of Mexico");

        // Units
        text = NegativeCRegex().Replace(text, "negative ");
        text = text.Replace("c/km", "celsius per kilometer", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("j/kg", "jewels per kilogram", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("g/kg", "grams per kilogram", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("m2/s2", string.Empty, StringComparison.OrdinalIgnoreCase);
        text = ReplaceAcronym(text, "kft", "thousand feet");
        text = ReplaceAcronym(text, "km", "kilometer");
        text = ReplaceAcronym(text, "mb", "millibar");
        text = ReplaceAcronym(text, "kt", "knots");
        text = ReplaceAcronym(text, "m", "meters");
        text = ReplaceAcronym(text, "theta-e", "theta e");

        // Slashes
        text = text.Replace("/", ",");

        // Numeric Ranges
        text = NumericRangeRegex().Replace(text, " to ");

        // AI pronunciation
        text = text.Replace("arklatex", "arc law tex", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("bowing", "boweing", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("max.", "maximum.", StringComparison.Ordinal);
        text = text.Replace(" I-", " eye-", StringComparison.Ordinal);
        text = text.Replace("lead shortwave trough", "leed shortwave through", StringComparison.OrdinalIgnoreCase);

        // States
        text = OlieStates.AbbrToFull.Aggregate(text, ReplaceAcronym);
        text = text.Replace("American Samoa", "AS", StringComparison.Ordinal);

        // Acronyms/Abbreviations
        foreach (var acronym in Acronyms)
        {
            text = ReplaceAcronym(text, acronym);
        }

        return text;
    }

    [GeneratedRegex(@"-(?=\d+C)")]
    private static partial Regex NegativeCRegex();

    [GeneratedRegex(@"(?<=\d)-(?=\d+)")]
    private static partial Regex NumericRangeRegex();
}