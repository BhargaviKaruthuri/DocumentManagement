using DocumentManagement.Data.Domain;
using System.Reflection.Metadata;

namespace DocumentManagement.Service.Mapper
{
    public class MappingDocument 
    {
        public static DocumentDetailsDto ToDto(DocumentDetails documentDetails)
        {
            return new DocumentDetailsDto
            {
                Id = documentDetails.Id,
                PropertyIdentityNumber = documentDetails.PropertyIdentityNumber,
                Name = documentDetails.Name,
                Description = documentDetails.Description,
                Documents = documentDetails.Documents.Select(d => new DocumentsDto
                {
                    Id = d.Id,
                    DocumentDetailId = d.DocumentDetailId,
                    FileName = d.FileName,
                    FileUrl = d.FileUrl,
                    FileType = d.FileType
                }).ToList()
            };
        }

        public static DocumentDetails ToEntity(DocumentDetailsDto documentDetailsDto)
        {
            return new DocumentDetails
            {
                Id = documentDetailsDto.Id,
                PropertyIdentityNumber = documentDetailsDto.PropertyIdentityNumber,
                Name = documentDetailsDto.Name,
                Description = documentDetailsDto.Description,
                Documents = documentDetailsDto.Documents.Select(d => new Documents
                {
                    Id = d.Id,
                    DocumentDetailId = d.DocumentDetailId,
                    FileName = d.FileName,
                    FileUrl = d.FileUrl,
                    FileType = d.FileType
                }).ToList()
            };
        }
        public static List<DocumentDetailsDto> ToDto(List<DocumentDetails> documentDetailsList)
        {
            return documentDetailsList.Select(ToDto).ToList();
        }

        public static DocumentsDto ToDto(Documents documents)
        {
            return new DocumentsDto
            {
               
                    Id = documents.Id,
                    DocumentDetailId = documents.DocumentDetailId,
                    FileName = documents.FileName,
                    FileUrl = documents.FileUrl,
                    FileType = documents.FileType
                
            };
        }

        public static List<DocumentsDto> ToDto(List<Documents> documentsList)
        {
            return documentsList.Select(ToDto).ToList();
        }

    }
}
