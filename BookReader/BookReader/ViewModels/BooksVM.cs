using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BookReader.ViewModels
{
    class BooksVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        ObservableCollection<string> books;
            public BooksVM()
            {
                //display text to user
                asyncCreatePaper();
            }
            public async Task asyncCreatePaper()
            {
                books = new ObservableCollection<string>();
                Books = await getBooks();
            }
        public ObservableCollection<String> Books
        {
            get { return this.books; }
            set { books = value;
                OnPropertyChanged("Books"); }
        }
        public async Task<ObservableCollection<String>> getBooks()
        {
            books = new ObservableCollection<string>();
            var folder = ApplicationData.Current.LocalFolder;
            try
            {
                folder = await folder.GetFolderAsync("books");
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                foreach (StorageFile s in files)
                {
                    books.Add(s.Name);
                }
            }
            catch (FileNotFoundException)
            {
                await folder.CreateFolderAsync("books");
            }
            return books;
        }
        public void AddBook(String book)
        {
            books.Add(book);
            OnPropertyChanged("Books");
        }
    }
}
    