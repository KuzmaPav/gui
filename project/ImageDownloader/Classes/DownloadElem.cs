using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


// manually added
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Data;
using System.Windows.Media.Animation;

namespace ImageDownloader.Classes
{
    public class DownloadElement : ToggleButton
    {
        public ImageDownloadTask downloadTask { get; private set; }

        ProgressBar progressBar;

        TextBox limitTextBox;

        Slider limitSlider;

        Label linkLabel;

        MainWindow parentWindow;

        public DownloadElement(ImageDownloadTask downloadTask, MainWindow parentWindow)
        {
            // Toggle element (Download element) properties:
            BorderThickness = new Thickness(2);
            Height = 50;
            Margin = new Thickness(10);

            // Actions:
            Checked += ThisChecked;
            Unchecked += ThisUnchecked;

            //----------------------------------------------------------------------------------------
            this.parentWindow = parentWindow;

            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            linkLabel = new Label { Content = downloadTask.downLink, MaxWidth = 200, Margin = new Thickness(5) };
            stackPanel.Children.Add(linkLabel);

            var downLimitLabel = new Label { Content = "Down limit:", Margin = new Thickness(5, 5, 0, 5) };
            stackPanel.Children.Add(downLimitLabel);

            // Textbox for download speed
            limitTextBox = new TextBox { Text = "10", Width = 40, Height = 20, Margin = new Thickness(0, 5, 0, 5) };
            limitTextBox.TextChanged += LimitTextBox_TextChanged;
            stackPanel.Children.Add(limitTextBox);

            var unitLabel = new Label { Content = "KB/s", Margin = new Thickness(0, 5, 5, 5) };
            stackPanel.Children.Add(unitLabel);

            // Slider for download speed
            limitSlider = new Slider { Minimum = 1, Maximum = 1_000, Value = 10, Width = 50, Height = 20, Margin = new Thickness(5) };
            limitSlider.ValueChanged += LimitSlider_ValueChanged;
            stackPanel.Children.Add(limitSlider);

            // ProgressBar to show progress of the download
            progressBar = new ProgressBar { MinWidth = 25, Width = 100, Minimum = 0, Maximum = 1, Height = 20, Margin = new Thickness(5) };
            stackPanel.Children.Add(progressBar);

            Content = stackPanel;

            //-----------------------------------------------------------------------------------------

            this.downloadTask = downloadTask;
            downloadTask.AddObserver(this);
        }


        // Connect TextBox value with Slider value
        private void LimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(limitTextBox.Text, out int value))
            {
                limitSlider.Value = value;
                downloadTask.ChangeLimiter(value);
            }
        }

        private void LimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            limitTextBox.Text = Math.Round(limitSlider.Value, 0).ToString();
            downloadTask.ChangeLimiter(int.Parse(limitTextBox.Text));
        }



        private void ThisChecked(object sender, RoutedEventArgs e)
        {
            parentWindow.SingleToggle(sender);

            if (downloadTask.state == "finished")
            {
                parentWindow.button_EditImage.IsEnabled = true;
                parentWindow.button_EditImage.Click += EditImage;

                parentWindow.button_FinalizeDownload.IsEnabled = true;
                parentWindow.button_FinalizeDownload.Content = "Close finished";
                parentWindow.button_FinalizeDownload.Click += FinalizeDownload;
            }
            else if (downloadTask.state == "fail")
            {
                parentWindow.button_FinalizeDownload.IsEnabled = true;
                parentWindow.button_FinalizeDownload.Content = "Close unfinished";
                parentWindow.button_FinalizeDownload.Click += FinalizeDownload;
            }
        }


        private void ThisUnchecked(object sender, RoutedEventArgs e)
        {
            parentWindow.button_EditImage.IsEnabled = false;
            parentWindow.button_EditImage.Click -= EditImage;

            parentWindow.button_FinalizeDownload.IsEnabled = false;
            parentWindow.button_FinalizeDownload.Content = "Close element";
            parentWindow.button_FinalizeDownload.Click -= FinalizeDownload;
        }


        public void UpdateProgress(double value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                progressBar.Value = value;
            });
        }


        public void OnFail()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BorderBrush = Brushes.Red;
                
            });
        }

        public void OnFinish()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BorderBrush = Brushes.Green;
            });
        }

        
        private void FinalizeDownload(object sender, RoutedEventArgs e)
        {
            ThisUnchecked(this, e);
            parentWindow.RemoveDownElement(this);
        }
        

        private void EditImage(object sender, RoutedEventArgs e)
        {
            // Your EditImage logic here
            MessageBox.Show("Edit here");
        }

    }
}