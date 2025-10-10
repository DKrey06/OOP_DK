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
        private string _filterText;
        private TaskState _filterState;
        private TaskCategory _filterCategory;
        private TaskImportance _filterImportance;
        private readonly JsonDataService _dataService;

        public MainWindowViewModel()
        {
            _dataService = new JsonDataService();
            _allTasks = new ObservableCollection<Task>(_dataService.LoadTasks());
            Tasks = new ObservableCollection<Task>(_allTasks);

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

                        var index = AllTasks.IndexOf(SelectedTask);
                        if (index >= 0)
                        {
                            AllTasks[index] = updatedTask;
                            _dataService.SaveTasks(AllTasks.ToList());
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
                SelectedTask.TaskState = TaskState.Complite;
                _dataService.SaveTasks(AllTasks.ToList());
                OnPropertyChanged(nameof(SelectedTask));
                FilterTasks();
            }
        }

        public void FilterTasks()
        {
            var filtered = AllTasks.Where(task =>
                (string.IsNullOrEmpty(FilterText) ||
                 task.Title.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 (task.Description != null && task.Description.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0)) && // Добавлена проверка на null
                (FilterState == 0 || task.TaskState == FilterState) &&
                (FilterCategory == 0 || task.TaskCategory == FilterCategory) &&
                (FilterImportance == 0 || task.TaskImportance == FilterImportance)
            ).ToList();

            Tasks.Clear();
            foreach (var task in filtered)
            {
                Tasks.Add(task);
            }
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