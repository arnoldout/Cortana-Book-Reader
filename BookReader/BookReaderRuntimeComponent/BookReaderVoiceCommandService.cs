﻿using System;
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
                                    userMessage = new VoiceCommandUserMessage();
                                    userMessage.DisplayMessage = "Deleting book";
                                    userMessage.SpokenMessage = "Deleting Book";
                                    progressReport = VoiceCommandResponse.CreateResponse(userMessage);
                                    await voiceServiceConnection.ReportProgressAsync(progressReport);
                                    await b.DeleteAsync();
                                    
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
                        // As a last resort, launch the app in the foreground.
                        default:
                            LaunchAppInForeground();
                            break;
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

        private void VoiceCommandCompleted(VoiceCommandServiceConnection sender,VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                // Insert your code here.
                // Complete the service deferral.
                this.serviceDeferral.Complete();
            }
        }

        private async void SendCompletionMessageForDestination(string destination)
        {
            // Take action and determine when the next trip to destination
            // Insert code here.

            // Replace the hardcoded strings used here with strings 
            // appropriate for your application.

            // First, create the VoiceCommandUserMessage with the strings 
            // that Cortana will show and speak.
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Here’s your trip.";
            userMessage.SpokenMessage = "Your trip to Vegas is on August 3rd.";

            // Optionally, present visual information about the answer.
            // For this example, create a VoiceCommandContentTile with an 
            // icon and a string.
            var destinationsContentTiles = new List<VoiceCommandContentTile>();

            var destinationTile = new VoiceCommandContentTile();
            destinationTile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
            // The user can tap on the visual content to launch the app. 
            // Pass in a launch argument to enable the app to deep link to a 
            // page relevant to the item displayed on the content tile.
            destinationTile.AppLaunchArgument = string.Format("destination={0}", "Las Vegas");
            destinationTile.Title = "Las Vegas";
            destinationTile.TextLine1 = "August 3rd 2015";
            destinationsContentTiles.Add(destinationTile);

            // Create the VoiceCommandResponse from the userMessage and list    
            // of content tiles.
            var response = VoiceCommandResponse.CreateResponse(userMessage, destinationsContentTiles);

            // Cortana will present a "Go to app_name" link that the user 
            // can tap to launch the app. 
            // Pass in a launch to enable the app to deep link to a page 
            // relevant to the voice command.
            response.AppLaunchArgument = string.Format("destination={0}", "Las Vegas");

            // Ask Cortana to display the user message and content tile and 
            // also speak the user message.
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Launching Adventure Works";

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            // When launching the app in the foreground, pass an app 
            // specific launch parameter to indicate what page to show.
            response.AppLaunchArgument = "showAllTrips=true";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }
    }
}