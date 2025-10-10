using DKrey_TODO_list.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DKrey_TODO_list.Task_ADD_Window
{
    public partial class ADD_Task_Window : Window, INotifyPropertyChanged
    {
        private string _taskTitle;
        private string _taskDescription;
        private DateTime _dueDate;
        private TaskCategory _selectedCategory;
        private TaskImportance _selectedImportance;

        public ADD_Task_Window()
        {
            InitializeComponent();
            DataContext = this;
            WindowTitle = "Добавление задачи";
            DueDate = DateTime.Now.AddDays(1);
            SelectedCategory = TaskCategory.Personal;
            SelectedImportance = TaskImportance.Middle;
        }

        public ADD_Task_Window(Task taskToEdit) : this()
        {
            WindowTitle = "Редактирование задачи";
            TaskTitle = taskToEdit.Title;
            TaskDescription = taskToEdit.Description;
            DueDate = taskToEdit.DueDate;
            SelectedCategory = taskToEdit.TaskCategory;
            SelectedImportance = taskToEdit.TaskImportance;
        }

        public string WindowTitle { get; private set; }
        public Task NewTask { get; private set; }

        public string TaskTitle
        {
            get => _taskTitle;
            set
            {
                _taskTitle = value;
                OnPropertyChanged(nameof(TaskTitle));
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public string TaskDescription
        {
            get => _taskDescription;
            set
            {
                _taskDescription = value;
                OnPropertyChanged(nameof(TaskDescription));
            }
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }

        public TaskCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public TaskImportance SelectedImportance
        {
            get => _selectedImportance;
            set
            {
                _selectedImportance = value;
                OnPropertyChanged(nameof(SelectedImportance));
            }
        }

        public bool CanSave => !string.IsNullOrWhiteSpace(TaskTitle);

        public List<TaskCategory> Categories => Enum.GetValues(typeof(TaskCategory)).Cast<TaskCategory>().ToList();
        public List<TaskImportance> ImportanceLevels => Enum.GetValues(typeof(TaskImportance)).Cast<TaskImportance>().ToList();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CanSave) return;

            NewTask = new Task
            {
                Title = TaskTitle.Trim(),
                Description = TaskDescription?.Trim() ?? "",
                DueDate = DueDate,
                StartDate = DateTime.Now,
                IsComplete = false,
                TaskState = TaskState.NotStarted,
                TaskCategory = SelectedCategory,
                TaskImportance = SelectedImportance
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}