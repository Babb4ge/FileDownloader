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
        static async Task FFDLMAIN()
        {

            Console.WriteLine("Paste the Download Link here");
            Console.Write(">");
            string Link = Console.ReadLine()??"";

            string PostFilePath = @"C: \Users\Derpy\Downloads\";
            //Pulls Download Prompt
            //Gets File Size
            long filesize = FileInformation.FiSize(Link);
            string math = IMath.ConvertViaMath(filesize);
            Console.WriteLine(math + " " + Link);
            Console.WriteLine("What would you like to name the file?");
            string fileName = Console.ReadLine()??"";
            Console.Clear();

            string filetype = FileInformation.FileType(Link);
            string[] parts = filetype.Split("/");
            string Fcontenttype = ("." + parts[1]);

            Console.WriteLine("Is this the correct file type? || " + fileName + Fcontenttype + "\n Press 1 for YES\n Press 2 for NO");

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
            while (key != ConsoleKey.D1 && key != ConsoleKey.NumPad1 && key != ConsoleKey.NumPad2 && key != ConsoleKey.D2);
        }



        private static void SelfType(string fileName, string Link)
        {
            Console.WriteLine(fileName + Link);
            Console.WriteLine("Input the correct file type");
            Console.Write(">");
            string NewPreType = Console.ReadLine();
            string Fcontenttype = "." + NewPreType;
            Console.Clear();
            SelfFinalize(fileName, Fcontenttype, Link);


        }
        //broken not wokring
        static async Task SelfFinalize(string fileName, string Fcontenttype, string Link)
        {
            Console.WriteLine(fileName + Fcontenttype, Link);
            long filesize;
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Link, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                filesize = FileInformation.FiSize(Link);
            }

            int chunksize = (int)Math.Ceiling(Convert.ToDouble(filesize) / 4);

            for (int i = 0; i < 4; i++)
            {
                int start = i * chunksize;
                int end = (i == 3) ? (int)filesize - 1 : start + chunksize - 1;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Range = new RangeHeaderValue(start, end);
                    using (var response = await client.GetAsync(Link, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var fileStream = new FileStream("C:\\Users\\Derpy\\Downloads\\" + fileName + "." + Fcontenttype, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }
                    }
                }
            }
        }
    }
}




    