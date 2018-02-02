using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace WebRole1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        //Trie to be used in the service
        private static Trie trie;
        private string fullPath = "";

        //Performance counter to keep track of memory usage
        private PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available MBytes");

        //Method to download the wiki titles file from the Azure blob
        [WebMethod]
        public string BlobAccess()
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("wikititlesdata");

            if (container.Exists())
            {

                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;

                        //override the path
                        fullPath = System.IO.Path.GetTempFileName();

                        //download the file
                        blob.DownloadToFile(fullPath, FileMode.Create);
                        
                        return fullPath;

                    }
                }
            }

            return fullPath;
        }

        [WebMethod]
        public string BuildTrie()
        {
            //initate the trie
            trie = new Trie();

            //get the most updated version of the file
            string path = BlobAccess();
           
            //read the file
            using (StreamReader sr = new StreamReader(path))
            {

                var keepRunning = true;
                var currentInsertionNum = 0;
                var lastInserted = "";
                
                //while memory is above 15mb
                while (keepRunning)
                {
                   
                    //check every 10k inserts
                    if (currentInsertionNum % 10000 == 0)
                    {
                        keepRunning = memProcess.NextValue() >= 15;
                    }

                    //current line being read
                    string line = sr.ReadLine();

                    //if the line is not an empty string
                    if (line != "")
                    {
                        trie.InsertString(line);
                        lastInserted = line;
                        currentInsertionNum++;
                    }

                }
                //return the stats
                return "Last Inserted String: " + lastInserted +
                    " " + "| Inserted: " + currentInsertionNum +
                    " | Memory Remaining: " + memProcess.NextValue();
            }


        }

        //Method to search the trie with the user input by calling the Search function in Trie.cs
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> Search(string input)
        { 
            var list = trie.Search(input);
            return list;

        }


    }
}
