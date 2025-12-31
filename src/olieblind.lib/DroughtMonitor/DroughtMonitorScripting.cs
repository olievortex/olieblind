using olieblind.lib.DroughtMonitor.Models;
using System.Text;

namespace olieblind.lib.DroughtMonitor;

public class DroughtMonitorScripting : IDroughtMonitorScripting
{
    private const int SentenceLimit = 5;

    public string CreateDefaultScript(DroughtMonitorProductModel model, CancellationToken ct)
    {
        var result = new StringBuilder(8192);
        result.AppendLine(CreateHeadline(model));
        result.AppendLine(model.Intro);
        result.AppendLine(model.Forecast);

        var headings = new List<string>();
        var paragraphs = new List<string>();

        foreach (var region in model.Regions
                     .Where(region =>
                         !region.Key.Equals("Pacific", StringComparison.OrdinalIgnoreCase))
                     .Where(region =>
                         !region.Key.Equals("Caribbean", StringComparison.OrdinalIgnoreCase)))
        {
            headings.Add(region.Key);
            paragraphs.Add(ReplaceAcronyms(region.Value));
        }

        foreach (var (heading, paragraph) in headings.Zip(paragraphs))
        {
            result.AppendLine(CreateHeading(heading));
            result.AppendLine(paragraph);
        }

        return result.ToString();
    }

    public string CreateDefaultTranscript(DroughtMonitorProductModel model)
    {
        var result = new StringBuilder(8192);
        result.AppendLine(CreateHeadline(model));
        result.AppendLine(model.Intro);
        result.AppendLine(model.Forecast);

        foreach (var region in model.Regions)
        {
            if (region.Key.Equals("Pacific", StringComparison.OrdinalIgnoreCase)) continue;
            if (region.Key.Equals("Caribbean", StringComparison.OrdinalIgnoreCase)) continue;

            result.AppendLine(CreateHeading(region.Key));
            result.AppendLine(region.Value);
        }

        return result.ToString();
    }

    public string CreateDefaultTitle(DroughtMonitorProductModel model)
    {
        return $"U.S. Drought Monitor {model.EffectiveDate:M/d}.";
    }

    public static string CreateHeadline(DroughtMonitorProductModel model)
    {
        return
            $"U.S. Drought Monitor discussion for {model.EffectiveDate.DayOfWeek}, {model.EffectiveDate:MMMM} {model.EffectiveDate.Day}.";
    }

    public static string CreateHeading(string region)
    {
        return $"{region} region.";
    }

    public static string ReplaceAcronyms(string text)
    {
        text = text.Replace("NWS", "National Weather Service");
        text = text.Replace("SPI", "Standardized Precipitation Index");
        text = text.Replace("QPF", "Quantitative Precipitation Forecast");
        text = text.Replace("water year to date (WYTD)", "water year to date", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("abnormal dryness (D0)", "abnormal dryness", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("moderate drought (D1)", "moderate drought", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("severe drought (D2)", "severe drought", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("extreme drought (D3)", "extreme drought", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("WYTD", "water year to date");
        text = text.Replace("snow water equivalent (SWE)", "snow water equivalent", StringComparison.OrdinalIgnoreCase);
        text = text.Replace("SWE", "snow water equivalent");
        text = text.Replace("SPEI", "Standardized Precipitation-Evapotranspiration Index");
        text = text.Replace("CoCoRaHS", "coco rahs", StringComparison.OrdinalIgnoreCase);

        return text;
    }
}