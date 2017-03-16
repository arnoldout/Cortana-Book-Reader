using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BookReader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //parseEpub(@"c:\users\olivr\documents\visual studio 2015\Projects\BookReader\BookReader\AnimalFarm.epub");

        }

        public async void parseEpub(String zipPath)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            //var file = await localFolder.GetFileAsync("file.json");
            //await file.DeleteAsync();
            var archive = await localFolder.GetFileAsync(zipPath);
            ZipFile.ExtractToDirectory(archive.Path, localFolder.Path);
        }

        private async void ReadBook_Click(object sender, RoutedEventArgs e)
        {
            String str = new TxtParser().readFile("19033-8.txt");
            await new Speaker().SpeakTextAsync(str, this.mediaElement);

        }
    }
}
