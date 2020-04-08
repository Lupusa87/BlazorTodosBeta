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



        [JsonProperty(PropertyName = "uid")]
        public Guid UserID { get; set; }

        [JsonProperty(PropertyName = "n")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "d")]
        public string Description { get; set; }


        [JsonProperty(PropertyName = "p")]
        public int Priority { get; set; }

        [JsonProperty(PropertyName = "isd")]
        public bool IsDone { get; set; }


        [JsonProperty(PropertyName = "hdd")]
        public bool HasDueDate { get; set; }

        [JsonProperty(PropertyName = "dd")]
        public DateTime DueDate { get; set; }

        [JsonProperty(PropertyName = "cd")]
        public DateTime CreateDate { get; set; }


        [JsonProperty(PropertyName = "cid")]
        public Guid CategoryID { get; set; }

        [JsonIgnore]
        [JsonProperty(PropertyName = "rs")]
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
