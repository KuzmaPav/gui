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
using System.Linq.Expressions;

namespace ImageDownloader.Classes
{
    public class ImageDownloadTask
    {
        HttpClient downloadClient;

        List<DownloadElement> observers = new List<DownloadElement>();

        int chunkSize;

        public string state { get; private set; } = "inprogress";

        public string downLink { get; private set; }

        public string targetFile { get; private set; }


        // For decreasing visual load for slower pc
        const int updateMulitplier = 1;

        public ImageDownloadTask(string downLink, string targetFile)
        {
            this.downloadClient = new HttpClient();
            this.chunkSize = 10;
            this.downLink = downLink;
            this.targetFile = targetFile;
        }

        public void AddObserver(DownloadElement observer) =>
            observers.Add(observer);
        

        public void ChangeLimiter(int new_limit) =>
            this.chunkSize = new_limit;
        

        //Observer notifying 
        private void NotifyFail()
        {
            foreach (var observer in observers)
                observer.OnFail();
        }

        private void NotifyFinish()
        {
            foreach (var observer in observers)
                observer.OnFinish();
        }

        private void UpdateProgress(double value)
        {
            foreach (var observer in observers)
                observer.UpdateProgress(value);
        }



        public async Task Download()
        {
            // Exceptions checking
            HttpResponseMessage? response;

            try
            {
                response = await downloadClient.GetAsync(downLink, HttpCompletionOption.ResponseHeadersRead);
            }
            catch
            {
                response = null;
            }


            if(response == null)
            {
                NotifyFail();
                state = "failed";
                return;
            }
            
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show($"Invalid image link\nWeb response: {response.StatusCode}");
                NotifyFail();
                state = "failed";
                return;
            }

            if (!response.Content.Headers.ContentType.ToString().Contains("image"))
            {
                MessageBox.Show("Given url is not an image url.");
                NotifyFail();
                state = "failed";
                return;
            }

            // Declaring variables
            long totalImgSize = response.Content.Headers.ContentLength ?? -1;

            bool canReportState = false;

            if (totalImgSize != -1)
                canReportState = true;

            long totalBytesRead = 0;

            var buffer = new byte[this.chunkSize * updateMulitplier];
            int bytesRead;

            var chunkTimeStart = Stopwatch.StartNew();

            // Logic
            try
            {
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        chunkTimeStart.Restart();

                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;

                        if (chunkSize != buffer.Length)
                            buffer = new byte[this.chunkSize * updateMulitplier];


                        if (canReportState)
                            UpdateProgress((double)totalBytesRead / totalImgSize);

                        await Task.Delay(int.Max(0, (1000 * updateMulitplier) - (int)chunkTimeStart.ElapsedMilliseconds));
                    }

                    state = "finished";
                    NotifyFinish();
                }
            }

            catch
            {
                state = "failed";
                NotifyFail();
            }
                
        }
    }
}

