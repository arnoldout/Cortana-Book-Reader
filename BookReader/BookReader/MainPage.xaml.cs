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
            List<String> t = new List<String>();
            
            listBooks.ItemsSource = t;
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
            var folder = ApplicationData.Current.LocalFolder;
            var files = await folder.GetFilesAsync();
            var desiredFile = files.FirstOrDefault(x => x.Name == "books.txt");
            if(desiredFile==null)
            {
                desiredFile = await folder.CreateFileAsync("books.txt");
            }
            //open file picker
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //show icons as thumbnails
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //open on desktop if not opened before
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            //only need txt or epub files
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".epub");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            
            if (file != null)
            {
                IRandomAccessStream sr = await file.OpenReadAsync();
                byte[] result;
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        result = memoryStream.ToArray();
                    }
                }
                await FileIO.WriteTextAsync(file, result);
            }
            


            String str = new TxtParser().readFile(@st2r);
            await new Speaker().SpeakTextAsync(str, this.mediaElement);

        }
    }
}
