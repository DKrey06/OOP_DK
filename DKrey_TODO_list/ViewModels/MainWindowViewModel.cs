using DKrey_TODO_list.DataService;
using DKrey_TODO_list.Models;
using DKrey_TODO_list.Task_ADD_Window;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DKrey_TODO_list.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Task _selectedTask;
        private ObservableCollection<Task> _tasks;
        private ObservableCollection<Task> _allTasks;
        private ObservableCollection<Task> _activeTasks;
        private ObservableCollection<Task> _completedTasks;
        private string _filterText;
        private TaskState _filterState;
        private TaskCategory _filterCategory;
        private TaskImportance _filterImportance;
        private readonly JsonDataService _dataService;

        public MainWindowViewModel()
        {
            _dataService = new JsonDataService();
            _allTasks = new ObservableCollection<Task>(_dataService.LoadTasks());
            UpdateTaskCollections();

            AddTaskCommand = new RelayCommand(AddTask);
            EditTaskCommand = new RelayCommand(EditTask, CanEditOrDeleteTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask, CanEditOrDeleteTask);
            CompleteTaskCommand = new RelayCommand(CompleteTask, CanEditOrDeleteTask);
            FilterTasksCommand = new RelayCommand(FilterTasks);
        }

        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        public ObservableCollection<Task> AllTasks
        {
            get => _allTasks;
            set
            {
                _allTasks = value;
                OnPropertyChanged(nameof(AllTasks));
                UpdateTaskCollections();
            }
        }

        public ObservableCollection<Task> ActiveTasks
        {
            get => _activeTasks;
            set
            {
                _activeTasks = value;
                OnPropertyChanged(nameof(ActiveTasks));
            }
        }

        public ObservableCollection<Task> CompletedTasks
        {
            get => _completedTasks;
            set
            {
                _completedTasks = value;
                OnPropertyChanged(nameof(CompletedTasks));
            }
        }

        public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged(nameof(FilterText));
                FilterTasks();
            }
        }

        public TaskState FilterState
        {
            get => _filterState;
            set
            {
                _filterState = value;
                OnPropertyChanged(nameof(FilterState));
                FilterTasks();
            }
        }

        public TaskCategory FilterCategory
        {
            get => _filterCategory;
            set
            {
                _filterCategory = value;
                OnPropertyChanged(nameof(FilterCategory));
                FilterTasks();
            }
        }

        public TaskImportance FilterImportance
        {
            get => _filterImportance;
            set
            {
                _filterImportance = value;
                OnPropertyChanged(nameof(FilterImportance));
                FilterTasks();
            }
        }


        public RelayCommand AddTaskCommand { get; }
        public RelayCommand EditTaskCommand { get; }
        public RelayCommand DeleteTaskCommand { get; }
        public RelayCommand CompleteTaskCommand { get; }
        public RelayCommand FilterTasksCommand { get; }

        private void UpdateTaskCollections()
        {
            var active = AllTasks.Where(t => !t.IsComplete).ToList();
            var completed = AllTasks.Where(t => t.IsComplete).ToList();

            ActiveTasks = new ObservableCollection<Task>(active);
            CompletedTasks = new ObservableCollection<Task>(completed);
        }

        public void AddTask()
        {
            var addWindow = new ADD_Task_Window();
            if (addWindow.ShowDialog() == true)
            {
                var newTask = addWindow.NewTask;
                if (newTask != null)
                {
                    newTask.Id = _dataService.GetNextId(AllTasks.ToList());
                    AllTasks.Add(newTask);
                    _dataService.SaveTasks(AllTasks.ToList());
                    UpdateTaskCollections();
                    FilterTasks();
                }
            }
        }

        public void EditTask()
        {
            if (SelectedTask != null)
            {
                var editWindow = new ADD_Task_Window(SelectedTask);
                if (editWindow.ShowDialog() == true)
                {
                    var updatedTask = editWindow.NewTask;
                    if (updatedTask != null)
                    {
                        updatedTask.Id = SelectedTask.Id;
                        updatedTask.IsComplete = SelectedTask.IsComplete; 

                        var index = AllTasks.IndexOf(SelectedTask);
                        if (index >= 0)
                        {
                            AllTasks[index] = updatedTask;
                            _dataService.SaveTasks(AllTasks.ToList());
                            UpdateTaskCollections();
                            FilterTasks();

                            SelectedTask = updatedTask;
                        }
                    }
                }
            }
        }

        public void DeleteTask()
        {
            if (SelectedTask != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить выбранную задачу?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    AllTasks.Remove(SelectedTask);
                    _dataService.SaveTasks(AllTasks.ToList());
                    UpdateTaskCollections();
                    SelectedTask = null;
                    FilterTasks();
                }
            }
        }

        public void CompleteTask()
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsComplete = true;
                SelectedTask.TaskState = TaskState.Завершено;
                _dataService.SaveTasks(AllTasks.ToList());
                UpdateTaskCollections();
                FilterTasks();

                
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public void FilterTasks()
        {
            var filtered = AllTasks.Where(task =>
                (string.IsNullOrEmpty(FilterText) ||
                 task.Title.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 (task.Description != null && task.Description.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                (FilterState == 0 || task.TaskState == FilterState) &&
                (FilterCategory == 0 || task.TaskCategory == FilterCategory) &&
                (FilterImportance == 0 || task.TaskImportance == FilterImportance)
            ).ToList();

            Tasks = new ObservableCollection<Task>(filtered);

            
            var activeFiltered = filtered.Where(t => !t.IsComplete).ToList();
            var completedFiltered = filtered.Where(t => t.IsComplete).ToList();

            ActiveTasks = new ObservableCollection<Task>(activeFiltered);
            CompletedTasks = new ObservableCollection<Task>(completedFiltered);
        }

        private bool CanEditOrDeleteTask()
        {
            return SelectedTask != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}