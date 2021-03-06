﻿using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Thingy.Db.Entity;

namespace Thingy.Db.Implementation
{
    internal class ToDo : ImplementationBase<ToDoItem>, ITodo
    {
        public ToDo(LiteCollection<ToDoItem> collection) : base(collection)
        {
        }

        public IEnumerable<ToDoItem> GetCompletededTasks()
        {
            return EntityCollection.Find(x => x.IsCompleted == true)
                .OrderBy(x => x.DueDate);

        }

        public IEnumerable<ToDoItem> GetUncompletedTasks()
        {
            return EntityCollection.Find(x => x.IsCompleted == false);
        }

        public void SaveToDoItem(ToDoItem itemtoSave)
        {
            EntityCollection.Insert(itemtoSave);
        }

        public void DeleteToDoItem(ToDoItem toDelete)
        {
            EntityCollection.Delete(item => item.Content == toDelete.Content);
        }

        public void UpdateToDoItem(ToDoItem toUpdate)
        {
            var u = (EntityCollection.Find(item => item.Content == toUpdate.Content)).FirstOrDefault();
            if (u != null)
            {
                u.CompletedDate = toUpdate.CompletedDate;
                u.Content = toUpdate.Content;
                u.DueDate = toUpdate.DueDate;
                u.IsCompleted = toUpdate.IsCompleted;
                EntityCollection.Update(u);
            }
        }

        public void DeleteCompletedToDoItems()
        {
            var q = MessageBox.Show("Delete Completed Items?\nOperation can't be undone.", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (q == MessageBoxResult.Yes)
            {
                EntityCollection.Delete(item => item.IsCompleted == true);
            }
        }
    }
}
