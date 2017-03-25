using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
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

namespace BookReader
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        public static String book = "";

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            try
            {
                System.Diagnostics.Debug.WriteLine("VCD loading attempted. ");
                StorageFile vcdFile = await Package.Current.InstalledLocation.GetFileAsync("VoiceCommandDefinition.xml");

                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdFile);


                VoiceCommandDefinition commandDefinitions;

                if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue("UniversalAppCommandSet_en-us", out commandDefinitions))
                {
                    List<string> books = new List<string>();
                    var folder = ApplicationData.Current.LocalFolder;
                    var subFolder = await folder.GetFolderAsync("books");
                    var files = await subFolder.GetFilesAsync();
                    foreach (StorageFile sf in files)
                    {
                        books.Add(sf.DisplayName);
                    }

                    await commandDefinitions.SetPhraseListAsync("bookName", books);
                }
            }
            catch (Exception ex)
            {
                MessageDialog msgDialog = new MessageDialog("Error Reading VCD File " + ex);
                await msgDialog.ShowAsync();
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }
        /// <summary>
        /// Invoked when the application is activated by some other means other than launching
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (args.Kind == ActivationKind.VoiceCommand)
            {
                try
                {
                    // now get the parameters pased in
                    VoiceCommandActivatedEventArgs commandArgs = args as VoiceCommandActivatedEventArgs;
                    SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;
                    string voiceCommandName = speechRecognitionResult.RulePath[0];
                    string textSpoken = speechRecognitionResult.Text;
                    IReadOnlyList<string> recognisedVoiceCommandPhrases;
                    MessageDialog msgDialog = new MessageDialog("vcn:" + voiceCommandName + "txtspn" + textSpoken);
                    await msgDialog.ShowAsync();
                    System.Diagnostics.Debug.WriteLine("Voice CommandName: " + voiceCommandName);
                    System.Diagnostics.Debug.WriteLine("text Spoken: " + textSpoken);

                    switch (voiceCommandName)
                    {
                        case "readBook":
                            {
                                MessageDialog dialog = new MessageDialog("Still just hit the switch");
                                await dialog.ShowAsync();
                                string bookName = this.SemanticInterpretation("bookName", speechRecognitionResult);
                                var folder = ApplicationData.Current.LocalFolder;
                                var subFolder = await folder.GetFolderAsync("books");
                                var files = await subFolder.GetFilesAsync();
                                foreach (StorageFile sf in files)
                                {
                                    msgDialog = new MessageDialog("Looking for " + bookName + ", found " + sf.Name);
                                    await msgDialog.ShowAsync();

                                    if (sf.DisplayName.Equals(bookName))
                                    {
                                        book = sf.Name;
                                        break;
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                msgDialog.Content = "Unknown Command";
                                break;

                            }
                    }   // end Switch(voiceCommandName)

                    System.Diagnostics.Debug.WriteLine("go to main page Now ");
                }
                catch (Exception)
                {
                    MessageDialog msgDialog = new MessageDialog("Crashed");
                    await msgDialog.ShowAsync();
                }
            }
            else if (args.Kind == ActivationKind.Protocol)
            {
                MessageDialog dialog = new MessageDialog("Hit Protocol");
                await dialog.ShowAsync();
                var commandArgs = args as ProtocolActivatedEventArgs;
                Windows.Foundation.WwwFormUrlDecoder decoder = new Windows.Foundation.WwwFormUrlDecoder(commandArgs.Uri.Query);
                var destination = decoder.GetFirstValueByName("LaunchContext");

            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(EnterByVoice), args.PreviousExecutionState);
            }
            // Ensure the current window is active
            Window.Current.Activate();

        }
    
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        private string SemanticInterpretation(string interpretationKey, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[interpretationKey].FirstOrDefault();
        }

    }
}
