using System;
using System.Collections.Generic;
using RostyslavSavelko.TaskPlanner.Domain.Models.Enums;
using RostyslavSavelko.TaskPlanner.Domain.Models;
using RostyslavSavelko.TaskPlanner.Domain.Logic;
using RostyslavSavelko.TaskPlanner.DataAccess;

internal static class Program
{
    private static FileWorkItemsRepository _workItemsRepository;
    private static Dictionary<char, Action> _actions;
    private const string Menu = "[A]dd work item\n[B]uild a plan\n" +
                                "[M]ark work item as completed\n[R]emove a work item\n[E]xit";

    private static void Main()
    {
        _workItemsRepository = new FileWorkItemsRepository();
        InitializeActions();
        DisplayMenu();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (_actions.TryGetValue(char.ToLower(key.KeyChar), out var action))
            {
                action();
                Console.ReadKey(intercept: true);
            }

            Console.Clear();
            DisplayMenu();
        }
    }

    private static void InitializeActions()
    {
        _actions = new Dictionary<char, Action>
        {
            { 'a', AddWorkItem },
            { 'b', BuildPlan },
            { 'm', MarkAsCompleted },
            { 'r', RemoveWorkItem },
            { 'e', ExitApplication }
        };
    }

    private static void DisplayMenu()
    {
        Console.WriteLine(Menu);
    }

    private static void AddWorkItem()
    {
        var workItem = new WorkItem
        {
            Title = Prompt("Enter the title: "),
            DueTime = ParseDate("Enter the due date (dd-MM-yyyy): "),
            Priority = ParseEnum<Priority>("Enter the priority (None = 0, Low = 1, Medium = 2, High = 3, Urgent = 4): ")
        };

        _workItemsRepository.Add(workItem);
        _workItemsRepository.SaveChanges();
        Console.WriteLine($"Added: {workItem}");
    }

    private static void BuildPlan()
    {
        _workItemsRepository.SaveChanges();
        var planner = new SimpleTaskPlanner();
        var plan = planner.CreatePlan(_workItemsRepository.GetAll());

        foreach (var task in plan)
        {
            Console.WriteLine(task);
        }
    }

    private static void MarkAsCompleted()
    {
        var id = ParseGuid("Enter task ID: ");
        try
        {
            _workItemsRepository.Get(id).IsCompleted = true;
            Console.WriteLine("Marked as done!");
        }
        catch
        {
            Console.WriteLine("Task not found or an error occurred.");
        }
    }

    private static void RemoveWorkItem()
    {
        var id = ParseGuid("Enter task ID: ");
        try
        {
            _workItemsRepository.Remove(id);
            Console.WriteLine("Successfully removed.");
        }
        catch
        {
            Console.WriteLine("Task not found or an error occurred.");
        }
    }

    private static void ExitApplication()
    {
        _workItemsRepository.SaveChanges();
        Environment.Exit(0);
    }

    private static string Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    private static DateTime ParseDate(string prompt)
    {
        while (true)
        {
            var input = Prompt(prompt);
            if (DateTime.TryParseExact(input, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }
            Console.WriteLine("Invalid date format. Please try again.");
        }
    }

    private static TEnum ParseEnum<TEnum>(string prompt) where TEnum : struct, Enum
    {
        while (true)
        {
            var input = Prompt(prompt);
            if (Enum.TryParse(input, out TEnum value) && Enum.IsDefined(typeof(TEnum), value))
            {
                return value;
            }
            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    private static Guid ParseGuid(string prompt)
    {
        while (true)
        {
            var input = Prompt(prompt);
            if (Guid.TryParse(input, out var guid))
            {
                return guid;
            }
            Console.WriteLine("Invalid GUID. Please try again.");
        }
    }
}
