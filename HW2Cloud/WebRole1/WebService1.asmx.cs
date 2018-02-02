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

        private static Trie trie;
        private string fullPath = "";
        private PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available MBytes");

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

                        /*
                        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
                        fullPath = Path.Combine(folderPath, "formatted-wiki-titles.txt");


                        var fileStream = System.IO.File.OpenWrite(fullPath);

                        using (fileStream)
                        {
                            blob.DownloadToStream(fileStream);
                        }
                        */

                        System.Diagnostics.Debug.WriteLine("Supposed file name:" + System.IO.Path.GetTempFileName());

                        fullPath = System.IO.Path.GetTempFileName();

                        System.Diagnostics.Debug.WriteLine("File name override: " + fullPath);
                        blob.DownloadToFile(fullPath, FileMode.Create);
                        System.Diagnostics.Debug.WriteLine("File name override2: " + fullPath);
                        return fullPath;

                    }
                }



                //BuildTrie(fullPath);
            }

            return fullPath;
        }

        [WebMethod]
        public string BuildTrie()
        {
            trie = new Trie();


            /*
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            fullPath = Path.Combine(folderPath, "formatted-wiki-titles.txt");

            */
            string path = BlobAccess();
           
            //System.Diagnostics.Debug.WriteLine("Full Path for build: " + path);
            using (StreamReader sr = new StreamReader(path))
            {

                var keepRunning = true;
                var currentInsertionNum = 0;
                var lastInserted = "";
                

                while (keepRunning)
                {
                    //keepRunning = memProcess.NextValue() > 8000;

                    if (currentInsertionNum % 10000 == 0)
                    {
                        keepRunning = memProcess.NextValue() >= 15;
                    }

                    //System.Diagnostics.Debug.WriteLine("Index: " + currentInsertionNum);

                    //System.Diagnostics.Debug.WriteLine("Memory : " + memProcess.NextValue());


                    string line = sr.ReadLine();

                    /*
                    int endIndex = line.LastIndexOf('"');
                    if (endIndex > 0)
                    {
                        line = line.Substring(0, endIndex);
                    }
                    line = line.Replace("\"", "");
                    line = line.Replace("_", " ");
                    line = line.Replace("0", "");
                    line = line.Trim();
                    */
                    if (line != "")
                    {
                        //System.Diagnostics.Debug.WriteLine(i);
                        //System.Diagnostics.Debug.WriteLine("Inserting:" + '"' + line + '"');
                        //System.Diagnostics.Debug.WriteLine();
                        trie.InsertString(line);
                        lastInserted = line;
                        currentInsertionNum++;
                    }

                }
                return "Last Inserted String: " + lastInserted +
                    " " + "| Inserted: " + currentInsertionNum +
                    " | Memory Remaining: " + memProcess.NextValue();
            }


        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> Search(string input)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var list = trie.Search(input);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;


            /*
            foreach (var str in list) 
            {
                returnString = returnString + str + " | ";
            }
            return returnString;
            */

            System.Diagnostics.Debug.WriteLine("Time: " + elapsedMs + " ms");

            return list;

            /*
            using (StreamReader sr = new StreamReader(streamPath))
            {

                return "Hello World " + sr.ReadLine();
            }
            */

        }


    }
}
