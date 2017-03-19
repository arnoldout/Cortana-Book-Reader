﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BookReader
{
    class TxtParser
    {
        public async Task<string> readFile(StorageFile file)
        {
            if (file.ContentType.Equals("txt"))
            {
                return await FileIO.ReadTextAsync(file);
            }
            return "We don't support Epubs at the moment";
        }
    }
}
