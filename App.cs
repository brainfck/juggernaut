using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace juggernaut
{
    public class App
    {
        private static string NasaImageOfTheDayRSSUrl = @"https://www.nasa.gov/rss/dyn/lg_image_of_the_day.rss";

        private readonly IImageDownloader _imageDownloader;

        public App(IImageDownloader imageDownloader)
        {
            _imageDownloader = imageDownloader;
        }

        public async Task Run(string directoryPath = "")
        {
            await DownloadImage(directoryPath);
        }

        private Uri GetDailyImageUri()
        {
            using var reader = XmlReader.Create(NasaImageOfTheDayRSSUrl);
            var feed = SyndicationFeed.Load(reader);

            var dailyImage = feed
                .Items
                .FirstOrDefault()
                ?.Links
                ?.FirstOrDefault(x => x.RelationshipType == "enclosure");

            return dailyImage?.Uri;
        }

        private async Task DownloadImage(string directoryPath = "")
        {
            if (string.IsNullOrEmpty(directoryPath)) directoryPath = Environment.CurrentDirectory;
            var fileName = DateTime.Now.Date.ToShortDateString().Replace(@"/", ".");
            
            var uri = GetDailyImageUri();

            await _imageDownloader.DownloadImageAsync(directoryPath, fileName, uri);
        }

        private async Task DownloadAllImages(string directoryPath = "")
        {
            using var reader = XmlReader.Create(NasaImageOfTheDayRSSUrl);
            var feed = SyndicationFeed.Load(reader);

            if (string.IsNullOrEmpty(directoryPath)) directoryPath = Environment.CurrentDirectory;

            foreach (var item in feed.Items)
            {
                var image = item
                    ?.Links
                    ?.FirstOrDefault(x => x.RelationshipType == "enclosure");

                var uri = image?.Uri;

                if (uri == null)
                    continue;

                var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                var fileName = Path.GetFileNameWithoutExtension(uriWithoutQuery);

                await _imageDownloader.DownloadImageAsync(directoryPath, fileName, uri);
            }
        }
    }
}
