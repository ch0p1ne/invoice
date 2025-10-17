﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace invoice.Utilities
{
    public class MenuItem : VMBase
    {
        private string _name;
        public string Name 
        { 
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string? _icon;
        public string? Icon 
        { 
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        }

        private ObservableCollection<MenuItem>? _childrens;
        public ObservableCollection<MenuItem>? Childrens 
        { 
            get => _childrens; 
            set => _childrens = value;
        }

        public MenuItem(string name, ObservableCollection<MenuItem>? childrens =  null, VMBase? viewModel = null, string icon = "")
        {
            _name = name;
            _childrens = childrens;
            _viewModel = viewModel;
            _icon = icon;

        }

        private VMBase? _viewModel;
        public VMBase? ViewModel 
        { 
            get => _viewModel; 
            set => _viewModel = value;
        }

        public MenuItem()
        {
            _name = string.Empty;
            _childrens = null;
            _viewModel = null;
        }
    }
}
