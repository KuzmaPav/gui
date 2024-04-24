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

namespace ImageDownloader
{

    public partial class MainWindow : Window
    {
        // Store references to finalize and edit buttons
        public Button btn_FinalizeDownload { get; private set; }
        public Button btn_EditImage { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            // Assign buttons from XAML to properties
            btn_FinalizeDownload = ButtonFinalize;
            btn_EditImage = ButtonEdit;
        }

        private async void InitiateDownload(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox_downLink.Text))
            {


                if (stackPanel_downloadElems.Children.Count < 5)
                {

                    string downLink = textBox_downLink.Text;

                    // Clear TextBox element
                    textBox_downLink.Text = "";

                    // Create a new instance of DownloadElement
                    DownloadElement newDownloadElement = new DownloadElement(downLink, this);

                    await Task.Run(() => newDownloadElement.imageDownloader.EstablishConnection());

                    if (newDownloadElement.imageDownloader.response != null)
                    {
                        // Add child element to StackPanel element
                        stackPanel_downloadElems.Children.Add(newDownloadElement);

                        // Run async function (method) 
                        await newDownloadElement.imageDownloader.Download();
                    }

                }
                else
                {
                    MessageBox.Show("You can download only 5 images at once.","No more");
                }

                
            }
        }
    }
}
