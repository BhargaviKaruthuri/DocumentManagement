using DocumentManagement.Data.Domain;
using DocumentManagement.Service.Mapper;
using Microsoft.AspNetCore.Http;


namespace DocumentManagement.Tests
{
    public static class ModelMockingData
    {
        public static readonly int DocId = 1;
        public static readonly int Pin = 123;
        public static readonly int BadReqDocId = 0;
        public static readonly string ZipName = "123.zip";
        public static readonly string ZipContentType = "application/zip";
        public static readonly string FileUrl = "Sample/test.txt";
        public static readonly string FileType = "text/plain";

        public static readonly int BadRequestStatusCode = 400;
        public static readonly int InternalServerStatusCode = 500;
        public static readonly int SuccessStatusCode = 200;

        public static readonly string ErrorMessageforInternalServer = "Test exception";
    }
    public class ModelMock
    {
        public DocumentDetailsDto documentDetailsDto = new DocumentDetailsDto { Id = 1, PropertyIdentityNumber = 1234, Name = "Test Document", Description = "Test Description" };

        public List<DocumentsDto> documentsDto = new List<DocumentsDto>
            {
                new DocumentsDto { Id = 1, DocumentDetailId = 1, FileName = "doc1.pdf",FileUrl ="Test/doc1.pdf",FileType="pdf" },
                new DocumentsDto { Id = 2, DocumentDetailId = 1, FileName = "doc2.docs" ,FileUrl ="Test/doc2.docs",FileType="docs" }
            };

        public DocumentsDto docsDto = new DocumentsDto { FileUrl = "Test/doc1.pdf", FileType = "pdf" };


        public DocumentDetails documentDetails = new DocumentDetails { Id = 1, PropertyIdentityNumber = 1234, Name = "Test Document", Description = "Test Description" };

        public List<Documents> documents = new List<Documents>
            {
                new Documents { Id = 1, DocumentDetailId = 1, FileName = "doc1.pdf",FileUrl ="Test/doc1.pdf",FileType="pdf" },
                new Documents { Id = 2, DocumentDetailId = 1, FileName = "doc2.docs" ,FileUrl ="Test/doc2.docs",FileType="docs" }
            };

        public Documents docs = new Documents { FileUrl = "Test/doc1.pdf", FileType = "pdf" };

        public List<DocumentDetails> documentsListModel = new List<DocumentDetails>
            {
              new DocumentDetails { Id = 1,PropertyIdentityNumber=1234,Name = "Document 1",Description ="Test Execution 1" },
              new DocumentDetails { Id = 2,PropertyIdentityNumber=4567, Name = "Document 2" ,Description ="Test Execution 2"}
            };

        public IFormFile GetMockFormFile(string fileName)
        {
            var content = "Test file content";
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "id_from_form", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };
        }
    }
}
