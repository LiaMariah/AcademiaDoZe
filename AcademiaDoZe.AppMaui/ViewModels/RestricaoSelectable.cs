using AcademiaDoZe.Application.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AcademiaDoZe.AppMaui.ViewModels
{
    public class RestricaoSelectable : ObservableObject
    {
        public AppMatriculaRestricoes Value { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
