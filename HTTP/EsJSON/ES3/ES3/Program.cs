using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HttpProxyControl;

public class Foto
{
    public int AlbumId { get; set; }
    public int Id { get; set; }
    public string? Titolo { get; set; }
    public string? Url { get; set; }
    public string? UrlThumbnail { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        string url = "https://jsonplaceholder.typicode.com/photos";
        await GetAndProcessPhotos(url);
    }

    static async Task GetAndProcessPhotos(string apiUrl)
    {
        try
        {
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);
            client.BaseAddress = new Uri(apiUrl);
            List<Foto>? photos = await client.GetFromJsonAsync<List<Foto>>("");

            if (photos != null && photos.Count > 0)
            {
                string path = AppContext.BaseDirectory;
                string? appDir = Path.GetDirectoryName(path);
                string photoSubDir = "cachedPhotos";
                string photoDir = Path.Combine(appDir, photoSubDir);
                if (!Directory.Exists(photoDir))
                    Directory.CreateDirectory(photoDir);
                string thumbnailSubDir = "thumbnailPhotos";
                string thumbnailDir = Path.Combine(appDir, thumbnailSubDir);
                if (!Directory.Exists(thumbnailDir))
                    Directory.CreateDirectory(thumbnailDir);

                foreach (var foto in photos.GetRange(0, Math.Min(photos.Count, 10)))
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(foto.Url);
                    byte[] thumbnailBytes = await client.GetByteArrayAsync(foto.UrlThumbnail);

                    string localFileNamePhoto = GetFileNameFromUrl(foto.Url) + ".png";
                    string localPathPhoto = Path.Combine(photoDir, localFileNamePhoto);
                    await File.WriteAllBytesAsync(localPathPhoto, imageBytes);

                    string localFileNameThumbnail = GetFileNameFromUrl(foto.UrlThumbnail) + ".png";
                    string localPathThumbnail = Path.Combine(thumbnailDir, localFileNameThumbnail);
                    await File.WriteAllBytesAsync(localPathThumbnail, thumbnailBytes);
                }

                Console.WriteLine("Photos and thumbnails saved successfully.");
            }
            else
            {
                Console.WriteLine("No photos found.");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"HttpRequestException: {e.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    static string GetFileNameFromUrl(string url)
    {
        Uri SomeBaseUri = new("http://canbeanything");
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            uri = new Uri(SomeBaseUri, url);
        //Path.GetFileName funziona se ha in input un URL assoluto
        return Path.GetFileName(uri.LocalPath);
    }
}
