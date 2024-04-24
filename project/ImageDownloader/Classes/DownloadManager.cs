using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Net.Http;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media;
using System.Threading;

namespace ImageDownloader.Classes
{
    public class ImageDownloader
    {
        HttpClient downloadClient;
        ProgressBar progressBar;

        int chunkSize;

        public bool isFinished { get; private set; } = false;
        public BitmapImage? image { get; private set; }

        const int updateMulitplier = 1;

        string downLink;

        public HttpResponseMessage? response { get; private set; } = null;


        public ImageDownloader(ProgressBar progressBar, string downLink)
        {
            this.downloadClient = new HttpClient();
            this.progressBar = progressBar;
            this.chunkSize = 10;
            this.downLink = downLink;

        }


        // make this one work, definetly not working
        public void ChangeLimiter(int new_limit)
        {
            this.chunkSize = new_limit;
        }

        public async Task EstablishConnection()
        {

            try
            {
                response = await downloadClient.GetAsync(downLink, HttpCompletionOption.ResponseHeadersRead);
            }
            catch
            {
                MessageBox.Show("Could not establish connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }



        // maybe working not properly tested yet
        public async Task Download()                 // vložit all to try body
        {


            if (response == null)
                throw new Exception("Establish connection first");
            

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show($"Invalid image link\nWeb response: {response.StatusCode}");
                return;
            }

            long totalImgSize = response.Content.Headers.ContentLength ?? -1;

            bool canReportState = false;

            if (totalImgSize != -1)
                canReportState = true;

            long totalBytesRead = 0;


            using (var contentStream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = totalImgSize != -1 ? new MemoryStream((int)totalImgSize) : new MemoryStream())
            {
                var buffer = new byte[this.chunkSize * updateMulitplier];
                int bytesRead;

                var chunkTimeStart = Stopwatch.StartNew();

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    chunkTimeStart.Restart();

                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;

                    if (chunkSize != buffer.Length)
                        buffer = new byte[this.chunkSize * updateMulitplier];

                    if (canReportState)
                        progressBar.Value = (double)totalBytesRead / totalImgSize * 100;

                    await Task.Delay(int.Max(0, (1000 * updateMulitplier) - (int)chunkTimeStart.ElapsedMilliseconds));
                }


                // Convert byte[] to BitmapImage
                image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                fileStream.Seek(0, SeekOrigin.Begin);
                image.StreamSource = fileStream;
                image.EndInit();

                isFinished = true;


            }

        }

    }
}
