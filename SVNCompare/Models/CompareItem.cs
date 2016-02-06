using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVNCompare.Models
{
    public enum CompareItemStatus { Base = 0, Unknown = 1, Identical = 2, Different = 3 }
    public enum CompareItemSVNUpdateStatus { None = 0, Success = 1, Error = 2 }

    public class CompareResultItem
    {
        public CompareItem source { get; internal set; }
        public CompareItem target { get; internal set; }

        public ObservableCollection<String> Log { get; internal set; }
        public int totalFiles { get; internal set; }
        public int identicalFiles { get; internal set; }
        public int differentFiles { get; internal set; }
        public int leftUniqueFiles { get; internal set; }
        public int rightUniqueFiles { get; internal set; }

        public CompareResultItem()
        {
            Log = new ObservableCollection<String>();
        }

        public void Clear()
        {
            source = null;
            target = null;

            totalFiles = identicalFiles = differentFiles = leftUniqueFiles = rightUniqueFiles = 0;
            Log.Clear();
        }
    }


    public class CompareItem : _BaseModel
    {
        // Podaci za compare
        public string Name { get; internal set; }
        public string Path { get; internal set; }
        public bool Default { get; internal set; }

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
