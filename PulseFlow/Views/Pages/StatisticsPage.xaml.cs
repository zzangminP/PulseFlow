using PulseFlow.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace PulseFlow.Views.Pages
{
    /// <summary>
    /// StatisticsPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StatisticsPage : INavigableView<StatisticsViewModel>
    {
        public StatisticsPage(StatisticsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        public StatisticsViewModel ViewModel { get; }
    }
}
