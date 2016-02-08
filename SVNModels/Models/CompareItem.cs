using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVNModels
{
    public enum CompareItemStatus { Base = 0, Unknown = 1, Identical = 2, Different = 3 }
    public enum CompareItemSVNUpdateStatus { None = 0, Success = 1, Error = 2 }

    public enum CompareFileStatus { Unknown = 0, Identical = 1, Different = 2, Unique = 3, BaseUnique = 4 }

    public class CompareFileResult
    {
        public CompareFileStatus Status { get; internal set; }
        public string FileLeft { get; internal set; }
        public string FileRight { get; internal set; }
    }

    public class CompareResultItem
    {
        public CompareItem Source { get; internal set; }
        public CompareItem Target { get; internal set; }

        public ObservableCollection<CompareFileResult> FileResults { get; internal set; }

        public int TotalFiles { get; internal set; }
        public int IdenticalFiles { get; internal set; }
        public int DifferentFiles { get; internal set; }
        public int UniqueFiles { get; internal set; }
        public int BaseUniqueFiles { get; internal set; }

        public CompareResultItem()
        {
            FileResults = new ObservableCollection<CompareFileResult>();
        }

        public void Clear()
        {
            Source = null;
            Target = null;

            TotalFiles = IdenticalFiles = DifferentFiles = UniqueFiles = BaseUniqueFiles = 0;
            FileResults.Clear();
        }
    }


    public class CompareItem : _BaseModel
    {
        // Podaci za compare
        public string Name { get; internal set; }
        public string Path { get; internal set; }
        public bool Default { get; internal set; }

        public CompareGroup Group { get; internal set; }

        private CompareItemStatus _Status = CompareItemStatus.Unknown;
        public CompareItemStatus Status 
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        public CompareResultItem CompareResult;

        public CompareItem()
        {
            CompareResult = new CompareResultItem();
        }
    }
}
