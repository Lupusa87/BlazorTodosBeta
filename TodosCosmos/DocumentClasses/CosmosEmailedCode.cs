using System;
using static TodosCosmos.Enums;

namespace TodosCosmos.DocumentClasses
{
    public class CosmosEmailedCode:BaseDocType
    {

        public CosmosEmailedCode()
        {
            ID = Guid.NewGuid();
            DocType = (int)DocTypeEnum.EmailedCode;
            GeneratePK();
        }

        public string Email { get; set; }

        public string Code { get; set; }

        public int OperationType { get; set; }


        public string IPAddress { get; set; }


        public string MachineID { get; set; }

        public DateTime AddDate { get; set; }
    }
}
