using DocumentManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Service.Mapper
{
    public class BaseEntityDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public string CreatedOn { get; set; } = string.Empty;
        public string UpdatedOn { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = UserContent.CreatedBy;
        public string UpdatedBy { get; set; } = UserContent.UpdatedBy;
    }
}
