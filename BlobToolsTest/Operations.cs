using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using BlobTools;

namespace BlobToolsTest
{
    /// <summary>
    /// Unit tests for BlobTools.Operations
    /// </summary>
    [TestClass]
    public class Operations
    {
        string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CreateBlobOK()
        {
            string containerName = "azure-tools";
            string blobName = "create-blob-ok-" + Guid.NewGuid().ToString();
            string content = "123";
            var bloburi = BlobTools.Operations.CreateBlob(
                connectionString,
                containerName,
                blobName,
                ref content);
            Assert.IsTrue(bloburi.EndsWith(blobName));
            var container = BlobTools.Extensions.NewCloudBlobClient(connectionString).GetContainerReferenceWithValidation(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            blob.Delete();
        }
    }
}
