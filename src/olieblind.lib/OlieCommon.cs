using olieblind.lib.Services;
using System.Globalization;

namespace olieblind.lib;

public static class OlieCommon
{
    public static string CreateLocalTmpPath(string extension)
    {
        var tmpPath = Path.GetTempPath();
        var guid = Guid.NewGuid().ToString();

        return $"{tmpPath}{guid}{extension}";
    }

    public static void DeleteTempFiles(IOlieWebService ows, List<string> fileList, params string[] additionalFiles)
    {
        var combined = fileList.Select(s => s).ToList();
        combined.AddRange(additionalFiles);

        foreach (var path in combined) ows.FileDelete(path);
    }

    public static int GetCurrentTimeZoneOffset(DateTime effectiveDate)
    {
        return IsDaylightSavingsTime(effectiveDate) ? -5 : -6;
    }

    public static bool IsDaylightSavingsTime(DateTime effectiveDate)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        return tz.IsDaylightSavingTime(effectiveDate);
    }

    public static DateTime ParseSpcEffectiveDate(string line)
    {
        // 0101 PM CDT Fri Mar 14 2025
        if (string.IsNullOrWhiteSpace(line)) throw new ArgumentNullException(nameof(line));

        var parts = line.Split(' ');
        var hour = int.Parse(parts[0][..2]);
        var minute = int.Parse(parts[0][2..]);
        var year = int.Parse(parts[6]);
        var month = DateTime.ParseExact(parts[4], "MMM", CultureInfo.CurrentCulture).Month;
        var day = int.Parse(parts[5]);
        var tzOffset = TimeZoneToOffset(parts[2]);
        var pmOffset = parts[1] == "PM" ? 12 : 0;

        if (hour == 12 && parts[1] == "AM") hour = 0;
        if (hour == 12 && parts[1] == "PM") pmOffset = 0;

        var result = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
        result = result.AddHours(-tzOffset);
        result = result.AddHours(pmOffset);

        return result;
    }

    public static int TimeZoneToOffset(string tz)
    {
        return tz.ToUpper() switch
        {
            "UTC" => 0,
            "EDT" => -4,
            "EST" => -5,
            "CDT" => -5,
            "CST" => -6,
            "MDT" => -6,
            "MST" => -7,
            "PDT" => -7,
            "PST" => -8,
            _ => throw new ApplicationException($"Unknown time zone: {tz}")
        };
    }

}