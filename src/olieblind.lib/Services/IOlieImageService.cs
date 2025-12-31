using System.Drawing;

namespace olieblind.lib.Services;

public interface IOlieImageService
{
    void AddFont(string fontPath);
    Task<byte[]> Resize(byte[] bitmap, Point finalSize, CancellationToken ct);
    Task<byte[]> ResizeAndAddText(byte[] bitmap, Point finalSize, string text, string fontName, float fontSize, Point textPosition, int offset, CancellationToken ct);

    Task CreateMp4FromImagesAndAudio(List<string> imagesIn, double imageDuration, string audioIn,
        string mp4Out, string ffmpegPath, string ffmpegCodec, CancellationToken ct);
}
