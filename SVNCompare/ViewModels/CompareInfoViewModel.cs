using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVNModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SVNCompare.ViewModels
{
    class CompareInfoViewModel : _BaseViewModel
    {
        #region Properties
        private bool _ShowIdenticalFiles;
        public bool ShowIdenticalFiles
        {
            get
            {
                return _ShowIdenticalFiles;
            }
            set
            {
                if (_ShowIdenticalFiles != value)
                {
                    _ShowIdenticalFiles = value;
                    RaisePropertyChanged("ShowIdenticalFiles");
                    RefreshFileResults(false);
                }
            }
        }

        private bool _ShowDifferentFiles;
        public bool ShowDifferentFiles
        {
            get
            {
                return _ShowDifferentFiles;
            }
            set
            {
                if (_ShowDifferentFiles != value)
                {
                    _ShowDifferentFiles = value;
                    RaisePropertyChanged("ShowDifferentFiles");
                    RefreshFileResults(false);
                }
            }
        }

        private bool _ShowLeftUniqueFiles;
        public bool ShowLeftUniqueFiles
        {
            get
            {
                return _ShowLeftUniqueFiles;
            }
            set
            {
                if (_ShowLeftUniqueFiles != value)
                {
                    _ShowLeftUniqueFiles = value;
                    RaisePropertyChanged("ShowLeftUniqueFiles");
                    RefreshFileResults(false);
                }
            }
        }

        private bool _ShowRightUniqueFiles;
        public bool ShowRightUniqueFiles
        {
            get
            {
                return _ShowRightUniqueFiles;
            }
            set
            {
                if (_ShowRightUniqueFiles != value)
                {
                    _ShowRightUniqueFiles = value;
                    RaisePropertyChanged("ShowRightUniqueFiles");
                    RefreshFileResults(false);
                }
            }
        }

        private CompareGroup _SelectedGroup;
        public CompareGroup SelectedGroup 
        {
            get
            {
                return _SelectedGroup;
            }
            set
            {
                if (_SelectedGroup != value)
                {
                    _SelectedGroup = value;
                    _SelectedItem = null;
                    RaisePropertyChanged("SelectedGroup");
                    RaisePropertyChanged("SelectedItem");
                    RaisePropertyChanged("GetItemCompareInfo");
                    RaisePropertyChanged("GetItemCompareLog");
                }
            }
        }

        private CompareItem _SelectedItem;
        public CompareItem SelectedItem 
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                    RaisePropertyChanged("SelectedItem");
                    RaisePropertyChanged("GetItemCompareInfo");
                    RaisePropertyChanged("GetItemCompareLog");

                    // Sa izborom novog Item-a, refreshamo view za prikaz fajlova
                    RefreshFileResults(true);
                }
            }
        }

        public string GetItemCompareInfo
        {
            get
            {
                if (SelectedGroup != null && SelectedItem != null)
                    return String.Format("{0} - comparing {1} with {2}", SelectedGroup.Name, SelectedItem.Name, SelectedGroup.DefaultItem.Name);
                else
                    return "";
            }
        }
        public ObservableCollection<CompareFileResult> GetItemCompareLog
        {
            get
            {
                Console.WriteLine("MIHO");

                if (SelectedItem != null)
                {
                    return SelectedItem.CompareResult.FileResults;
                }
                else
                    return null;
            }
        }

        public ICollectionView GetFileCompareResults { get; private set; }
        #endregion


        public CompareInfoViewModel()
        {
            _ShowIdenticalFiles = false;
            _ShowDifferentFiles = _ShowLeftUniqueFiles = _ShowRightUniqueFiles = true;
        }

        public void RefreshFileResults(bool loadItem)
        {
            // Ako je odabran Item - kreiramo filtrirani view
            if (loadItem || GetFileCompareResults == null)
                GetFileCompareResults = CollectionViewSource.GetDefaultView(SelectedItem.CompareResult.FileResults);

            // Uvijek radimo reload filtera
            GetFileCompareResults.Filter = item =>
            {
                var checkItem = item as CompareFileResult;

                if (checkItem == null)
                    return false;

                bool showFileResult =
                    (ShowIdenticalFiles && checkItem.Status == CompareFileStatus.Identical) ||
                    (ShowDifferentFiles && checkItem.Status == CompareFileStatus.Different) ||
                    (ShowLeftUniqueFiles && checkItem.Status == CompareFileStatus.LeftUnique) ||
                    (ShowRightUniqueFiles && checkItem.Status == CompareFileStatus.RightUnique);

                return showFileResult;
            };

            RaisePropertyChanged("GetFileCompareResults");
        }
    }
}
