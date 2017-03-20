using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReader
{
    class BookSegments
    {
        private String segment;
        public BookSegments(String s)
        {
            segment = s;
        }

        public string Segment
        {
            get
            {
                return segment;
            }

            set
            {
                segment = value;
            }
        }
    }
}
