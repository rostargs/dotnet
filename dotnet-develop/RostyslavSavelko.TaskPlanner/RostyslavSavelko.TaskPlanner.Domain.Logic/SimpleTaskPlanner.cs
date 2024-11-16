using System;
using System.Linq;
using RostyslavSavelko.TaskPlanner.Domain.Models;

namespace RostyslavSavelko.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        public WorkItem[] CreatePlan(WorkItem[] items)
        {
            return items
                .OrderByDescending(item => item.Priority)
                .ThenByDescending(item => item.DueTime.Date)
                .ThenBy(item => item.Title)
                .ToArray();
        }
    }
}
