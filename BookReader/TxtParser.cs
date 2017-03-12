using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReader
{
    class TxtParser
    {
        public String readFile(String txtfile)
        {
            TextReader textReader = File.OpenText(txtfile);
            String line = "";
            StringBuilder sb = new StringBuilder();

            while ((line = textReader.ReadLine()) != null)
            {
                sb.Append(line);
            }
            return sb.ToString();
        }
    }
}
