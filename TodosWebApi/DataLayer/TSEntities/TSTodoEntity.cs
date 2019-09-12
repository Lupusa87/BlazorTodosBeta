using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodosShared;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSTodoEntity : TableEntity
    {
        public TSTodoEntity(TSTodo tsTodo)
        {
            PartitionKey = tsTodo.UserID;
            RowKey = tsTodo.TodoID.ToString();
            Name = tsTodo.Name;
            Description = tsTodo.Description;
            HasDueDate = tsTodo.HasDueDate;
            DueDate = tsTodo.DueDate;
            CreateDate = tsTodo.CreateDate;
            Priority = tsTodo.Priority;
            IsDone = tsTodo.IsDone;
            CategoryID = tsTodo.CategoryID;
            HasRemindDate = tsTodo.HasRemindDate;
            RemindDate = tsTodo.RemindDate;
            IsReminderEmailed = tsTodo.IsReminderEmailed;
        }

        public TSTodoEntity() { }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public bool IsDone { get; set; }

        public bool HasDueDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime CreateDate { get; set; }

        public int CategoryID { get; set; }

        public bool HasRemindDate { get; set; }

        public bool IsReminderEmailed { get; set; }

        public DateTime RemindDate { get; set; }

        public TSTodo toTSTodo()
        {
            

            return new TSTodo()
            {
                UserID = PartitionKey,
                TodoID =int.Parse(RowKey),
                Name = Name,
                Description = Description,
                HasDueDate = HasDueDate,
                DueDate = DueDate,
                IsDone = IsDone,
                Priority = Priority,
                CreateDate = CreateDate,
                CategoryID = CategoryID,
                HasRemindDate = HasRemindDate,
                RemindDate = RemindDate,
                IsReminderEmailed = IsReminderEmailed,
            };
        }
    }
}
