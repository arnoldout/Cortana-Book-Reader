using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using Windows.UI.Popups;

namespace BookReader.Runtime
{
    public sealed class BookReaderVoiceCommandService : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        VoiceCommandServiceConnection voiceServiceConnection;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            //Take a service deferral so the service isn&#39;t terminated.
            this.serviceDeferral = taskInstance.GetDeferral();

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
           
            if (triggerDetails != null && triggerDetails.Name == "BookReaderVoiceCommandService")
            {
                try
                {
                    voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted += VoiceCommandCompleted;

                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();
                    switch (voiceCommand.CommandName)
                    {
                        case "deleteBook":
                            {
                                var userMessage = new VoiceCommandUserMessage();
                                userMessage.DisplayMessage = "Deleting book";
                                userMessage.SpokenMessage = "Deleting Book";
                                var progressReport = VoiceCommandResponse.CreateResponse(userMessage);
                                await voiceServiceConnection.ReportProgressAsync(progressReport);
                                var book = voiceCommand.Properties["bookName"][0];
                                userMessage = new VoiceCommandUserMessage();
                                userMessage.DisplayMessage = book + "deleted";
                                userMessage.SpokenMessage = "File deleted";
                                try
                                {
                                    var folder = ApplicationData.Current.LocalFolder;
                                    var subFolder = await folder.GetFolderAsync("books");
                                    var a = await folder.GetFolderAsync(book + ".txt");
                                    var b = await subFolder.GetFileAsync(book + ".txt");
                                    await a.DeleteAsync();
                                    await b.DeleteAsync();
                                    updateBookPhrases();
                                    await voiceServiceConnection.ReportSuccessAsync(VoiceCommandResponse.CreateResponse(userMessage));
                                }
                                catch(Exception)
                                {
                                    userMessage.DisplayMessage = "Something went wrong, "+book+" not deleted";
                                    userMessage.SpokenMessage = "Something went wrong, File not deleted";
                                    await voiceServiceConnection.ReportFailureAsync(VoiceCommandResponse.CreateResponse(userMessage));
                                }
                                break;
                            }
                    }
                }
                finally
                {
                    if (this.serviceDeferral != null)
                    {
                        // Complete the service deferral.
                        this.serviceDeferral.Complete();
                    }
                }
            }
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
        private void VoiceCommandCompleted(VoiceCommandServiceConnection sender,VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                // Insert your code here.
                // Complete the service deferral.
                this.serviceDeferral.Complete();
            }
        }
    }
}