using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace juggernaut
{
    public interface IImageDownloader
    {
        Task DownloadImageAsync(string directoryPath, string fileName, Uri uri);
    }
}
