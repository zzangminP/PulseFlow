using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using PulseFlow.Interfaces;
using PulseFlow.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Wpf.Ui.Abstractions.Controls;

namespace PulseFlow.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        #region FIELDS
        private readonly IMachineControlService machineControlService;
        private DispatcherTimer timer;


        public ChartValues<double> TempValuesA { get; } = new();
        public ChartValues<double> TempValuesB { get; } = new();
        public ChartValues<double> TempValuesC { get; } = new();
        public ChartValues<double> PressValuesA { get; } = new();
        public ChartValues<double> PressValuesB { get; } = new();
        public ChartValues<double> PressValuesC { get; } = new();


        #endregion

        #region PROPERTIES

        [ObservableProperty] private string statusTextA = "데이터 대기 중...";
        [ObservableProperty] private string statusTextB = "데이터 대기 중...";
        [ObservableProperty] private string statusTextC = "데이터 대기 중...";

        [ObservableProperty] private bool isMachineA_Running = true;
        [ObservableProperty] private bool isMachineB_Running = true;
        [ObservableProperty] private bool isMachineC_Running = true;

        [ObservableProperty] private bool isMachineA_ChartVisible = true;
        [ObservableProperty] private bool isMachineB_ChartVisible = true;
        [ObservableProperty] private bool isMachineC_ChartVisible = true;
        #endregion

        #region CONSTRUCTORS


        public DashboardViewModel(IMachineControlService machineControlService)
        {
            this.machineControlService = machineControlService;
            this.timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            this.timer.Tick += UpdateDashboardData;
            this.timer.Start();
        }


        #endregion

        #region COMMANDS


        [RelayCommand] private async Task StopMachineAsync(string machineName) => await machineControlService.StopMachineAsync(machineName);
        
        
        [RelayCommand] private async Task StartMachineAsync(string machineName) => await machineControlService.StartMachineAsync(machineName);


        [RelayCommand]
        private async Task ToggleMachineAsync(string machineName)
        {
            bool targetState = machineName switch { "Machine_A" => IsMachineA_Running, "Machine_B" => IsMachineB_Running, "Machine_C" => IsMachineC_Running, _ => false };
            if (targetState) await StartMachineAsync(machineName); else await StopMachineAsync(machineName);
        }



        [RelayCommand]
        private async Task StopAllAsync()
        {
            IsMachineA_Running = IsMachineB_Running = IsMachineC_Running = false;
            await StopMachineAsync("Machine_A");
            await StopMachineAsync("Machine_B");
            await StopMachineAsync("Machine_C");
        }



        [RelayCommand]
        private async Task StartAllAsync()
        {
            IsMachineA_Running = IsMachineB_Running = IsMachineC_Running = true;
            await StartMachineAsync("Machine_A");
            await StartMachineAsync("Machine_B");
            await StartMachineAsync("Machine_C");
        }


        #endregion

        #region METHODS


        private void UpdateDashboardData(object? sender, EventArgs e)
        {
            try
            {
                using var context = new PulseFlowDbContext();
                UpdateMachineData(context, "Machine_A", TempValuesA, PressValuesA, IsMachineA_Running, t => StatusTextA = t);
                UpdateMachineData(context, "Machine_B", TempValuesB, PressValuesB, IsMachineB_Running, t => StatusTextB = t);
                UpdateMachineData(context, "Machine_C", TempValuesC, PressValuesC, IsMachineC_Running, t => StatusTextC = t);
            }
            catch (Exception ex) { StatusTextA = $"DB 에러: {ex.Message}"; }
        }

        private void UpdateMachineData(PulseFlowDbContext context, string name, ChartValues<double> tVals, ChartValues<double> pVals, bool run, Action<string> update)
        {
            if (!run) { 
                
                update("정지됨");
                tVals.Add(double.NaN);
                pVals.Add(double.NaN);

                return; 
            }
            var log = context.SensorLogs.Where(x => x.MachineName == name).OrderByDescending(x => x.LoggedAt).FirstOrDefault();
            if (log != null)
            {
                update($"온도: {log.Temperature:F1}℃ | 압력: {log.Pressure:F2}atm");
                tVals.Add(log.Temperature); pVals.Add(log.Pressure);
                if (tVals.Count > 30) { tVals.RemoveAt(0); pVals.RemoveAt(0); }
            }
        }

        public Task OnNavigatedToAsync() => Task.CompletedTask;
        public Task OnNavigatedFromAsync() => Task.CompletedTask;


        #endregion
    }
}