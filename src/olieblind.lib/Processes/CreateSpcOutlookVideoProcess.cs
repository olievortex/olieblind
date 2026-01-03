using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;
using olieblind.lib.Services.Speech;
using olieblind.lib.StormPredictionCenter.Interfaces;
using olieblind.lib.StormPredictionCenter.Models;
using olieblind.lib.Video;
using System.Drawing;

namespace olieblind.lib.Processes;

public class CreateSpcOutlookVideoProcess(
    IOlieWebService ows,
    IOlieImageService ois,
    IOlieConfig config,
    ICommonProcess common,
    IOutlookProduct outlookProduct,
    IOutlookProductParsing parsing,
    IOutlookProductScript scripting,
    IOlieSpeechService speechService) : ICreateSpcOutlookVideoProcess
{
    private const string Mp4Extension = ".mp4";
    private const string GifExtension = ".gif";
    private const string WavExtension = ".wav";

    public Point FinalSize { get; init; } = new(1280, 720);
    public Point PosterSize { get; init; } = new(640, 360);
    public int ImageLengthSeconds { get; init; } = 5;

    public async Task Run(string folderRoot, string fontName, string fontPath, string voiceName, int dayNumber, CancellationToken ct)
    {
        var category = $"Day {dayNumber} Convective Outlook";
        var filePrefix = $"Day{dayNumber}Outlook";

        // Font
        ois.AddFont(fontPath);

        // Acquire a product
        var html = await outlookProduct.GetCurrentIndexAsync(dayNumber, ct);
        var product = GetProductFromHtml(html, dayNumber);

        // Generate the script
        var script = scripting.CreateDefaultScript(product, dayNumber);
        var transcript = scripting.CreateDefaultTranscript(product, dayNumber);
        var title = scripting.CreateHeadline(product, dayNumber);

        // Text to speech
        var localWavPath = OlieCommon.CreateLocalTmpPath(WavExtension);
        var timeSpanWav = await common.GenerateSpeechAsync(voiceName, localWavPath, script, ImageLengthSeconds, speechService, ct);

        // Process images
        var (imageList, posterFilename) = await GenerateImagesAsync(html, fontName, dayNumber, ct);
        var posterUri = common.GetUploadUri(product.EffectiveDate, config.BaseVideoUrl, filePrefix, GifExtension);
        var posterFullPath = common.GetUploadFullPath(product.EffectiveDate, folderRoot, filePrefix, GifExtension);

        // Create the story board
        var storyboardList = common.CreateStoryboard(imageList, ImageLengthSeconds, timeSpanWav);

        // Create the video
        var videoFilename = await common.ImagesToMp4Async(storyboardList, localWavPath, config.FfmpegPath, config.FfmpegCodec, ImageLengthSeconds, ct);
        var videoUri = common.GetUploadUri(product.EffectiveDate, config.BaseVideoUrl, filePrefix, Mp4Extension);
        var videoFullPath = common.GetUploadFullPath(product.EffectiveDate, folderRoot, filePrefix, Mp4Extension);

        // Upload final video and poster
        await common.UploadFileAsync(videoFilename, videoFullPath, ct);
        await common.UploadFileAsync(posterFilename, posterFullPath, ct);

        // Add record to the database
        await common.PushVideoToMySqlAsync(videoUri, category, posterUri, title, transcript, posterFullPath, videoFullPath, ct);

        // Cleanup
        OlieCommon.DeleteTempFiles(ows, imageList, localWavPath, videoFilename);
    }

    #region Private Helpers

    private async Task<(List<string>, string)> GenerateImagesAsync(string html, string fontName, int dayNumber, CancellationToken ct)
    {
        var imageNames = parsing.ExtractImageNamesFromHtml(html, dayNumber);
        var result = new List<string>();
        var first = true;
        var posterFile = string.Empty;

        foreach (var imageName in imageNames)
        {
            var source = await outlookProduct.GetCurrentImageAsync(imageName, ct);
            var fullSizeFile = await GetBrandedImage(source, imageName, fontName, FinalSize, ct);
            result.Add(fullSizeFile);

            if (!first) continue;

            posterFile = await GetBrandedImage(source, imageName, fontName, PosterSize, ct);
            first = false;
        }

        return (result, posterFile);
    }

    private async Task<string> GetBrandedImage(byte[] bitmap, string imageName, string fontName, Point finalSize, CancellationToken ct)
    {
        var branding = outlookProduct.GetBrandingByImageName(imageName, fontName, finalSize);
        var bytes = await outlookProduct.AddBrandingToImageAsync(branding, bitmap, finalSize, ct);
        var localFile = OlieCommon.CreateLocalTmpPath(Path.GetExtension(imageName));

        await ows.FileWriteAllBytes(localFile, bytes, ct);

        return localFile;
    }

    private OutlookProductModel GetProductFromHtml(string html, int dayNumber)
    {
        var narrative = parsing.ExtractNarrativeFromHtml(html);
        var product = parsing.ParseNarrative(narrative, dayNumber);

        return product;
    }

    #endregion
}