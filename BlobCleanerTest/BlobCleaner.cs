using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using BlobTools;
using BlobCleaner;

namespace BlobCleanerTest
{
    [TestClass]
    public class BlobCleanerTest
    {
        string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

        [TestMethod]
        public void BlobCleanerOK()
        {
            var containerName = "azure-tools";
            var container = Extensions.NewCloudBlobClient(connectionString).GetContainerReferenceWithValidation(containerName);
            var name1 = "1.txt";
            var name2 = "2.txt";
            var name3 = "3.txt";
            var contentA = "123";
            var contentB = "456";
            BlobTools.Operations.CreateBlob(connectionString, containerName, name1, ref contentA);
            BlobTools.Operations.CreateBlob(connectionString, containerName, name2, ref contentB);
            BlobTools.Operations.CreateBlob(connectionString, containerName, name3, ref contentA);
            BlobCleaner.Program.BlobCleaner(connectionString, containerName);
            var blob1 = container.GetBlockBlobReference(name1);
            Assert.IsTrue(blob1.Exists());
            blob1.Delete();
            var blob2 = container.GetBlockBlobReference(name2);
            Assert.IsTrue(blob2.Exists());
            blob2.Delete();
            var blob3 = container.GetBlockBlobReference(name3);
            Assert.IsFalse(blob3.Exists());
        }

        [TestMethod]
        public void BlobCleanerDryRunOK()
        {
            var containerName = "azure-tools";
            var container = Extensions.NewCloudBlobClient(connectionString).GetContainerReferenceWithValidation(containerName);
            var name1 = "1.txt";
            var name2 = "2.txt";
            var name3 = "3.txt";
            var contentA = "123";
            var contentB = "456";
            BlobTools.Operations.CreateBlob(connectionString, containerName, name1, ref contentA);
            BlobTools.Operations.CreateBlob(connectionString, containerName, name2, ref contentB);
            BlobTools.Operations.CreateBlob(connectionString, containerName, name3, ref contentA);
            BlobCleaner.Program.BlobCleaner(connectionString, containerName, false, true);
            var blob1 = container.GetBlockBlobReference(name1);
            Assert.IsTrue(blob1.Exists());
            blob1.Delete();
            var blob2 = container.GetBlockBlobReference(name2);
            Assert.IsTrue(blob2.Exists());
            blob2.Delete();
            var blob3 = container.GetBlockBlobReference(name3);
            Assert.IsTrue(blob3.Exists());
            blob3.Delete();
        }
    }
}
