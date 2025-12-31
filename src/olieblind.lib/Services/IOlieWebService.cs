using Azure.AI.TextAnalytics;
using Azure.Storage.Blobs;

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
    Task<byte[]> ApiGetBytes(string url, CancellationToken ct);
    Task<string> ApiGetString(string url, CancellationToken ct);
    Task<string> ApiPostJson(string uri, string json, Dictionary<string, string> headers, CancellationToken ct);

    #endregion

    #region Blob

    Task BlobUploadFile(BlobContainerClient client, string fileName, string localFileName, CancellationToken ct);

    #endregion

    #region File

    void CompressGzip(string sourceFile, string destinationFile);
    void DirectoryDelete(string path);
    List<string> DirectoryList(string path);
    void FileDelete(string path);
    void FileMakeDirectory(string path);
    Task<byte[]> FileReadAllBytes(string path, CancellationToken ct);
    Task FileWriteAllBytes(string path, byte[] data, CancellationToken ct);

    #endregion

    #region Purple

    Task<string> PurpleShell(IOlieConfig config, string arguments, CancellationToken ct);

    #endregion
}
