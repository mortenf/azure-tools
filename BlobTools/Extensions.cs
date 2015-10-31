using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text.RegularExpressions;

namespace BlobTools
{
    public static class Extensions
    {
        public static CloudBlobClient NewCloudBlobClient(string connectionString)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            client.DefaultRequestOptions.RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.ExponentialRetry(TimeSpan.FromSeconds(5), 5);
            return client;
        }

        public static CloudBlobContainer GetContainerReferenceWithValidation(this CloudBlobClient client, string containerName, bool validateExisting = false)
        {
            if (!Regex.Match(containerName, @"^(([a-z\d]((-(?=[a-z\d]))|([a-z\d])){2,62})|(\$root))$").Success)
                throw new ArgumentOutOfRangeException("containerName", string.Format("Invalid blob container name ({0})", containerName));
            if (validateExisting)
            {
                try
                {
                    var container = client.GetContainerReference(containerName);
                    if (!container.Exists())
                        throw new ArgumentOutOfRangeException("containerName", containerName, "Container does not exist");
                    return container;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw;
                }
                catch
                {
                    throw new ArgumentOutOfRangeException("containerName", containerName, "Invalid or nonexisting container");
                }
            }
            return client.GetContainerReference(containerName);
        }

        public static string GetName(this CloudBlockBlob blob)
        {
            var segments = blob.Uri.Segments;
            while (segments.Length > 0 && segments[0] != blob.Container.Name + "/")
                segments = segments.Skip(1).ToArray();
            return String.Join("", segments.Skip(1));
        }

    }
}
