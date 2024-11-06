using Dapper;
using DocumentManagement.Data;
using DocumentManagement.Data.Domain;
using DocumentManagement.Service.Interface;
using DocumentManagement.Service.Mapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DocumentManagement.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
         private readonly IConfiguration _configuration;
        private readonly IDbConnectionFactory _dbConnectionFactory;


        public DocumentService(ApplicationDbContext context, IConfiguration configuration, IDbConnectionFactory dbConnectionFactory)
        {
            _context = context;
            _configuration = configuration;
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<bool> addDocument(DocumentDetailsDto documentDetailsDto)
        {
            var documentDetails = MappingDocument.ToEntity(documentDetailsDto);
            _context.DocumentDetails.Add(documentDetails);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<DataTableResponse<DocumentDetailsDto>> GetAllDocuments(DataTableRequest dataTableRequest)
        {
            string documentsList = DBQuery.getDocumentsList;
            string countQuery = DBQuery.getDocumentsListCount;
            DynamicParameters parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(dataTableRequest.SearchValue))
            {
                documentsList += " WHERE (doc.PropertyIdentityNumber LIKE @SearchValue OR doc.Name LIKE @SearchValue" +
                    " OR doc.Description LIKE @SearchValue OR doc.CreatedBy LIKE @SearchValue OR doc.UpdatedBy LIKE @SearchValue)";

                countQuery += " WHERE (doc.PropertyIdentityNumber LIKE @SearchValue OR doc.Name LIKE @SearchValue" +
                     " OR doc.Description LIKE @SearchValue OR doc.CreatedBy LIKE @SearchValue OR doc.UpdatedBy LIKE @SearchValue)";

                parameters.Add("@SearchValue", "%" + dataTableRequest.SearchValue + "%");
            }
            if (!(string.IsNullOrEmpty(dataTableRequest.SortColumn) && string.IsNullOrEmpty(dataTableRequest.SortColumnDirection)))
            {
                documentsList += " ORDER BY doc." + dataTableRequest.SortColumn + " " + dataTableRequest.SortColumnDirection;
            }
            documentsList += $" OFFSET {dataTableRequest.Skip} ROWS FETCH NEXT {dataTableRequest.PageSize} ROWS ONLY";

            int recordsTotal = await _dbConnectionFactory.ExecuteScalarAsync<int>(countQuery, parameters);
            var queryResult = await _dbConnectionFactory.QueryAsync<DocumentDetails>(documentsList, parameters);
            var result = queryResult.ToList();
            var data = MappingDocument.ToDto(result);
            var response = new DataTableResponse<DocumentDetailsDto>
            {
                Draw = dataTableRequest.Draw,
                RecordsFiltered = recordsTotal,
                RecordsTotal = recordsTotal,
                Data = data
            };

            return response;
        }

        public async Task<DocumentDetailsDto> ViewAllDocsRelatedtoPIN(int docDetailId)
        {
            DocumentDetailsDto documentDetailsModel = new DocumentDetailsDto();
            string queryDocDetails = DBQuery.getDocumentsList;
            queryDocDetails += " WHERE doc.id=@docDetailId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@docDetailId", docDetailId);
            var docDetails = await _dbConnectionFactory.QueryAsync<DocumentDetails>(queryDocDetails, parameters);
            var documentDetails = docDetails.FirstOrDefault();
            documentDetailsModel = MappingDocument.ToDto(documentDetails);
            documentDetailsModel.Documents = await GetAllFilesforPin(docDetailId);
            return documentDetailsModel;
        }

        public async Task<List<DocumentsDto>> GetAllFilesforPin(int docDetailId)
        {
            List<DocumentsDto> documentsdto = new List<DocumentsDto>();
            string queryAllDocs = DBQuery.getDocumentsforPin;
            queryAllDocs += " WHERE dd.Id = @docDetailId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@docDetailId", docDetailId);
            var docs = await _dbConnectionFactory.QueryAsync<Documents>(queryAllDocs, parameters);
            var documents = docs.ToList();
            documentsdto = MappingDocument.ToDto(documents);
            return documentsdto;
        }

        public async Task<DocumentsDto> GetFilePath(int docId)
        {
            string fileName = string.Empty;
            string queryFileName = DBQuery.getfileName;
            queryFileName += " WHERE id=@docId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@docId", docId);
            var docDetails = await _dbConnectionFactory.QueryAsync<Documents>(queryFileName, parameters);
            return MappingDocument.ToDto(docDetails.FirstOrDefault());
        }
    }
}
