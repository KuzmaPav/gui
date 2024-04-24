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



namespace ImageDownloader.Classes
{
    public class DownloadElement : ToggleButton
    {
        public ImageDownloader imageDownloader { get; private set; }

        public ProgressBar progressBar { get; private set; }

        public TextBox limitTextBox { get; private set; }

        public Slider limitSlider { get; private set; }

        public Label linkLabel { get; private set; }

        public MainWindow ParentWindow { get; set; }

        

        public DownloadElement(string downLink, MainWindow parentWindow)
        {
            // Toggle element (Download element) properties:

            BorderBrush = Brushes.Gray;
            BorderThickness = new Thickness(2);
            Height = 50;
            Margin = new Thickness(10);

            // Actions:
            Checked += SingleToggle;
            Unchecked += ToggleUnchecked;


            //----------------------------------------------------------------------------------------
            ParentWindow = parentWindow;

            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            linkLabel = new Label { Content = downLink, MaxWidth = 200, Margin = new Thickness(5) };
            stackPanel.Children.Add(linkLabel);

            var downLimitLabel = new Label { Content = "Down limit:", Margin = new Thickness(5, 5, 0, 5) };
            stackPanel.Children.Add(downLimitLabel);

            // Use TextBox instead of Label for the limit value
            limitTextBox = new TextBox { Text = "10", Width = 40, Height = 20, Margin = new Thickness(0, 5, 0, 5) };
            limitTextBox.TextChanged += LimitTextBox_TextChanged;
            stackPanel.Children.Add(limitTextBox);

            var unitLabel = new Label { Content = "KB/s", Margin = new Thickness(0, 5, 5, 5) };
            stackPanel.Children.Add(unitLabel);

            // Slider for speed
            limitSlider = new Slider { Minimum = 1, Maximum = 1_000, Value = 10, Width = 50, Height = 20, Margin = new Thickness(5) };
            limitSlider.ValueChanged += LimitSlider_ValueChanged;
            stackPanel.Children.Add(limitSlider);

            // Initialize and add ProgressBar to the stackPanel
            progressBar = new ProgressBar { MinWidth = 25, Width = 100, Minimum = 0, Maximum = 100, Height = 20, Margin = new Thickness(5) };
            stackPanel.Children.Add(progressBar);

            Content = stackPanel;

            imageDownloader = new ImageDownloader(progressBar, downLink);
        }

        private void LimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(limitTextBox.Text, out int value))
            {
                limitSlider.Value = value;
                imageDownloader.ChangeLimiter(value);
            }
        }

        private void LimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            limitTextBox.Text = Math.Round(limitSlider.Value, 0).ToString();
            imageDownloader.ChangeLimiter(int.Parse(limitTextBox.Text));
        }

        private void SingleToggle(object sender, RoutedEventArgs e)
        {
            var toggledButton = sender as ToggleButton;
            foreach (var child in ParentWindow.stackPanel_downloadElems.Children)
                if (child is DownloadElement downloadElement && downloadElement != this && downloadElement.IsChecked == true)
                    downloadElement.IsChecked = false;
            

            // If this button is checked, associate functions to finalize and edit
            if (toggledButton.IsChecked == true && imageDownloader.isFinished)
            {
                ParentWindow.btn_EditImage.IsEnabled = true;
                ParentWindow.btn_FinalizeDownload.IsEnabled = true;


                ParentWindow.btn_FinalizeDownload.Click += FinalizeDownload;
                ParentWindow.btn_EditImage.Click += EditImage;
            }

            else
                ToggleUnchecked(sender,e);
            
        }

        private void ToggleUnchecked(object sender, RoutedEventArgs e)
        {
            ParentWindow.btn_EditImage.IsEnabled = false;
            ParentWindow.btn_FinalizeDownload.IsEnabled = false;

            // If this button is unchecked, remove associations
            ParentWindow.btn_FinalizeDownload.Click -= FinalizeDownload;
            ParentWindow.btn_EditImage.Click -= EditImage;

        }



        private async void FinalizeDownload(object sender, RoutedEventArgs e)
        {

            if (imageDownloader.isFinished)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JPEG Image|*.jpg";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        // Save downloaded BitmapImage to file
                        var encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imageDownloader.image));
                        using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            encoder.Save(fileStream);
                        }

                        ToggleUnchecked(sender, e);
                        ParentWindow.stackPanel_downloadElems.Children.Remove(this);
                    }
                });
            }
            else
                MessageBox.Show("No image downloaded yet.");    // maybe remove messagebox
            
        }

        private void EditImage(object sender, RoutedEventArgs e)
        {
            // Your EditImage logic here
            MessageBox.Show("Edit here");
        }
    }
}
