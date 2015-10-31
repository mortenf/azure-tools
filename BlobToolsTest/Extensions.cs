using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using BlobTools;

namespace BlobToolsTest
{
    [TestClass]
    public class Extensions
    {
        string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

        [TestMethod]
        public void NewCloudBlobClientOK()
        {
            var client = BlobTools.Extensions.NewCloudBlobClient(connectionString);
            Assert.IsNotNull(client);
        }

        [TestMethod]
        public void GetContainerReferenceWithValidationInvalidName()
        {
            var client = BlobTools.Extensions.NewCloudBlobClient(connectionString);
            try
            {
                BlobTools.Extensions.GetContainerReferenceWithValidation(client, ":");
                Assert.Fail("GetContainerReferenceWithValidation should fail with invalid container name");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        [TestMethod]
        public void GetContainerReferenceWithValidationNonExisting()
        {
            var client = BlobTools.Extensions.NewCloudBlobClient(connectionString);
            try
            {
                BlobTools.Extensions.GetContainerReferenceWithValidation(client, Guid.NewGuid().ToString(), true);
                Assert.Fail("GetContainerReferenceWithValidation should fail with missing container");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!e.Message.StartsWith("Container does not exist"))
                    throw;
            }
        }

        [TestMethod]
        public void GetContainerReferenceWithValidationOK()
        {
            var container = BlobTools.Extensions.NewCloudBlobClient(connectionString).GetContainerReferenceWithValidation("azure-tools", true);
            Assert.IsNotNull(container);
        }

        [TestMethod]
        public void GetNameOK()
        {
            string expected = "test/get-name-ok/" + Guid.NewGuid().ToString();
            string actual = BlobTools.Extensions.NewCloudBlobClient(connectionString)
                .GetContainerReferenceWithValidation("azure-tools", true)
                .GetBlockBlobReference(expected)
                .GetName();
            Assert.AreEqual(expected, actual);
        }

    }
}
