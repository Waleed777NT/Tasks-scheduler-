using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TaskSchedulerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskManager manager = new TaskManager();
            manager.LoadTasks();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Smart Task Scheduler ===");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. View Tasks");
                Console.WriteLine("3. Remove Task");
                Console.WriteLine("4. Upcoming Tasks");
                Console.WriteLine("5. Save & Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        manager.AddTask();
                        break;
                    case "2":
                        manager.DisplayTasks();
                        break;
                    case "3":
                        manager.RemoveTask();
                        break;
                    case "4":
                        manager.ShowUpcomingTasks();
                        break;
                    case "5":
                        manager.SaveTasks();
                        return;
                    default:
                        Console.WriteLine("Invalid choice, press Enter to try again...");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }

    [Serializable]
    public class TaskItem
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }

        public override string ToString()
        {
            return $"{Title} - Due: {DueDate:dd/MM/yyyy HH:mm}";
        }
    }

    public class TaskManager
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private readonly string filePath = "tasks.json";

        public void AddTask()
        {
            Console.Write("Enter task title: ");
            string title = Console.ReadLine();

            Console.Write("Enter due date (yyyy-MM-dd HH:mm): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
            {
                tasks.Add(new TaskItem { Title = title, DueDate = dueDate });
                Console.WriteLine("Task added successfully!");
            }
            else
            {
                Console.WriteLine("Invalid date format.");
            }
            Pause();
        }

        public void DisplayTasks()
        {
            Console.WriteLine("\n--- All Tasks ---");
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available.");
            }
            else
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {tasks[i]}");
                }
            }
            Pause();
        }

        public void RemoveTask()
        {
            DisplayTasks();
            Console.Write("\nEnter task number to remove: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tasks.Count)
            {
                tasks.RemoveAt(index - 1);
                Console.WriteLine("Task removed successfully!");
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
            Pause();
        }

        public void ShowUpcomingTasks()
        {
            Console.WriteLine("\n--- Upcoming Tasks (Next 3 days) ---");
            DateTime now = DateTime.Now;
            DateTime upcoming = now.AddDays(3);

            var upcomingTasks = tasks.FindAll(t => t.DueDate >= now && t.DueDate <= upcoming);

            if (upcomingTasks.Count == 0)
            {
                Console.WriteLine("No upcoming tasks.");
            }
            else
            {
                foreach (var task in upcomingTasks)
                {
                    Console.WriteLine(task);
                }
            }
            Pause();
        }

        public void SaveTasks()
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            Console.WriteLine("Tasks saved. Goodbye!");
        }

        public void LoadTasks()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
        }

        private void Pause()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}
