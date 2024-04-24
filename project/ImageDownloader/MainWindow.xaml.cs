using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageDownloader.Classes;
using Microsoft.Win32;

namespace ImageDownloader
{

    public partial class MainWindow : Window
    {
        // Store references to finalize and edit buttons
        public Button button_FinalizeDownload { get; private set; }
        public Button button_EditImage { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            // Assign buttons from XAML to properties
            button_FinalizeDownload = ButtonFinalize;
            button_EditImage = ButtonEdit;
        }

        private void InitiateDownload(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_downLink.Text)) return;

            if (stackPanel_downloadElems.Children.Count >= 5)
            {
                MessageBox.Show("You can download only 5 images at once.", "Max downloads", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "JPEG Image|*.jpg";

            if (saveDialog.ShowDialog() == true)
            {
                var downloadTask = new ImageDownloadTask(textBox_downLink.Text, saveDialog.FileName);

                // Create a new instance of DownloadElement
                DownloadElement newDownloadElement = new DownloadElement(downloadTask, this);

                Task.Run(() => newDownloadElement.downloadTask.Download());

                // Add child element to StackPanel element
                stackPanel_downloadElems.Children.Add(newDownloadElement);

                // Clear TextBox element
                textBox_downLink.Text = "";
            }

        }

        // Unchecks all other elements in StackPanel and leave only one selected
        public void SingleToggle(object sender)
        {
            var toggledButton = sender as ToggleButton;
            foreach (var child in stackPanel_downloadElems.Children)
                if (child is DownloadElement downloadElement && downloadElement != toggledButton && downloadElement.IsChecked == true)
                    downloadElement.IsChecked = false;
        }


        // Removes element from StackPanel
        public void RemoveDownElement(object sender)
        {
            stackPanel_downloadElems.Children.Remove((UIElement)sender);
        }

    }
}
