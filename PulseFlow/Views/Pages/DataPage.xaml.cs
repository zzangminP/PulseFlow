using System.Windows;
using PulseFlow.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace PulseFlow.Views.Pages
{
    public partial class DataPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }

        public DataPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            DataContext = this;

            // set initial visibility according to view model state
            this.menuGridLoadingControl.Visibility = ViewModel.IsLoading ? Visibility.Visible : Visibility.Collapsed;
            this.menuGrid.Visibility = ViewModel.IsLoading ? Visibility.Collapsed : Visibility.Visible;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsLoading":
                    if (ViewModel.IsLoading)
                    {
                        this.menuGridLoadingControl.Visibility = Visibility.Visible;
                        this.menuGrid.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.menuGridLoadingControl.Visibility = Visibility.Collapsed;
                        this.menuGrid.Visibility = Visibility.Visible;
                    }
                    break;
            }
            
        }
    }
}
