using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Data
{
    public class GlobalConstants
    {
        public const string UploadsFolder = @"uploads\";
    }
    public static class ErrorMessages
    {
        public static readonly string InvalidDocumentId = "Invalid document Id.";
        public static readonly string InvalidDocumentDetailId = "Invalid document detail ID.";
        public static readonly string InvalidDocumentDetails = "Invalid document details.";
        public static readonly string FileNotFound = "File not found.";
    }
    public static class UserContent
    {
        public static readonly string UpdatedBy = "System";
        public static readonly string CreatedBy = "System";
    }
}
