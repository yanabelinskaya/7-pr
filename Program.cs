using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

public static class FileExplorer
{
    public static void DisplayDrives()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();

        Console.WriteLine("Доступные диски:");
        for (int i = 0; i < drives.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {drives[i].Name} - {drives[i].DriveType}");
        }
    }

    public static string SelectDrive()
    {
        Console.CursorVisible = false;
        int selectedDriveIndex = 0;
        DriveInfo[] drives = DriveInfo.GetDrives();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Доступные диски:");

            for (int i = 0; i < drives.Length; i++)
            {
                if (i == selectedDriveIndex)
                {
                    Console.Write("-> ");
                }
                else
                {
                    Console.Write("   ");
                }
                Console.WriteLine($"{drives[i].Name} - {drives[i].DriveType}");
            }

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.Enter:
                    Console.CursorVisible = true;
                    return drives[selectedDriveIndex].Name;
                case ConsoleKey.UpArrow:
                    if (selectedDriveIndex > 0)
                    {
                        selectedDriveIndex--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedDriveIndex < drives.Length - 1)
                    {
                        selectedDriveIndex++;
                    }
                    break;
                case ConsoleKey.Escape:
                    Console.CursorVisible = true;
                    return null; // Пользователь вышел из выбора диска
            }
        }
    }


    public static void DisplayDriveInfo(string driveName)
    {
        DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name == driveName);
        if (drive != null)
        {
            Console.WriteLine($"Информация о диске {driveName}:");
            Console.WriteLine($"Объем диска: {drive.TotalSize / (1024 * 1024 * 1024)} ГБ");
            Console.WriteLine($"Свободное место: {drive.AvailableFreeSpace / (1024 * 1024 * 1024)} ГБ");
        }
        else
        {
            Console.WriteLine("Диск не найден.");
        }
    }

    public static void DisplayDirectoryContents(string path)
    {
        Console.WriteLine($"Содержание {path}:");

        string[] directories = Directory.GetDirectories(path);
        Console.WriteLine("Директории:");
        foreach (string directory in directories)
        {
            Console.WriteLine(Path.GetFileName(directory));
        }

        string[] files = Directory.GetFiles(path);
        Console.WriteLine("Файлы:");
        foreach (string file in files)
        {
            Console.WriteLine(Path.GetFileName(file));
        }
    }

    public static bool IsDirectory(string path)
    {
        return Directory.Exists(path);
    }

    public static void RunFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string extension = Path.GetExtension(filePath);
                string programToOpen = null;

                switch (extension)
                {
                    case ".txt":
                        programToOpen = "notepad.exe";
                        break;
                    case ".docx":
                        programToOpen = "winword.exe";
                        break;
                    case ".pdf":
                        programToOpen = "AcroRd32.exe";
                        break;
                    default:
                        Console.WriteLine("Нет программы для открытия данного файла.");
                        break;
                }

                if (programToOpen != null)
                {
                    Process.Start(programToOpen, filePath);
                }
            }
            else
            {
                Console.WriteLine("Файл не найден.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    public static string GetParentDirectory(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            return Directory.GetParent(path)?.FullName;
        }
        return null;
    }

    public static string SelectFolder(string currentPath)
    {
        Console.CursorVisible = false;
        int selectedItemIndex = 0;
        List<string> items = new List<string> { "Вернуться" };

        // Добавляем папки и файлы в список
        items.AddRange(Directory.GetDirectories(currentPath));
        items.AddRange(Directory.GetFiles(currentPath));

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Содержание {currentPath}:");

            for (int i = 0; i < items.Count; i++)
            {
                if (i == selectedItemIndex)
                {
                    Console.Write("-> ");
                }
                else
                {
                    Console.Write("   ");
                }
                Console.WriteLine(items[i]);
            }

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.Enter:
                    if (selectedItemIndex == 0)
                    {
                        Console.CursorVisible = true;
                        return "Вернуться";
                    }
                    else
                    {
                        Console.CursorVisible = true;
                        string selectedItem = items[selectedItemIndex];
                        return selectedItem;
                    }
                case ConsoleKey.UpArrow:
                    if (selectedItemIndex > 0)
                    {
                        selectedItemIndex--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedItemIndex < items.Count - 1)
                    {
                        selectedItemIndex++;
                    }
                    break;
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string selectedDrive = null;
        string currentPath = null;

        while (true)
        {
            Console.Clear();
            FileExplorer.DisplayDrives();
            selectedDrive = FileExplorer.SelectDrive();

            if (selectedDrive == null)
            {
                break;
            }

            Console.Clear();
            FileExplorer.DisplayDriveInfo(selectedDrive);
            currentPath = selectedDrive;

            while (true)
            {
                Console.Clear();
                FileExplorer.DisplayDirectoryContents(currentPath);
                string selectedItem = FileExplorer.SelectFolder(currentPath);

                if (selectedItem == "Вернуться" || selectedItem == null)
                {
                    currentPath = FileExplorer.GetParentDirectory(currentPath);
                }
                else if (FileExplorer.IsDirectory(Path.Combine(currentPath, selectedItem)))
                {
                    currentPath = Path.Combine(currentPath, selectedItem);
                }
                else
                {
                    FileExplorer.RunFile(Path.Combine(currentPath, selectedItem));
                }
            }
        }
    }
}
