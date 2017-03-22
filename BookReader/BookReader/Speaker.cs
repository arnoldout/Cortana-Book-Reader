using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BookReader
{
    class Speaker
    {
        //Code Developed from https://blogs.windows.com/buildingapps/2016/05/23/using-speech-in-your-uwp-apps-from-talking-to-conversing/
        async Task<IRandomAccessStream> SynthesizeTextToSpeechAsync(string text)
        {
            // Windows.Storage.Streams.IRandomAccessStream
            IRandomAccessStream stream = null;

            // Windows.Media.SpeechSynthesis.SpeechSynthesizer
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                // Windows.Media.SpeechSynthesis.SpeechSynthesisStream
                stream = await synthesizer.SynthesizeTextToStreamAsync(text);
            }

            return (stream);
        }
        public async Task SpeakTextAsync(string text, MediaElement media)
        {
            IRandomAccessStream stream = await this.SynthesizeTextToSpeechAsync(text);
            media.SetSource(stream, string.Empty);
            media.Play();
            //await media.PlayStreamAsync(stream, true);
        }
        {
            var synthesisStream = await new SpeechSynthesizer().SynthesizeTextToStreamAsync(text);
            var sf = await lf.CreateFileAsync(fileID+".wav");
            var writeStream = await sf.OpenAsync(FileAccessMode.ReadWrite);
            var outputStream = writeStream.GetOutputStreamAt(0);
            const int BufferSize = 4096;
            var dataWriter = new DataWriter(outputStream);
            var buffer = new Windows.Storage.Streams.Buffer(BufferSize);
            while (synthesisStream.Position < synthesisStream.Size)
            {
                await synthesisStream.ReadAsync(buffer, BufferSize, InputStreamOptions.None);
                dataWriter.WriteBuffer(buffer);
            }
            dataWriter.StoreAsync().AsTask().Wait();
            outputStream.FlushAsync().AsTask().Wait();
            outputStream.Dispose();
            writeStream.Dispose();
        
        }
    }
    public static class MediaElementExtensions
    {
        public static async Task PlayStreamAsync(this MediaElement mediaElement,IRandomAccessStream stream,bool disposeStream = true)
        {
            // bool is irrelevant here, just using this to flag task completion.
            TaskCompletionSource<bool> taskCompleted = new TaskCompletionSource<bool>();

            // Note that the MediaElement needs to be in the UI tree for events
            // like MediaEnded to fire.
            RoutedEventHandler endOfPlayHandler = (s, e) =>
            {
                if (disposeStream)
                {
                    stream.Dispose();
                }
                taskCompleted.SetResult(true);
            };
            mediaElement.MediaEnded += endOfPlayHandler;

            mediaElement.SetSource(stream, string.Empty);
            mediaElement.Play();

            await taskCompleted.Task;
            mediaElement.MediaEnded -= endOfPlayHandler;
        }
    }
}
