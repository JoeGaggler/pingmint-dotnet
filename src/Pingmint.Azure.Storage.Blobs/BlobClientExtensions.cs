using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace Pingmint.Azure.Storage.Blobs;

/// <summary>
/// Extension methods for <see cref="BlobClient"/>.
/// </summary>
public static class BlobClientExtensions
{
    /// <summary>
    /// Generates a user delegated SAS URL for the specified <see cref="BlobClient"/>.
    /// </summary>
    /// <param name="blobClient">Blob client.</param>
    /// <param name="expiresOn">SAS expiration.</param>
    /// <param name="permissions">SAS permissions.</param>
    /// <param name="cancellationToken">Cancels the SAS generation request.</param>
    /// <returns>Url</returns>
    public static async Task<Uri> GetUserDelegatedSasUrlAsync(
        this BlobClient blobClient,
        DateTimeOffset expiresOn,
        BlobContainerSasPermissions permissions = BlobContainerSasPermissions.Read,
        CancellationToken cancellationToken = default)
    {
        var blobUri = blobClient.Uri;
        var blobContainerClient = blobClient.GetParentBlobContainerClient();
        var blobServiceClient = blobContainerClient.GetParentBlobServiceClient();
        var keyResult = await blobServiceClient.GetUserDelegationKeyAsync(null, expiresOn, cancellationToken);
        var userDelegationKey = keyResult.Value;

        var sasBuilder = new BlobSasBuilder(permissions, expiresOn)
        {
            BlobContainerName = blobContainerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b", // b for blob
        };

        var sasParameters = sasBuilder.ToSasQueryParameters(
            userDelegationKey,
            blobServiceClient.AccountName
        );

        var uriBuilder = new BlobUriBuilder(blobUri) { Sas = sasParameters };
        var sasUri = uriBuilder.ToUri();
        return sasUri;
    }
}
