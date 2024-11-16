using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RostyslavSavelko.TaskPlanner.Domain.Models;
using RostyslavSavelko.TaskPlanner.DataAccess.Abstractions;

namespace RostyslavSavelko.TaskPlanner.DataAccess
{
    public class FileWorkItemsRepository : IWorkItemsRepository
    {
        private const string FileName = "work-items.json";
        private readonly Dictionary<Guid, WorkItem> _workItems;

        public FileWorkItemsRepository()
        {
            _workItems = File.Exists(FileName)
                ? LoadWorkItems()
                : new Dictionary<Guid, WorkItem>();
        }

        public Guid Add(WorkItem workItem)
        {
            var clone = (WorkItem)workItem.Clone();
            clone.Id = Guid.NewGuid();
            _workItems[clone.Id] = clone;
            return clone.Id;
        }

        public WorkItem Get(Guid id) =>
            _workItems.TryGetValue(id, out var workItem) ? workItem : null;

        public WorkItem[] GetAll() => _workItems.Values.ToArray();

        public bool Update(WorkItem workItem)
        {
            if (!_workItems.ContainsKey(workItem.Id)) return false;
            _workItems[workItem.Id] = workItem;
            return true;
        }

        public bool Remove(Guid id) => _workItems.Remove(id);

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(_workItems.Values, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }

        private static Dictionary<Guid, WorkItem> LoadWorkItems()
        {
            var fileContent = File.ReadAllText(FileName);
            if (string.IsNullOrEmpty(fileContent)) return new Dictionary<Guid, WorkItem>();

            var items = JsonConvert.DeserializeObject<WorkItem[]>(fileContent) ?? Array.Empty<WorkItem>();
            var workItems = new Dictionary<Guid, WorkItem>();
            foreach (var item in items)
            {
                workItems[item.Id] = item;
            }
            return workItems;
        }
    }
}
