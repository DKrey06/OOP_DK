using DKrey_TODO_list.Models;
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
        private string _filterText;
        private TaskState _filterState = TaskState.NotStarted;
        private TaskCategory _filterCategory;
        private TaskImportance _filterImportance;

        public MainWindowViewModel()
        {
            Tasks = new ObservableCollection<Task>();
            LoadTasks();

            
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

        public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
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

        // Команды
        public RelayCommand AddTaskCommand { get; }
        public RelayCommand EditTaskCommand { get; }
        public RelayCommand DeleteTaskCommand { get; }
        public RelayCommand CompleteTaskCommand { get; }
        public RelayCommand FilterTasksCommand { get; }

        public void LoadTasks()
        {
        
            Tasks.Clear();
            Tasks.Add(new Task
            {
                Id = 1,
                Title = "Пример задачи",
                Description = "Описание примерной задачи",
                DueDate = DateTime.Now.AddDays(7),
                StartDate = DateTime.Now,
                IsComplete = false,
                TaskState = TaskState.NotStarted,
                TaskCategory = TaskCategory.Work,
                TaskImportance = TaskImportance.Middle
            });
        }

        public void AddTask()
        {
           
            MessageBox.Show("Функция добавления задачи будет реализована после создания окна TaskAddWindow");
        }

        public void EditTask()
        {
            if (SelectedTask != null)
            {
               
                MessageBox.Show($"Редактирование задачи: {SelectedTask.Title}");
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
                    Tasks.Remove(SelectedTask);
                    SelectedTask = null;
                }
            }
        }

        public void CompleteTask()
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsComplete = true;
                SelectedTask.TaskState = TaskState.Complite;
                OnPropertyChanged(nameof(SelectedTask));


                var index = Tasks.IndexOf(SelectedTask);
                if (index >= 0)
                {
                    Tasks[index] = SelectedTask;
                }
            }
        }

        public void FilterTasks()
        {

            LoadTasks();
        }

        public void MarkTaskAsComplete(Task task)
        {
            if (task != null)
            {
                task.IsComplete = true;
                task.TaskState = TaskState.Complite;

                var index = Tasks.IndexOf(task);
                if (index >= 0)
                {
                    Tasks[index] = task;
                }
            }
        }

        public void DeleteTask(Task task)
        {
            if (task != null)
            {
                Tasks.Remove(task);
                if (SelectedTask == task)
                {
                    SelectedTask = null;
                }
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