using Microsoft.EntityFrameworkCore.Storage;
using PulseFlow.Interfaces;
using PulseFlow.Models;
using System.Windows.Media;
using Wpf.Ui.Abstractions.Controls;

namespace PulseFlow.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {


        #region FIELDS
        private bool _isInitialized = false;


        private readonly IDatabase<SensorLog> database;





        #endregion


        #region PROPERTIES

        [ObservableProperty]
        private IEnumerable<SensorLog?>? sensorLogs;

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }




        #endregion




        #region CONSTRUCTORS
        public DataViewModel(IDatabase<SensorLog?>? database)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }

        #endregion

        #region COMMANDS

        #endregion

        #region METHODS

        #endregion

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModelAsync();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task InitializeViewModelAsync()
        {
            // indicate loading started
            IsLoading = true;

            this.sensorLogs = await Task.Run(() => this.database?.Get());

            // loading finished
            IsLoading = false;

            _isInitialized = true;      


        }
    }
}
