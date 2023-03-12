using Microsoft.Extensions.Logging;
using System;
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
            DownloadChunk[] chunks = GetChunks(4 , size);

            Parallel.ForEach(chunks, (chunk) =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(Link);
                request.AddRange(chunk.Start, chunk.End);
                using WebResponse response = request.GetResponse();
                using Stream responseStream = response.GetResponseStream();
                byte[] buffer = new byte[1024];
                int bytesRead;
                using MemoryStream memoryStream = new MemoryStream();
                while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
                chunk.Data = memoryStream.ToArray();
            });
            
            using FileStream fs = new("C:\\Users\\Derpy\\Downloads\\" + fileName + Fcontenttype, FileMode.Create);
            foreach (DownloadChunk chunk in chunks)
            {
                fs.Write(chunk.Data, 0, chunk.Data.Length);
            }
            

        }
        private static DownloadChunk[] GetChunks(int parts, long totalsize)
        {
            DownloadChunk[] chunks = new DownloadChunk[parts];
            long chunksize = totalsize / parts;

            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i] = new()
                {
                    Start = i == 0 ? 0 : chunksize = i + 1,
                    End = i == 0 ? chunksize : i == chunks.Length - 1 ? totalsize : chunksize * i + chunksize
                };
            }
            return chunks;
        }
    }
}




    