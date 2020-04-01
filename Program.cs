using System;
using System.Collections.Generic;

using System.Linq;
using System.IO;
using System.Xml.Serialization;


namespace ToDoTaskManager
{
    public class Task
    {
        string name;
        string description;
        int priority;
        int hour;
        int minute;
        bool NameWasCorrect = false;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }
        public int Hour
        {
            get
            {
                return hour;
            }
            set
            {
                if (value >= 0 && value < 24)
                    hour = value;
                else hour = 0;
            }
        }
        public int Minute
        {
            get
            {
                return minute;
            }
            set
            {
                if(value >= 0 && value < 60)
                    minute = value;
            }
        }
      
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    name = value;
                    NameWasCorrect = true;
                }
                else
                {
                    Console.WriteLine("Ім'я введено не правильне");
                }
            }
        }
        public Task()
        {
            if(Program.isLoading == false) Editting();
        }
        public void Editting()
        {
            Console.Clear();
            Program.Header();

            NameWasCorrect = false;
            while (NameWasCorrect == false)
            {
                Console.WriteLine("Введіть ім'я Таска:");
                Name = Console.ReadLine();
            }
            Console.WriteLine("Введіть опис вашого таска");
            Description = Console.ReadLine();

            Console.WriteLine("Введіть дедлайн");
            Console.WriteLine("Година:");
            Hour = int.Parse(Console.ReadLine());
            Console.WriteLine("Хвилина:");
            Minute = int.Parse(Console.ReadLine());

            while (true)
            {
                Console.WriteLine("Важливість завдання(0,1,2):");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.D1) { Priority = 0; break; }
                if (key.Key == ConsoleKey.D2) { Priority = 1; break; }
                if (key.Key == ConsoleKey.D3) { Priority = 2; break; }
            }
            Console.Clear();
            Program.Header();
        }
        public void GetRemind()
        {
            Console.WriteLine($"Завдання: {Name}, лишилось {Math.Abs(Hour - DateTime.Now.Hour)} годин та {Math.Abs(Minute - DateTime.Now.Minute)}");
        }
        public void GetTaskInfo(int index)
        {
            Console.WriteLine($"\n{index}. {Name}");
            Console.WriteLine($"Опис: {Description}");
            Console.WriteLine($"Пріоритет: {Priority}");
            Console.WriteLine($"Дедлайн: {Hour}:{Minute}");
        }
    }
    class TaskManager
    {
        List<Task> tasks = new List<Task>();
        public void CreateTask()
        {
            tasks.Add(new Task());
        }
        public void EditTask()
        {
            Console.Clear();
            Console.WriteLine("------------------------------------------------");
            Program.Header();

            WatchAllTasks();

            Console.WriteLine("Введіть назву таску для його редагування:");
            string name = Console.ReadLine();
            var collection = tasks.Where(task => task.Name.Contains(name));
            Task tempTask = null;
            bool isdeletting = false;
            foreach (var task in collection)
            {
                Console.WriteLine("Оберіть тип редагування редагування(Видалити=<D>, Змінити=<E>):");
                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.D) { tempTask = task; isdeletting = true; break; }
                    if (key.Key == ConsoleKey.E) { task.Editting(); break; }
                }
                
            }
            if (isdeletting)
            {
                DeleteTask(tempTask);
            }
            
        }
        public void DeleteTask(Task _task)
        {
            foreach(Task task in tasks.ToArray())
            {
                if(_task != null)
                    if (task.Equals(_task)) tasks.Remove(task);
            }
            Console.WriteLine("Елемент успішно видалено!");
        }

        public void WatchAllTasks()
        {
            Console.Clear();
            Program.Header();

            Console.WriteLine("\t\tВаші завдання:");
            var taskColl = tasks.OrderBy(x => x.Priority);
            int i = 1;
            foreach(var task in taskColl)
            {
                if (task == null) continue;

                task.GetTaskInfo(i);
                i++;
            }
        }

        public void Remind()
        {
            foreach(Task task in tasks)
            {
                task.GetRemind();
            }
        }

        public void Save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Task>));
            string path = "save.txt";
            FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            ser.Serialize(file, tasks);
            file.Close();
        }

        public void Load()
        {
            Program.isLoading = true;
            XmlSerializer ser = new XmlSerializer(typeof(List<Task>));
            string path = "save.txt";
            FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            if (file.Length == 0)
            {
                file.Close();
                return;
            }
            tasks = (List<Task>)ser.Deserialize(file);
            
            file.Close();
        }
    }

    class Program
    {
        public static bool isLoading = false;
        public static void Header()
        {
            Console.WriteLine("\t\tTask Manager v0.1");
            Console.WriteLine("Маніпуляції:\n A - додати завдання\n B - редагувати завдання\n C - подивитись всі завдання\n D - зберегти зміни\n Esc - вихід");
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;

            TaskManager taskManager = new TaskManager();

            taskManager.Load();
            Program.isLoading = false;

            Program.Header();
            taskManager.Remind();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.A) taskManager.CreateTask();
                if (key.Key == ConsoleKey.B) taskManager.EditTask();
                if (key.Key == ConsoleKey.C) taskManager.WatchAllTasks();
                if (key.Key == ConsoleKey.D) taskManager.Save();
                if (key.Key == ConsoleKey.Escape) return;
            }
        }
    }
}
