using BookReader.ViewModels;
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
using Windows.ApplicationModel.VoiceCommands;
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
        BooksVM books;
        public MainPage()
        {
            this.InitializeComponent();
            var folder = ApplicationData.Current.LocalFolder;
            
            books = new BooksVM();
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
        public void ReadBook()
        {
            return;
        }

        private async void ReadBook_Click(object sender, RoutedEventArgs e)
        {
            //open file picker
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //show icons as thumbnails
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //open on desktop if not opened before
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            //only need txt or epub files
            picker.FileTypeFilter.Add(".txt");

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
                var folder = ApplicationData.Current.LocalFolder;
                StorageFolder subFolder;
                try
                {
                    subFolder = await folder.GetFolderAsync("books");
                }
                catch (FileNotFoundException)
                {
                    subFolder = await folder.CreateFolderAsync("books");
                }
                /*await subFolder.DeleteAsync();
                await folder.CreateFolderAsync("books");
                var k = await folder.GetFoldersAsync();
                foreach(StorageFolder sf in k)
                {
                    await sf.DeleteAsync();
                }*/
                if (!await isFilePresent(file.Name, subFolder))
                {
                    file = await subFolder.CreateFileAsync(file.Name);
                    await FileIO.WriteBytesAsync(file, result);
                    String str = await new TxtParser().readFile(file);
                    Book b = new Book(str);
                    String read = "";
                    int counter = 0;
                    var fileFolder = await folder.CreateFolderAsync(file.DisplayName);
                    MessageDialog dialog = new MessageDialog("Synthesizing speech, this can take a few minutes, we'll let you know when we're done" +
                        " Please don't close the app");
                    await dialog.ShowAsync();
                    while ((read = b.popSegment()) != null)
                    {
                        await new Speaker().StoreText(read, file.DisplayName, counter.ToString(), fileFolder);
                        counter++;
                    }
                    updateBookPhrases();
                    dialog = new MessageDialog("File Synthesized, thank you for waiting");
                    await dialog.ShowAsync();
                    //update bindings
                    books.AddBook(file.Name);
                    this.DataContextChanged += (s, DataContextChangedEventArgs) => this.Bindings.Update();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Book with that name already in library");
                    await dialog.ShowAsync();
                }
            }
        }
        public async Task<bool> isFilePresent(string fileName, StorageFolder folder)
        {
            var item = await folder.TryGetItemAsync(fileName);
            return item != null;
        }

        private async void listBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems?.FirstOrDefault();
            String bookName = (String)item;
            var folder = ApplicationData.Current.LocalFolder;
            var subFolder = await folder.GetFolderAsync("books");
            var sf = await subFolder.GetFileAsync(bookName);
            Frame.Navigate(typeof(EnterByVoice), sf);
        }

        public async void updateBookPhrases()
        {
            List<string> books = new List<string>();
            var folder = ApplicationData.Current.LocalFolder;
            var subFolder = await folder.GetFolderAsync("books");
            var files = await subFolder.GetFilesAsync();
            foreach (StorageFile sf in files)
            {
                books.Add(sf.DisplayName);
            }
            VoiceCommandDefinition commandDefinitions;
            if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue("UniversalAppCommandSet_en-us", out commandDefinitions))
            {
                await commandDefinitions.SetPhraseListAsync("bookName", books);
            }
        }
    }
}