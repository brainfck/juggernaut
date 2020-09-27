using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace juggernaut
{
    public class ImageDownloader : IImageDownloader, IDisposable
    {
        private bool _disposed;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public ImageDownloader()
        {
            _httpClient ??= new HttpClient();
        }

        public ImageDownloader(ILogger<ImageDownloader> logger, HttpClient httpClient = null)
        {
            _logger = logger;
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Downloads an image asynchronously from the <paramref name="uri"/>
        /// and places it in the specified <paramref name="directoryPath"/>
        /// with the specified <paramref name="fileName"/>.
        /// </summary>
        /// <param name="directoryPath">The relative or absolute path to the directory to place the image in.</param>
        /// <param name="fileName">The name of the file without the file extension.</param>
        /// <param name="uri">The URI for the image to download.</param>
        public async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (uri == null)
            {
                throw new ArgumentNullException(GetType().FullName);
            }

            try
            {
                _logger.LogInformation("Starting download of image.");
                _logger.LogInformation($"Downloading into: {directoryPath}, file name {fileName}.");

                var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                var fileExtension = Path.GetExtension(uriWithoutQuery);

                if (fileExtension == ".png")
                {
                    _logger.LogInformation("Image is .png, changing to .jpg.");
                    fileExtension = ".jpg";
                }

                var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");

                if (!Directory.Exists(directoryPath))
                {
                    _logger.LogInformation($"Directory {directoryPath} does not exists, creating...");
                    Directory.CreateDirectory(directoryPath);
                    _logger.LogInformation($"Directory {directoryPath} created.");
                }

                _logger.LogInformation("Trying to download image...");

                var imageBytes = await _httpClient.GetByteArrayAsync(uri);
                var image = Image.Load(imageBytes);

                _logger.LogInformation($"Image downloaded. Saving image...");

                await image.SaveAsync(path);

                _logger.LogInformation($"Image successfully saved.");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Error while downloading image. Try checking your internet connection. Exception message: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message); 
                throw;
            }
        }

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }
}
