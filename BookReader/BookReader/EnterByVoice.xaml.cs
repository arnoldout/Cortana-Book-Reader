using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BookReader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EnterByVoice : Page
    {
        public EnterByVoice()
        {
            this.InitializeComponent();
            playBook();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.book.Equals(""))
            {
                try
                {
                    StorageFile commandArgs = e.Parameter as StorageFile;
                    var folder = ApplicationData.Current.LocalFolder;
                    var subFolder = await folder.GetFolderAsync(commandArgs.DisplayName);
                    var q = await subFolder.GetFilesAsync();
                    foreach (StorageFile file in q)
                    {
                        var stream = await file.OpenAsync(FileAccessMode.Read);
                        media.SetSource(stream, "audio/wav");

                        media.Play();
                    }
                }
                catch (FileNotFoundException)
                {
                    MessageDialog dialog = new MessageDialog("Sorry, we can't find the file");
                    await dialog.ShowAsync();
                }
            }

        }
        public async void playBook()
        {
            if (!App.book.Equals(""))
            {
                var folder = ApplicationData.Current.LocalFolder;
                var subFolder = await folder.GetFolderAsync("books");
                var files = await subFolder.GetFilesAsync();
                foreach (StorageFile sf in files)
                {
                    if (sf.Name.Equals(App.book))
                    {
                        IReadOnlyList<StorageFile> q = null;
                        subFolder = await folder.GetFolderAsync(sf.Name);
                        q = await subFolder.GetFilesAsync();

                        foreach (StorageFile file in q)
                        {
                            var stream = await file.OpenAsync(FileAccessMode.Read);
                            media.SetSource(stream, "audio/wav");
                            App.book = "";
                            media.Play();
                        }
                    }
                }
            }
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            media.Position = new TimeSpan(0, 0, 7);
        }
    }
}
