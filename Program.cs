using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;



namespace FileDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await FFDLMAIN();
        }
        //Main Method where information is being retreived
        static async Task FFDLMAIN()
        {
            //Provides Link Information
            Console.WriteLine("Paste the Download Link here");
            Console.Write(">");
            string Link = Console.ReadLine() ?? "";
            Console.Clear();

            //Provides fileName Information
            Console.WriteLine("What would you like to name the file?");
            Console.Write(">");
            string fileName = Console.ReadLine() ?? "";
            Console.Clear();

            //Retreives the file type and splits it after the / upon retreival
            string filetype = Link_Request.File_Type(Link);
            string[] parts = filetype.Split("/");
            string Fcontenttype = ("." + parts[1]);

            //Prompts user with the file information obtained for validation
            Console.WriteLine("Is this the correct file type? || " + fileName + Fcontenttype + "\n Press 1 for YES\n Press 2 for NO");
            //Checks for Key input when 1 is pressed it sends the information to be finalized and downloaded
            //Checks for Key input when 2 in pressed it sends it to SelfType where the user will input the correct file type
            ConsoleKey key;
            do
            {

                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                        Console.Clear();
                        SelfFinalize(fileName, Fcontenttype, Link);
                        break;
                    case ConsoleKey.NumPad1:
                        Console.Clear();
                        SelfFinalize(fileName, Fcontenttype, Link);
                        break;
                    case ConsoleKey.D2:
                        Console.Clear();
                        SelfType(fileName, Link);
                        break;
                    case ConsoleKey.NumPad2:
                        Console.Clear();
                        SelfType(fileName, Link);
                        break;
                }
            }
            //While 1 or 2 are not being pressed nothing will happen
            while (key != ConsoleKey.D1 && key != ConsoleKey.NumPad1 && key != ConsoleKey.NumPad2 && key != ConsoleKey.D2);
        }


        //Method used for correcting file information if file type retreived via link is incorrect
        private static void SelfType(string fileName, string Link)
        {
            //prompts user to input the correct file type
            Console.WriteLine("Input the correct file type");
            Console.Write(">");
            string NewPreType = Console.ReadLine();

            //adds a '.' before the file type before passing it on to finalize
            string Fcontenttype = "." + NewPreType;
            Console.Clear();
            SelfFinalize(fileName, Fcontenttype, Link);


        }
        //Taks information provided and chunks the file into quarters and downloads them at the same time
        static void SelfFinalize(string fileName, string Fcontenttype, string Link)
        {

            long size = Link_Request.File_Size(Link);
            
            ConcurrentBag<DownloadChunk> chunks = GetChunks(4 , size);

            //https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq
            //Parallel operations using LINQ
            var processedChunks = chunks.AsParallel().Select(x => Download(x, Link)).ToList();

            // DAMIEN :Again when you are using a disposable object you need to use a using statement so it gets rid of it properly. Another
            //important thing about the using statement is that when it exists the function or if there is an exception it will
            //automatically close the stream, or dispose of the object.
            using (FileStream fs = new("C:\\Users\\DamienOstler\\Downloads\\" + fileName + Fcontenttype, FileMode.Create))
            {
                var offset = 0;
                //Sort by starting so that the one starting at 0 goes first and so on
                foreach (DownloadChunk chunk in processedChunks.OrderBy(x=>x.Start))
                {
                    //You should be using the size element since u already have it here.
                    fs.Write(chunk.Data, 0, (int)chunk.Size);
                    offset += chunk.Data.Length;
                }
            }
            

        }

        //NEW METHOD FOR DOWNLOADING THE CHUNKS
        private static DownloadChunk Download(DownloadChunk chunk,string Link)
        {
            HttpWebRequest request = WebRequest.CreateHttp(Link);
            request.AddRange(chunk.Start, chunk.End);
            // DAMIEN :When you are using a class that implements a idisposable interface thats when you use a using statement. And to
            //use it properly you must wrap it as I do below, and then either only have one statement after all of them
            //indented like you do with a if statement with no {, OR have a opening and closing { } where all the code that
            //needs that disposable will run. Whenever it exits that codeblock {}, it will automatically call .dispose() on
            //the disposable. In this case its closing the stream. So the reason everything wasnt working was that all the
            //the streams were immediatly being closed.
            using (WebResponse response = request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
                // DAMIEN : You need to set the posistion of the memory stream to 0 otherwise its never going to actually copy the contents over
                //since it will not start at the beggining of the the stream but at the 
                memoryStream.Position = 0;
                chunk.Data = memoryStream.ToArray();
                return chunk;
            }
        }

        //This method had to be changed to return a concurrent bag so that it is thread safe since you are doing a parallel foreach.
        private static ConcurrentBag<DownloadChunk> GetChunks(int parts, long totalsize)
        {
            ConcurrentBag<DownloadChunk> chunks = new ConcurrentBag<DownloadChunk>();
            long chunksize = totalsize / parts;

            // DAMIEN : Changed this loop to use the parts integer since it is the size already.
            for (int i = 0; i < parts; i++)
            {
                chunks.Add(new()
                {
                    //The math on this was wrong
                    Start = i == 0 ? 0 : chunksize *( i)+1,
                    End = i == 0 ? chunksize : chunksize * (i + 1)
                });
            }
            return chunks;
        }
    }
}




    