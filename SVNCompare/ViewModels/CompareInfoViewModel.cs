using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVNModels;
using System.Collections.ObjectModel;

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
                    RaisePropertyChanged("SelectedGroup");
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
        public ObservableCollection<String> GetItemCompareLog
        {
            get
            {
                if (SelectedItem != null)
                    return SelectedItem.CompareResult.Log;
                else
                    return null;
            }
        }
        #endregion


        public CompareInfoViewModel()
        {
            _ShowIdenticalFiles = false;
            _ShowDifferentFiles = _ShowLeftUniqueFiles = _ShowRightUniqueFiles = true;
        }
    }
}
