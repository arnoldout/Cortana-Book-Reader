using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
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
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.InitializeComponent();
            StorageFile commandArgs = e.Parameter as StorageFile;
            await new Speaker().SpeakTextAsync(await new TxtParser().readFile(commandArgs), this.media);
            /*try
            {
                VoiceCommandActivatedEventArgs commandArgs = e.Parameter as VoiceCommandActivatedEventArgs;
                SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;
                string voiceCommandName = speechRecognitionResult.RulePath[0];
                string textSpoken = speechRecognitionResult.Text;
                IReadOnlyList<string> recognisedVoiceCommandPhrases;

                System.Diagnostics.Debug.WriteLine("Voice CommandName: " + voiceCommandName);
                System.Diagnostics.Debug.WriteLine("text Spoken: " + textSpoken);

                MessageDialog msgDialog = new MessageDialog("");

                switch (voiceCommandName)
                {
                    case "readBook":
                        {
                            System.Diagnostics.Debug.WriteLine("readBook");
                            msgDialog.Content = "readBook";
                            break;
                        }
                    default:
                        {
                            msgDialog.Content = "Unknown Command";
                            break;

                        }
                }   // end Switch(voiceCommandName)
                await msgDialog.ShowAsync();
                await new Speaker().SpeakTextAsync("Alice In Wonderland", this.media);
                await new Speaker().SpeakTextAsync(new TxtParser().readFile("19033-8.txt"), this.media);
                System.Diagnostics.Debug.WriteLine("go to main page Now ");
            }
            catch (Exception)
            {
                MessageDialog msgDialog = new MessageDialog("Crashed");
                await msgDialog.ShowAsync();
            }*/
        }
    }
}
