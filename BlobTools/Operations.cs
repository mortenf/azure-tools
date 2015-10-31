using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace BlobTools
{
    public static class Operations
    {
        public static string CreateBlob(string connectionString, string containerName, string blobName, ref string content, string contentType = null, string contentEncoding = null)
        {
            var container = BlobTools.Extensions.NewCloudBlobClient(connectionString).GetContainerReferenceWithValidation(containerName);
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            var blob = container.GetBlockBlobReference(blobName);
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            blob.UploadFromStream(stream);
            if (contentType != null)
                blob.Properties.ContentType = contentType;
            if (contentEncoding != null)
                blob.Properties.ContentEncoding = contentEncoding;
            blob.SetProperties();
            return blob.Uri.ToString();
        }
    }
}
