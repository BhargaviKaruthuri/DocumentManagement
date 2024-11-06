using DocumentManagement.Data;
using DocumentManagement.Data.Domain;
using DocumentManagement.Service.Mapper;


namespace DocumentManagement.Service.Interface
{
    public interface IDocumentService
    {
        Task<bool> addDocument(DocumentDetailsDto documentDetails);
        Task<DataTableResponse<DocumentDetailsDto>> GetAllDocuments(DataTableRequest request);
        Task<DocumentDetailsDto> ViewAllDocsRelatedtoPIN(int docDetailId);
        Task<List<DocumentsDto>> GetAllFilesforPin(int docDetailId);
        Task<DocumentsDto> GetFilePath(int docId);
    }
}
