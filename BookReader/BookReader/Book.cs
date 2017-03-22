using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReader
{
    class Book
    {
        Queue<BookSegments> book = new Queue<BookSegments>();
        public Book(String strBook)
        {
            int segmentSize = strBook.Length / 50;
            //math breaks if segments are too small, due to small file

           /* if (segmentSize > 2)
            {
                try
                {
                    List<String> b = strBook.Select((x, i) => i).Where(i => i % segmentSize == 0).Select(i => String.Concat(strBook.Skip(i).Take(segmentSize))).ToList();

                    foreach (String s in b)
                    {
                        book.Enqueue(new BookSegments(s));
                    }
                }
                catch (Exception)
                {
                    book.Enqueue(new BookSegments(strBook));
                }
            }
            else
            {*/
                book.Enqueue(new BookSegments(strBook));
            //}
        }
        
        public String popSegment()
        {
            if (book.Count!=0)
            {
                BookSegments bs = book.Dequeue();
                //book.Enqueue(bs);
                return bs.Segment;
            }
            return null;
        }
    }
}
