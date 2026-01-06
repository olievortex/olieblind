using Amazon.S3;
using Amazon.S3.Model;
using Azure;
using Azure.AI.TextAnalytics;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace olieblind.lib.Services;

[ExcludeFromCodeCoverage]
public class OlieWebService(IHttpClientFactory httpClientFactory) : IOlieWebService
{
    #region Text/Speech

    public async Task<List<string[]>> TextSummarizationExtract(TextAnalyticsClient client, string[] batchedDocuments, int sentenceCount, CancellationToken ct)
    {
        var result = new List<string[]>();
        var options = new ExtractiveSummarizeOptions
        {
            MaxSentenceCount = sentenceCount
        };

        var operation = await client.ExtractiveSummarizeAsync(WaitUntil.Completed, batchedDocuments,
            options: options, cancellationToken: ct);
        if (operation.Status != TextAnalyticsOperationStatus.Succeeded)
            throw new ApplicationException($"Failed to complete extractive summary: {operation.Status}");

        await LoopOverPages(operation.Value);

        return result;

        async Task LoopOverPages(AsyncPageable<ExtractiveSummarizeResultCollection> pages)
        {
            await foreach (var documentsInPage in pages) LoopOverDocuments(documentsInPage);
        }

        void LoopOverDocuments(ExtractiveSummarizeResultCollection documents)
        {
            foreach (var document in documents)
            {
                if (document.HasError)
                    throw new ApplicationException(
                        $"Action error code: {document.Error.ErrorCode}. Message: {document.Error.Message}");

                LoopOverSentences(document.Sentences);
            }
        }

        void LoopOverSentences(IReadOnlyCollection<ExtractiveSummarySentence> sentences)
        {
            result.Add([.. sentences.Select(s => s.Text)]);
        }
    }

    public async Task<List<string[]>> TextSummarizationAbstract(TextAnalyticsClient client, string[] batchedDocuments, int sentenceCount, CancellationToken ct)
    {
        var result = new List<string[]>();
        var options = new AbstractiveSummarizeOptions
        {
            DisplayName = "OlievortexRed",
            SentenceCount = sentenceCount
        };

        if (batchedDocuments.Length == 0) return result;

        var operation =
            await client.AbstractiveSummarizeAsync(WaitUntil.Completed, batchedDocuments, options: options,
                cancellationToken: ct);
        if (operation.Status != TextAnalyticsOperationStatus.Succeeded)
            throw new ApplicationException($"Failed to complete abstractive summary: {operation.Status}");

        await LoopOverPages(operation.Value);

        return result;

        async Task LoopOverPages(AsyncPageable<AbstractiveSummarizeResultCollection> pages)
        {
            await foreach (var documentsInPage in pages) LoopOverDocuments(documentsInPage);
        }

        void LoopOverDocuments(AbstractiveSummarizeResultCollection documents)
        {
            foreach (var document in documents)
            {
                if (document.HasError)
                    throw new ApplicationException(
                        $"Action error code: {document.Error.ErrorCode}. Message: {document.Error.Message}");

                LoopOverSummaries(document.Summaries);
            }
        }

        void LoopOverSummaries(IReadOnlyCollection<AbstractiveSummary> summaries)
        {
            result.Add([.. summaries.Select(s => s.Text)]);
        }
    }

    #endregion

    #region Api

    public async Task ApiDownloadBytes(string path, string url, CancellationToken ct)
    {
        using var hc = httpClientFactory.CreateClient();
        hc.Timeout = TimeSpan.FromSeconds(30);
        var response = await ApiGetResponseMessage(url, hc, 4, ct);

        var body = await response.Content.ReadAsByteArrayAsync(ct);
        await File.WriteAllBytesAsync(path, body, ct);
    }

    public async Task<(HttpStatusCode, EntityTagHeaderValue?, string)> ApiGet(string url, EntityTagHeaderValue? etag, CancellationToken ct)
    {
        using var hc = new HttpClient();
        hc.Timeout = TimeSpan.FromSeconds(30);

        if (etag is not null) hc.DefaultRequestHeaders.IfNoneMatch.Add(etag);

        try
        {
            using var response = await hc.GetAsync(url, ct);

            var body = await response.Content.ReadAsStringAsync(ct);
            var etagResponse = response.Headers.ETag;
            var responseCode = response.StatusCode;

            return (responseCode, etagResponse, body);
        }
        catch (OperationCanceledException)
        {
            return (HttpStatusCode.NotFound, null, string.Empty);
        }
    }

    private async static Task<HttpResponseMessage> ApiGetResponseMessage(string url, HttpClient hc, int maxTries, CancellationToken ct)
    {
        var tries = -2;

        while (tries++ < (maxTries - 1))
        {
            if (tries >= 0) await Task.Delay(TimeSpan.FromSeconds(2 ^ tries), ct);
            var result = await hc.GetAsync(url, ct);

            if (result.StatusCode == System.Net.HttpStatusCode.OK) return result;
        }

        throw new ApplicationException($"Failed to get {url} after {maxTries} tries");
    }

    public async Task<byte[]> ApiGetBytes(string url, CancellationToken ct)
    {
        using var hc = httpClientFactory.CreateClient();
        hc.Timeout = TimeSpan.FromSeconds(30);
        var response = await ApiGetResponseMessage(url, hc, 4, ct);

        var body = await response.Content.ReadAsByteArrayAsync(ct);
        return body;
    }

    public async Task<string> ApiGetString(string url, CancellationToken ct)
    {
        using var hc = httpClientFactory.CreateClient();
        hc.Timeout = TimeSpan.FromSeconds(30);
        var response = await ApiGetResponseMessage(url, hc, 4, ct);

        var body = await response.Content.ReadAsStringAsync(ct);
        return body;
    }

    public async Task<string> ApiPostJson(string uri, string json, Dictionary<string, string> headers, CancellationToken ct)
    {
        var content = new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        foreach (var kvp in headers) content.Headers.Add(kvp.Key, kvp.Value);

        using var hc = httpClientFactory.CreateClient();
        hc.Timeout = TimeSpan.FromMinutes(10);
        var response = await hc.PostAsync(uri, content, ct);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync(ct);

        return result;
    }

    #endregion

    #region Aws

    //public async Task AwsDownloadAsync(string filename, string bucketName, string key, IAmazonS3 client,
    //    CancellationToken ct)
    //{
    //    var response = await client.GetObjectAsync(bucketName, key, ct);
    //    await response.WriteResponseStreamToFileAsync(filename, false, ct);
    //}

    public async Task<List<string>> AwsList(string bucketName, string prefix, IAmazonS3 client, CancellationToken ct)
    {
        var result = new List<string>();
        ListObjectsV2Response response;

        var request = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = prefix
        };

        do
        {
            response = await client.ListObjectsV2Async(request, ct);

            if (response.S3Objects != null)
            {
                result.AddRange(response.S3Objects.Select(s => s.Key));
            }

            // If the response is truncated, set the request ContinuationToken
            // from the NextContinuationToken property of the response.
            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated ?? false);

        return result;
    }

    #endregion

    #region Blob

    public async Task BlobDownloadFile(BlobContainerClient client, string fileName, string localFileName, CancellationToken ct)
    {
        var blobClient = client.GetBlobClient(fileName);
        await blobClient.DownloadToAsync(localFileName, ct);
    }

    public async Task BlobUploadFile(BlobContainerClient client, string fileName, string localFileName, CancellationToken ct)
    {
        var blobClient = client.GetBlobClient(fileName);
        var contentType = "application/octet-stream";
        var extension = Path.GetExtension(fileName);

        if (extension.Equals(".gif", StringComparison.OrdinalIgnoreCase)) contentType = "image/gif";

        if (extension.Equals(".mp4", StringComparison.OrdinalIgnoreCase)) contentType = "video/mp4";

        var headers = new BlobHttpHeaders
        {
            CacheControl = "public, max-age=604800",
            ContentType = contentType
        };

        await blobClient.UploadAsync(localFileName, headers, cancellationToken: ct);
    }

    public async Task BlobUploadText(BlobContainerClient client, string blobName, string text, CancellationToken ct)
    {
        var blobClient = client.GetBlobClient(blobName);
        var contentType = "application/octet-stream";
        var extension = Path.GetExtension(blobName);

        if (extension.Equals(".html", StringComparison.OrdinalIgnoreCase)) contentType = "text/html";
        if (extension.Equals(".gif", StringComparison.OrdinalIgnoreCase)) contentType = "image/gif";
        if (extension.Equals(".mp4", StringComparison.OrdinalIgnoreCase)) contentType = "video/mp4";

        var options = new BlobUploadOptions()
        {
            HttpHeaders = new BlobHttpHeaders
            {
                CacheControl = "public, max-age=604800",
                ContentType = contentType
            }
        };

        var data = new BinaryData(text);
        await blobClient.UploadAsync(data, options, cancellationToken: ct);
    }

    #endregion

    #region File

    public void CompressGzip(string sourceFile, string destinationFile)
    {
        using var originalFileStream = File.Open(sourceFile, FileMode.Open);
        using var compressedFileStream = File.Create(destinationFile);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionLevel.Optimal);
        originalFileStream.CopyTo(compressionStream);
    }

    public void DirectoryDelete(string path)
    {
        Directory.Delete(path);
    }

    public List<string> DirectoryList(string path)
    {
        if (!Directory.Exists(path)) return [];
        var files = Directory.GetFiles(path).ToList();
        return files;
    }

    public void FileDelete(string path)
    {
        File.Delete(path);
    }

    public void FileMakeDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory)) return;

        Directory.CreateDirectory(directory);
    }

    public async Task<byte[]> FileReadAllBytes(string path, CancellationToken ct)
    {
        return await File.ReadAllBytesAsync(path, ct);
    }

    public async Task<string> FileReadAllTextFromGz(string path, CancellationToken ct)
    {
        await using var stream = File.OpenRead(path);
        await using var gzip = new GZipStream(stream, CompressionMode.Decompress);
        using var sr = new StreamReader(gzip);
        var result = await sr.ReadToEndAsync(ct);

        return result;
    }

    public async Task FileWriteAllBytes(string path, byte[] data, CancellationToken ct)
    {
        await File.WriteAllBytesAsync(path, data, ct);
    }

    #endregion

    #region Purple

    public async Task<string> PurpleShell(IOlieConfig config, string arguments, CancellationToken ct)
    {
        const string AppiKey = "APPLICATIONINSIGHTS_CONNECTION_STRING";
        var sbStdOut = new StringBuilder();
        var sbErrOut = new StringBuilder();

        using var p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.FileName = config.PurpleCmdPath;
        p.StartInfo.RedirectStandardError = true;
        if (!p.StartInfo.EnvironmentVariables.ContainsKey(AppiKey))
            p.StartInfo.EnvironmentVariables.Add(AppiKey, config.ApplicationInsightsConnectionString);
        p.ErrorDataReceived += (_, args) => sbErrOut.AppendLine(args.Data);
        p.StartInfo.RedirectStandardOutput = true;
        p.OutputDataReceived += (_, args) => sbStdOut.AppendLine(args.Data);
        p.StartInfo.Arguments = arguments;
        p.Start();
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        await p.WaitForExitAsync(ct);

        if (p.ExitCode != 0) throw new ApplicationException($"purple exit code {p.ExitCode}: {sbStdOut}\n{sbErrOut}");
        p.Close();

        return sbStdOut.ToString();
    }

    #endregion
}
