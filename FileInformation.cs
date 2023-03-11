using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Net.Http.Headers;

namespace FileDownloader
{
    public class FileInformation
    {

        static async Task<long> GetFileSizeAsync(string url)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content.Headers.TryGetValues("Content-Length", out var values))
                        {

                            long filesize = long.Parse(values.First());
                            return filesize;
                        }
                        else
                        {
                            throw new WebException("Unable to parse the content length");
                        }
                    }
                    else
                    {
                        throw new WebException($"Error retreiving the file size: {response.StatusCode}");
                    }
                }
            }
        }
        public static long FiSize(string url)
        {
            return GetFileSizeAsync(url).Result;

        }
        //Retreives the file type
        static async Task<string> GetFileTypeAsync(string url)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content.Headers.TryGetValues("Content-Type", out var values))
                        {
                            var mediaType = MediaTypeHeaderValue.Parse(values.First());
                            return mediaType.MediaType.ToString();
                        }
                        else
                        {
                            return "Cannot retrieve content type";

                        }


                    }
                    else
                    {
                        return $"Error retrieving the download link: {response.StatusCode}";
                    }
                }
            }
        }

        public static string FileType(string url)
        {
            return GetFileTypeAsync(url).Result;
        }
    }
}
