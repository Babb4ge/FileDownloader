using System;
using System.IO;
using System.Net;
using System.Net.Http;
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
            string Link;
            string PostFilePath = @"C: \Users\Derpy\Downloads";
            do
            {
                Console.WriteLine("Paste the Download Link here");
                Console.Write(">");
                Link = Console.ReadLine();

                if (string.IsNullOrEmpty(Link) || !Uri.IsWellFormedUriString(Link, UriKind.Absolute))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid Link Try Again | Press Anything to Continue");
                    Console.ReadKey(true);
                    Console.Clear();
                }
            } while ((string.IsNullOrEmpty(Link) || !Uri.IsWellFormedUriString(Link, UriKind.Absolute)));

            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, Link));
            

            if (response.IsSuccessStatusCode)
            {
                Console.Clear();
            }


            var contentDisposition = response.Content.Headers.ContentDisposition;
            
            string contenttype = response.Content.Headers.ContentType.MediaType;
            string viewtype = contenttype;
            string[] parts = contenttype.Split("/");
            string Fcontenttype = ("." + parts[1]);

            Console.WriteLine("Is this the correct file type? || " + Fcontenttype + "\n Press 1 for YES\n Press 2 for NO");
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                        Console.Clear();
                        Finalize();
                        break;
                    case ConsoleKey.NumPad1:
                        Console.Clear();
                        Finalize();
                        break;
                    case ConsoleKey.D2:
                        Console.Clear();
                        SelfType(contentDisposition.FileName);
                        break;
                    case ConsoleKey.NumPad2:
                        Console.Clear();
                        SelfType(contentDisposition.FileName);
                        break;
                }
            }
            while (key != ConsoleKey.D1 && key != ConsoleKey.NumPad1 && key != ConsoleKey.NumPad2 && key != ConsoleKey.D2);
        }



        static void SelfType(string FileName)
        {

            Console.WriteLine("Input the correct file type");
            Console.Write(">");
            string NewPreType = Console.ReadLine();
            string NewPostType = "." + NewPreType;
            Console.WriteLine(FileName + NewPostType);
        }

        static void Finalize()
        {

        }

    }

    }


    