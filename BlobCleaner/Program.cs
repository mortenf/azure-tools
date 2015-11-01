using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using BlobTools;

namespace BlobCleaner
{
    class Options
    {
        [Option('a', "account", Required = true,
          HelpText = "Storage account name.")]
        public string AccountName { get; set; }

        [Option('k', "key", Required = true,
          HelpText = "Storage account key.")]
        public string AccountKey { get; set; }

        [Option('c', "container", Required = true,
          HelpText = "Blob Storage container name.")]
        public string ContainerName { get; set; }

        [Option('n', "dryrun", DefaultValue = false,
          HelpText = "Dry run - don't actually delete blobs.")]
        public bool DryRun { get; set; }

        [Option('v', "verbose", DefaultValue = false,
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        //[ParserState]
        //public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
#if DEBUG
            args = new[] {
                "--account", ConfigurationManager.AppSettings["StorageAccountName"],
                "--key", ConfigurationManager.AppSettings["StorageAccountKey"],
                "--container", "blob-cleaner",
                "--verbose",
            };
#endif
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Values are available here
                if (options.Verbose)
                {
                    Console.WriteLine("Storage Account: {0}", options.AccountName);
                    Console.WriteLine("Container: {0}", options.ContainerName);
                }
                BlobCleaner(String.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", options.AccountName, options.AccountKey), options.ContainerName, true, options.DryRun);
                Console.WriteLine("Done.");
            }
        }

        public static void BlobCleaner(string connectionString, string containerName, bool showProgress = false, bool dryRun = false)
        {
            var client = Extensions.NewCloudBlobClient(connectionString);
            var container = Extensions.GetContainerReferenceWithValidation(client, containerName, true);
            var blobs = container.ListBlobs(null, true, BlobListingDetails.All);
            HashSet<string> hash = new HashSet<string>();
            var state = 0;
            var progress = new char[] { '|', '/', '-', '\\' };
            foreach (var b in blobs)
            {
                if (showProgress)
                {
                    Console.Write("\r{0}", progress[state]);
                    state++;
                    if (state > 3)
                        state = 0;
                }
                var blob = container.GetBlockBlobReference(((CloudBlockBlob)b).GetName());
                blob.FetchAttributes();
                var md5 = blob.Properties.ContentMD5;
                if (hash.Contains(md5))
                {
                    if (showProgress)
                        Console.WriteLine("\rDeleting: {0}", blob.GetName());
                    if (!dryRun)
                        blob.Delete();
                }
                else
                    hash.Add(md5);
            }
            if (showProgress)
                Console.WriteLine("\r");
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Environment.Exit(1);
        }
    }
}
