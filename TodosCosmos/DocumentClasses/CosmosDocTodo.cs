using System;
using TodosShared;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosDocTodo : BaseDocType
    {
        public CosmosDocTodo()
        {
            ID = Guid.NewGuid();
            DocType = (int)DocTypeEnum.Todo;
            GeneratePK();
        }

        public CosmosDocTodo(TSTodo tsTodo)
        {
            UserID = tsTodo.UserID;
            ID = tsTodo.ID;
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


            DocType = (int)DocTypeEnum.Todo;
            GeneratePK();
        }

       


        public Guid UserID { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public bool IsDone { get; set; }

        public bool HasDueDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid CategoryID { get; set; }

        public bool HasRemindDate { get; set; }

        public bool IsReminderEmailed { get; set; }

        public DateTime RemindDate { get; set; }

        public TSTodo toTSTodo()
        {
            

            return new TSTodo()
            {
                UserID = UserID,
                ID = ID,
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
