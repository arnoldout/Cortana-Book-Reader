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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }
        public async void playBook()
        {
            try
            {
                try
                {
                    var folder = ApplicationData.Current.LocalFolder;
                    try
                    {
                        var subFolder = await folder.GetFolderAsync("books");
                        try
                        {
                            var files = await subFolder.GetFilesAsync();
                            foreach (StorageFile sf in files)
                            {
                                if (sf.Name.Equals(App.book))
                                {
                                    IReadOnlyList<StorageFile> q = null;
                                    try
                                    {
                                        subFolder = await folder.GetFolderAsync(sf.Name);
                                        try
                                        {
                                            q = await subFolder.GetFilesAsync();

                                            foreach (StorageFile file in q)
                                            {
                                                try
                                                {
                                                    var stream = await file.OpenAsync(FileAccessMode.Read);
                                                    media.SetSource(stream, "audio/wav");

                                                    media.Play();
                                                }
                                                catch (FileNotFoundException ex)
                                                {
                                                    MessageDialog dialog = new MessageDialog("5" + file.DisplayName + " //" + ex);
                                                    await dialog.ShowAsync();
                                                }
                                            }
                                        }
                                        catch (FileNotFoundException ex)
                                        {
                                            MessageDialog dialog = new MessageDialog("6" + subFolder.DisplayName+ " //" + ex);
                                            await dialog.ShowAsync();
                                        }
                                    }

                                    catch (FileNotFoundException ex)
                                    {
                                        /*
                                         * it just cant find the folder wiht the attached filename
                                         */
                                        MessageDialog dialog = new MessageDialog("4" + q +" //" + ex);
                                        await dialog.ShowAsync();
                                    }
                                }
                            }
                        }

                        catch (FileNotFoundException ex)
                        {
                            MessageDialog dialog = new MessageDialog("3" + ex);
                            await dialog.ShowAsync();
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        MessageDialog dialog = new MessageDialog("2" + ex);
                        await dialog.ShowAsync();
                    }
                }
                catch (FileNotFoundException ex)
                {
                    MessageDialog dialog = new MessageDialog("1" + ex);
                    await dialog.ShowAsync();
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageDialog dialog = new MessageDialog("Sorry, we can't find the file" +ex);
                await dialog.ShowAsync();
            }
            //get naviagation params
            //play it's stored voice file
            /*try
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
                try
                {
                    String commandArgs = e.Parameter as String;
                    var folder = ApplicationData.Current.LocalFolder;
                    var subFolder = await folder.GetFolderAsync("books");
                    var files = await subFolder.GetFilesAsync();
                    foreach (StorageFile sf in files)
                    {
                        if (sf.Name.Equals(commandArgs))
                        {
                            subFolder = await folder.GetFolderAsync(sf.DisplayName);
                            var q = await subFolder.GetFilesAsync();
                            foreach (StorageFile file in q)
                            {
                                var stream = await file.OpenAsync(FileAccessMode.Read);
                                media.SetSource(stream, "audio/wav");

                                media.Play();
                            }
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    try
                    {
                        var folder = ApplicationData.Current.LocalFolder;
                        var subFolder = await folder.GetFolderAsync("books");
                        var files = await subFolder.GetFilesAsync();
                        foreach (StorageFile sf in files)
                        {
                            if (sf.Name.Equals(App.book))
                            {
                                subFolder = await folder.GetFolderAsync(sf.DisplayName);
                                var q = await subFolder.GetFilesAsync();
                                foreach (StorageFile file in q)
                                {
                                    var stream = await file.OpenAsync(FileAccessMode.Read);
                                    media.SetSource(stream, "audio/wav");

                                    media.Play();
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageDialog dialog = new MessageDialog("Sorry, we can't find the file");
                        await dialog.ShowAsync();
                    }
                }
            }*/
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            media.Position = new TimeSpan(0, 0, 7);
        }
    }
}
