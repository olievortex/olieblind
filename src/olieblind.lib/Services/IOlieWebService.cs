using Amazon.S3;
using Azure.AI.TextAnalytics;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using System.Net;
using System.Net.Http.Headers;

namespace olieblind.lib.Services;

public interface IOlieWebService
{
    #region Text/Speech

    Task<List<string[]>> TextSummarizationAbstract(TextAnalyticsClient client, string[] texts, int sentenceCount,
        CancellationToken ct);

    Task<List<string[]>> TextSummarizationExtract(TextAnalyticsClient client, string[] batchedDocuments,
        int sentenceCount, CancellationToken ct);

    #endregion

    #region Api

    Task ApiDownloadBytes(string path, string url, CancellationToken ct);
    Task<(HttpStatusCode, EntityTagHeaderValue?, string)> ApiGet(string url, EntityTagHeaderValue? etag, CancellationToken ct);
    Task<byte[]> ApiGetBytes(string url, CancellationToken ct);
    Task<string> ApiGetString(string url, CancellationToken ct);
    Task<string> ApiPostJson(string uri, string json, Dictionary<string, string> headers, CancellationToken ct);

    #endregion

    #region Aws

    Task AwsDownload(string filename, string bucketName, string key, IAmazonS3 client, CancellationToken ct);

    Task<List<string>> AwsList(string bucketName, string prefix, IAmazonS3 client, CancellationToken ct);

    #endregion

    #region Blob

    Task BlobDownloadFile(BlobContainerClient client, string fileName, string localFileName, CancellationToken ct);

    Task BlobUploadFile(BlobContainerClient client, string fileName, string localFileName, CancellationToken ct);

    Task BlobUploadText(BlobContainerClient client, string blobName, string text, CancellationToken ct);

    #endregion

    #region File

    void CompressGzip(string sourceFile, string destinationFile);
    void DirectoryDelete(string path);
    List<string> DirectoryList(string path);
    void FileDelete(string path);
    void FileMakeDirectory(string path);
    Task<byte[]> FileReadAllBytes(string path, CancellationToken ct);
    Task<string> FileReadAllTextFromGz(string path, CancellationToken ct);
    Task FileWriteAllBytes(string path, byte[] data, CancellationToken ct);

    #endregion

    #region Brown

    Task<string> BrownShell(IOlieConfig config, string arguments, CancellationToken ct);

    #endregion

    #region ServiceBus

    Task ServiceBusSendJson(ServiceBusSender sender, object data, CancellationToken ct);

    #endregion
}
