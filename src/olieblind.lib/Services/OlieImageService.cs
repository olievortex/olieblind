using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace olieblind.lib.Services;

[ExcludeFromCodeCoverage]
public class OlieImageService : IOlieImageService
{
    private FontCollection FontCollection { get; } = new();
    private readonly IResampler _sampler = KnownResamplers.Lanczos2;

    public void AddFont(string fontPath)
    {
        FontCollection.Add(fontPath);
    }

    public Font GetFont(string name, float size)
    {
        var fontFamily = FontCollection.Get(name);
        var font = fontFamily.CreateFont(size);

        return font;
    }

    public async Task<byte[]> ResizeAndAddText(byte[] bitmap, System.Drawing.Point finalSize, string text, string fontName, float fontSize,
        System.Drawing.Point textPosition, int offset, CancellationToken ct)
    {
        var font = GetFont(fontName, fontSize);
        var blackBrush = new SolidBrush(Color.Black);
        var redBrush = new SolidBrush(Color.Red);
        var offsetPosition = new PointF(textPosition.X + offset, textPosition.Y + offset);

        using var image = Image.Load(bitmap);
        image.Mutate(x => x.Resize(finalSize.X, finalSize.Y, _sampler));
        image.Mutate(x => x.DrawText(text, font, blackBrush, offsetPosition));
        image.Mutate(x => x.DrawText(text, font, redBrush, new PointF(textPosition.X, textPosition.Y)));
        using var ms = new MemoryStream(512000);
        await image.SaveAsGifAsync(ms, ct);
        return ms.ToArray();
    }

    public async Task<byte[]> Resize(byte[] bitmap, System.Drawing.Point finalSize, CancellationToken ct)
    {
        using var image = Image.Load(bitmap);
        image.Mutate(x => x.Resize(finalSize.X, finalSize.Y, _sampler));
        using var ms = new MemoryStream(512000);
        await image.SaveAsGifAsync(ms, ct);
        return ms.ToArray();
    }

    public async Task CreateMp4FromImagesAndAudio(List<string> imagesIn, double imageDuration, string audioIn,
        string mp4Out, string ffmpegPath, string ffmpegCodec, CancellationToken ct)
    {
        var inputFilePath = CreateInputFile();
        var sbStdOut = new StringBuilder();
        var sbErrOut = new StringBuilder();

        using var ff = new Process();
        ff.StartInfo.UseShellExecute = false;
        ff.StartInfo.CreateNoWindow = true;
        ff.StartInfo.FileName = ffmpegPath;
        ff.StartInfo.RedirectStandardError = true;
        ff.ErrorDataReceived += (_, args) => sbErrOut.Append(args.Data);
        ff.StartInfo.RedirectStandardOutput = true;
        ff.OutputDataReceived += (_, args) => sbStdOut.Append(args.Data);
        ff.StartInfo.Arguments =
            $"-f concat -safe 0 -i {ToLinux(inputFilePath)} -i {ToLinux(audioIn)} -c:a aac -c:v {ffmpegCodec} -crf 35 -r 30 -pix_fmt yuv420p {ToLinux(mp4Out)}";
        ff.Start();
        ff.BeginOutputReadLine();
        ff.BeginErrorReadLine();
        await ff.WaitForExitAsync(ct);

        if (ff.ExitCode != 0) throw new ApplicationException($"ffmpeg exit code {ff.ExitCode}: {sbStdOut}\n{sbErrOut}");
        ff.Close();

        File.Delete(inputFilePath);

        return;

        string CreateInputFile()
        {
            var sb = new StringBuilder("ffconcat version 1.0");
            sb.AppendLine();

            foreach (var image in imagesIn)
            {
                sb.AppendLine($"file {ToLinux(image)}");
                sb.AppendLine($"duration {imageDuration}");
                sb.AppendLine();
            }

            var inputFile = ToLinux(Path.GetTempPath() + Guid.NewGuid() + ".txt");
            File.WriteAllText(inputFile, sb.ToString());

            return inputFile;
        }

        string ToLinux(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}
