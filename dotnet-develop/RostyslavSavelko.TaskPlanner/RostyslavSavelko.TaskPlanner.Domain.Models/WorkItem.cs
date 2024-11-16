using System;
using RostyslavSavelko.TaskPlanner.Domain.Models.Enums;

namespace RostyslavSavelko.TaskPlanner.Domain.Models
{
    public class WorkItem : ICloneable
    {
        public DateTime DueTime { get; set; }
        public DateTime CreationDate { get; set; }
        public Priority Priority { get; set; }
        public Complexity Complexity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public Guid Id { get; set; }

        public object Clone()
        {
            return new WorkItem
            {
                Title = (string)Title?.Clone(),
                DueTime = DueTime,
                CreationDate = CreationDate,
                Priority = Priority,
                Complexity = Complexity,
                Description = Description,
                IsCompleted = IsCompleted,
                Id = Id
            };
        }

        public override string ToString()
        {
            return $"{Title}: Due - {DueTime:dd.MM.yyyy}; Priority - {Priority}\n{Id}";
        }
    }
}
