using DocumentManagement.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Service.Mapper
{
    public class DocumentDetailsDto : BaseEntityDto
    {
        public int PropertyIdentityNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<DocumentsDto> Documents { get; set; } = new List<DocumentsDto>();
    }
    public class DocumentsDto : BaseEntityDto
    {
        public int DocumentDetailId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;

    }
}
