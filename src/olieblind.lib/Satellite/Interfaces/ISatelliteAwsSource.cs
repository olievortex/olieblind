namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteAwsSource
{
    string GetBucketName(int satellite);

    int GetChannelFromAwsKey(string key);

    string GetPrefix(DateTime effectiveHour);

    DateTime GetScanTime(string filename);
}