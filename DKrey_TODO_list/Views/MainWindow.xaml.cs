using DKrey_TODO_list.Models;
using DKrey_TODO_list.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace DKrey_TODO_list
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void TaskItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement element && element.DataContext is Task task)
            {
                var viewModel = DataContext as MainWindowViewModel;
                if (viewModel != null)
                {
                    viewModel.SelectedTask = task;
                    viewModel.EditTask();
                }
            }
        }
    }



}
