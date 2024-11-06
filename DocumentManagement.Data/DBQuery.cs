namespace DocumentManagement.Data
{
    public class DBQuery
    {
        public static readonly string insertDocuments = "INSERT INTO Documents (DocumentDetailId,FileName,FileUrl,FileType,IsActive,CreatedOn,UpdatedOn,CreatedBy,UpdatedBy) VALUES (@DocumentDetailId,@FileName,@FileUrl,@FileType,@IsActive,@CreatedOn,@UpdatedOn,@CreatedBy,@UpdatedBy)";
        public static readonly string getDocumentsList = "SELECT doc.id,doc.Name,doc.Description,doc.PropertyIdentityNumber,doc.IsActive,doc.CreatedBy,doc.UpdatedBy from DocumentDetails doc ";
        public static readonly string getDocumentsListCount = "SELECT COUNT(1) FROM DocumentDetails doc";
        public static readonly string getDocumentsforPin = "SELECT d.id,d.FileName,d.FileUrl,d.FileType FROM DocumentDetails dd LEFT JOIN Documents d on dd.id = d.DocumentDetailId ";
        public static readonly string getfileName = "SELECT FileUrl,FileType from Documents";
    }
}
