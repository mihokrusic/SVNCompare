using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SVNModels;
using System.Collections.ObjectModel;

namespace SVNCompare.ViewModels
{

    class MainViewModel : _BaseViewModel
    {
        #region Properties
        public ICommand btnCompare { get; set; }
        public ICommand btnLoadGroups { get; set; }
        public ICommand btnConfiguration { get; set; }
        public ICommand btnSetItemAsBase { get; set; }

        private bool _UpdateSVN;
        public bool UpdateSVN
        {
            get
            {
                return _UpdateSVN;
            }
            set
            {
                if (_UpdateSVN != value)
                {
                    _UpdateSVN = value;
                    RaisePropertyChanged("UpdateSVN");
                }
            }
        }

        private bool _UseFilters;
        public bool UseFilters
        {
            get
            {
                return _UseFilters;
            }
            set
            {
                if (_UseFilters != value)
                {
                    _UseFilters = value;
                    RaisePropertyChanged("UseFilters");
                }
            }
        }

        private CompareGroups _Groups;
        public CompareGroups Groups
        {
            get 
            {
                return _Groups;
            }
            set 
            {
                if (_Groups != value)
                {
                    _Groups = value;
                    RaisePropertyChanged("Groups");
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
                    RaisePropertyChanged("SelectedItemCompareLog");

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
                    RaisePropertyChanged("SelectedItemCompareLog");

                }
            }
        }

        public ObservableCollection<String> SelectedItemCompareLog 
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


        public MainViewModel()
        {
            LoadGroups(null);

            UpdateSVN = true;
            UseFilters = true;
            btnCompare = new RelayCommand(Compare, x => true);
            btnLoadGroups = new RelayCommand(LoadGroups, x => true);
            btnConfiguration = new RelayCommand(Configuration, x => true);
            btnSetItemAsBase = new RelayCommand(SetItemAsBase, x => true);
        }


        #region Public methods
        public void Compare(object obj)
        {
            _Groups.Compare();
        }

        public void LoadGroups(object obj)
        {
            if (_Groups == null)
                _Groups = new CompareGroups();

            _Groups.Groups.Clear();

            // Pokušavamo otvoriti XML datoteku
            try
            {
                //_Groups.LoadFromXML("CompareGroups.xml");
                _Groups.LoadFromXML(@"E:\WPF\SVNCompare\SVNCompare\CompareGroups.xml");
            }
            catch (FileNotFoundException e)
            {
                // TODO: zasad ne radimo ništa
            }
        }

        public void Configuration(object obj)
        {
            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            Process.Start("CompareGroups.xml");
        }

        public void SetItemAsBase(object obj)
        {
            Console.WriteLine(obj);
        }
        #endregion
    }
}
