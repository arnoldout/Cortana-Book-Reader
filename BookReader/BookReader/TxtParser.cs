using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BookReader
{
    class TxtParser
    {
        public async Task<string> readFile(StorageFile file)
        {
            String s = file.ContentType;
            if (file.FileType.Equals(".txt"))
            {
                IBuffer buffer = await FileIO.ReadBufferAsync(file);
                DataReader reader = DataReader.FromBuffer(buffer);
                byte[] fileContent = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(fileContent);
                string text = Encoding.UTF8.GetString(fileContent, 0, fileContent.Length);
                text = Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);
                return text;
            }
            return "We don't support Epubs at the moment";
        }
    }
}
