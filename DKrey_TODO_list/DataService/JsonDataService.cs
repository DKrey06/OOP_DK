using DKrey_TODO_list.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;


namespace DKrey_TODO_list.DataService
{
    public class JsonDataService
    {
        private readonly string _filePath;

        public JsonDataService()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");
        }

        public List<Task> LoadTasks()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Task>();

                var json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
            }
            catch (Exception)
            {
                return new List<Task>();
            }
        }

        public void SaveTasks(List<Task> tasks)
        {
            try
            {
                var json = JsonConvert.SerializeObject(tasks, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        public int GetNextId(List<Task> tasks)
        {
            return tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;
        }
    }
}