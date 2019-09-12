using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosWebApi.DataLayer.TSEntities
{
    public class TSEmailedCode
    {
        public string ID { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }

        public int OperationType { get; set; }


        public string IPAddress { get; set; }


        public string MachineID { get; set; }

        public DateTime AddDate { get; set; }
    }
}
