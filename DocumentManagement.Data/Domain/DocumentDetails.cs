using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Data.Domain
{
    public class DocumentDetails : BaseEntity
    {
        public int PropertyIdentityNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Documents> Documents { get; set; } = new List<Documents>();

    }
    public class Documents : BaseEntity
    {
        public int DocumentDetailId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DocumentDetails DocumentDetails { get; set; } = new DocumentDetails();

    }
}
