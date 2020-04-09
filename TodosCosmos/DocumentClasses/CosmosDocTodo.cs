using System;
using TodosShared;
using static TodosCosmos.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            Reminders = tsTodo.Reminders;
            DocType = (int)DocTypeEnum.Todo;
            GeneratePK();
        }



        [JsonProperty(PropertyName = "q")]
        public Guid UserID { get; set; }

        [JsonProperty(PropertyName = "w")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "e")]
        public string Description { get; set; }


        [JsonProperty(PropertyName = "r")]
        public int Priority { get; set; }

        [JsonProperty(PropertyName = "t")]
        public bool IsDone { get; set; }


        [JsonProperty(PropertyName = "y")]
        public bool HasDueDate { get; set; }

        [JsonProperty(PropertyName = "u")]
        public DateTime DueDate { get; set; }

        [JsonProperty(PropertyName = "i")]
        public DateTime CreateDate { get; set; }


        [JsonProperty(PropertyName = "o")]
        public Guid CategoryID { get; set; }

        [JsonIgnore]
        [JsonProperty(PropertyName = "p")]
        public List<DateTime> Reminders { get; set; } = new List<DateTime>();


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
                Reminders = Reminders,
            };
        }
    }
}
