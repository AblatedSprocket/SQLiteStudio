using SQLiteStudio.Utilities;
using System.Collections.ObjectModel;

namespace SQLiteStudio.Query
{
    class QueryViewModel : ViewModel
    {
        private ObservableCollection<Document> _queryItems = new ObservableCollection<Document>();
        public ObservableCollection<Document> QueryItems
        {
            get { return _queryItems; }
            set { OnPropertyChanged(ref _queryItems, value); }
        }

        public QueryViewModel()
        {
            QueryItems = new ObservableCollection<Document> { new Document { Name = "QueryDocument", Commands = "SELECT * FROM DATABASE WHERE STUFF IS THINGS", DatabasePath = "Path/to/database" } };
        }
    }
}
