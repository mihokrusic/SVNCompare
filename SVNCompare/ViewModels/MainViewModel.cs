﻿using System;
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
        #region Properties for View
        public ICommand btnCompare { get; set; }
        public ICommand btnLoadGroups { get; set; }
        public ICommand btnConfiguration { get; set; }
        public ICommand btnSetItemAsBase { get; set; }

        private bool _UpdateSVN;
        public bool UpdateSVN
        {
            get
            {
                return false; // TODO _UpdateSVN;
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
                return false; // TODO _UseFilters;
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
                    CompareInfoVM.SelectedGroup = _SelectedGroup;
                    RaisePropertyChanged("SelectedGroup");

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
                    SelectedGroup = _SelectedItem.Group;
                    CompareInfoVM.SelectedItem = _SelectedItem;
                    RaisePropertyChanged("SelectedItem");
                }
            }
        }
        #endregion


        public CompareInfoViewModel CompareInfoVM { get; private set; }


        public MainViewModel()
        {
            CompareInfoVM = new CompareInfoViewModel();
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
                _Groups.LoadFromXML(@"CompareGroups.xml");
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
            if (obj != null && obj is CompareItem)
            {
                SelectedItem = (obj as CompareItem);
                SelectedItem.Group.SetDefaultItem(SelectedItem);
                SelectedItem.Group.Compare();
            }
        }
        #endregion
    }
}
