using olieblind.lib.StormPredictionCenter.Models;
using System.Drawing;

namespace olieblind.lib.StormPredictionCenter.Interfaces;

public interface IOutlookProduct
{
    Task<byte[]> AddBrandingToImageAsync(OutlookBrandingModel branding, byte[] bitmap, Point finalSize, CancellationToken ct);
    OutlookBrandingModel GetBrandingByImageName(string imageName, string fontName, Point finalSize);
    Task<byte[]> GetCurrentImageAsync(string imageName, CancellationToken ct);
    Task<string> GetCurrentIndexAsync(int dayNumber, CancellationToken ct);
}
