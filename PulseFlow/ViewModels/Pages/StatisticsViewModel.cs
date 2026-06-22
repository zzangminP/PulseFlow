using CommunityToolkit.Mvvm.ComponentModel;
using PulseFlow.Interfaces;
using PulseFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions.Controls;

namespace PulseFlow.ViewModels.Pages
{
    public partial class StatisticsViewModel : ObservableObject, INavigationAware
    {
        #region FIELDS

        private bool _isInitialized = false;
        private readonly IDatabase<SensorLog> database;


        #endregion


        #region PROPERTIES

        [ObservableProperty]
        private bool isLoading;

        // 콤보박스 선택값 (0: 1시간, 1: 24시간, 2: 전체)
        [ObservableProperty]
        private int selectedTimeRangeIndex = 0;


        [ObservableProperty] private double machineA_AvgTemp;
        [ObservableProperty] private double machineA_AvgPress;
        [ObservableProperty] private double machineA_MaxTemp;
        [ObservableProperty] private double machineA_MaxPress;



        [ObservableProperty] private double machineB_AvgTemp;
        [ObservableProperty] private double machineB_AvgPress;
        [ObservableProperty] private double machineB_MaxTemp;
        [ObservableProperty] private double machineB_MaxPress;

        [ObservableProperty] private double machineC_AvgTemp;
        [ObservableProperty] private double machineC_AvgPress;
        [ObservableProperty] private double machineC_MaxTemp;
        [ObservableProperty] private double machineC_MaxPress;

        #endregion


        #region CONSTRUCTORS

        public StatisticsViewModel(IDatabase<SensorLog> database)
        {
            this.database = database;
        }

        #endregion


        #region COMMANDS

        #endregion


        #region METHODS

        // 콤보박스 값이 바뀔 때마다 자동으로 실행
        partial void OnSelectedTimeRangeIndexChanged(int value)
        {
            if (_isInitialized)
            {
                _ = CalculateStatisticsAsync();
            }
        }

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
            {
                _ = InitializeViewModelAsync();
            }
            else
            {

                _ = CalculateStatisticsAsync();
            }

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task InitializeViewModelAsync()
        {
            _isInitialized = true;
            await CalculateStatisticsAsync();
        }


        private async Task CalculateStatisticsAsync()
        {
            IsLoading = true;

            try
            {
                DateTime targetTime = DateTime.UtcNow;
                switch (SelectedTimeRangeIndex)
                {
                    case 0: targetTime = targetTime.AddHours(-1); break;
                    case 1: targetTime = targetTime.AddHours(-24); break;
                    case 2: targetTime = DateTime.MinValue; break; // 전체
                }

                await Task.Run(() =>
                {
                    using var context = new PulseFlowDbContext();

                    // Machine A 통계
                    var statsA = context.SensorLogs
                        .Where(x => x.MachineName == "Machine_A" && x.LoggedAt >= targetTime)
                        .GroupBy(x => 1) // 전체 그룹화
                        .Select(g => new {
                            AvgTemp = g.Average(x => x.Temperature),
                            AvgPress = g.Average(x => x.Pressure),
                            MaxTemp = g.Max(x => x.Temperature),
                            MaxPress = g.Max(x => x.Pressure)
                        }).FirstOrDefault();

                    if (statsA != null)
                    {
                        MachineA_AvgTemp = statsA.AvgTemp; MachineA_AvgPress = statsA.AvgPress;
                        MachineA_MaxTemp = statsA.MaxTemp; MachineA_MaxPress = statsA.MaxPress;
                    }
                    else
                    {
                        MachineA_AvgTemp = 0; MachineA_AvgPress = 0; MachineA_MaxTemp = 0; MachineA_MaxPress = 0;
                    }

                    // Machine B 통계
                    var statsB = context.SensorLogs
                        .Where(x => x.MachineName == "Machine_B" && x.LoggedAt >= targetTime)
                        .GroupBy(x => 1)
                        .Select(g => new {
                            AvgTemp = g.Average(x => x.Temperature),
                            AvgPress = g.Average(x => x.Pressure),
                            MaxTemp = g.Max(x => x.Temperature),
                            MaxPress = g.Max(x => x.Pressure)
                        }).FirstOrDefault();

                    if (statsB != null)
                    {
                        MachineB_AvgTemp = statsB.AvgTemp; MachineB_AvgPress = statsB.AvgPress;
                        MachineB_MaxTemp = statsB.MaxTemp; MachineB_MaxPress = statsB.MaxPress;
                    }
                    else { MachineB_AvgTemp = 0; MachineB_AvgPress = 0; MachineB_MaxTemp = 0; MachineB_MaxPress = 0; }

                    // Machine C 통계
                    var statsC = context.SensorLogs
                        .Where(x => x.MachineName == "Machine_C" && x.LoggedAt >= targetTime)
                        .GroupBy(x => 1)
                        .Select(g => new {
                            AvgTemp = g.Average(x => x.Temperature),
                            AvgPress = g.Average(x => x.Pressure),
                            MaxTemp = g.Max(x => x.Temperature),
                            MaxPress = g.Max(x => x.Pressure)
                        }).FirstOrDefault();

                    if (statsC != null)
                    {
                        MachineC_AvgTemp = statsC.AvgTemp; MachineC_AvgPress = statsC.AvgPress;
                        MachineC_MaxTemp = statsC.MaxTemp; MachineC_MaxPress = statsC.MaxPress;
                    }
                    else { MachineC_AvgTemp = 0; MachineC_AvgPress = 0; MachineC_MaxTemp = 0; MachineC_MaxPress = 0; }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"통계 계산 중 에러 발생: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }


        #endregion
    }
}