using DocumentManagement.Data.Domain;
using DocumentManagement.Data;
using DocumentManagement.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;
using System.IO.Compression;
using DocumentManagement.Service.Mapper;
using Microsoft.Extensions.Logging;


namespace DocumentManagement.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        private readonly IDocumentService _documentServices;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment, IDocumentService documentServices, ILogger<DocumentController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _documentServices = documentServices;
            _logger = logger;
        }

        [HttpPost("addDocument")]
        public async Task<IActionResult> AddDocumet([FromForm] string documentDetails, List<IFormFile> files)
        {
            try
            {
                _logger.LogInformation("Executing AddDocumet method in DocumentController.");

                List<DocumentsDto> documents = new List<DocumentsDto>();
                DocumentDetailsDto Details = new DocumentDetailsDto();
                var docDetails = JsonConvert.DeserializeObject<DocumentDetailsDto>(documentDetails);
                if (docDetails == null)
                {

                    return BadRequest(ErrorMessages.InvalidDocumentDetails);
                }
                else
                {
                    if (String.IsNullOrEmpty(docDetails.Name) || docDetails.PropertyIdentityNumber <= 0 || files.Count <= 0)
                    {
                        return BadRequest(ErrorMessages.InvalidDocumentDetails);
                    }

                    // Sanitize all input params
                    docDetails.Name = HttpUtility.HtmlEncode(docDetails.Name);
                    docDetails.Description = HttpUtility.HtmlEncode(docDetails.Description);
                    Details = docDetails;
                }

                if (files != null && files.Count > 0)
                {
                    string guidId = Guid.NewGuid().ToString();
                    foreach (var file in files)
                    {
                        string folderpath = Path.Combine(_hostingEnvironment.WebRootPath, GlobalConstants.UploadsFolder);
                        if (!Directory.Exists(folderpath))
                        {
                            Directory.CreateDirectory(folderpath);
                        }
                        string filepath = Path.Combine(folderpath, guidId + "_" + file.FileName);

                        using (FileStream fs = System.IO.File.Create(filepath))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                        DocumentsDto document = new DocumentsDto()
                        {
                            UpdatedBy = UserContent.UpdatedBy,
                            CreatedBy = UserContent.CreatedBy,
                            FileName = file.FileName,
                            FileUrl = guidId + "_" + file.FileName,
                            FileType = file.ContentType,
                            IsActive = true
                        };
                        documents.Add(document);
                    }

                }
                Details.Documents = documents;
                return Ok(await _documentServices.addDocument(Details));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DocumentController");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("getAllDocuments")]
        public async Task<IActionResult> GetAllDocuments()
        {
            try
            {
                _logger.LogInformation("Executing GetAllDocuments method in DocumentController.");

                DataTableRequest request = new DataTableRequest(Request.Form);
                return Ok(await _documentServices.GetAllDocuments(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DocumentController");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("viewAllDocsRelatedtoPIN")]
        public async Task<IActionResult> ViewAllDocsRelatedtoPIN(int docDetailId)
        {
            try
            {
                _logger.LogInformation("Executing GetPinRelatedDocuments method in DocumentController.");

                if (docDetailId <= 0)
                {
                    return BadRequest(ErrorMessages.InvalidDocumentDetailId);
                }

                return Ok(await _documentServices.ViewAllDocsRelatedtoPIN(docDetailId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DocumentController");
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("downloadAllDocsforPin")]
        public async Task<IActionResult> DownloadAllDocsforPin(int id, int pin)
        {
            try
            {
                _logger.LogInformation("Executing DownloadAllDocsforPin method in DocumentController.");
                if (id <= 0)
                {
                    return BadRequest(ErrorMessages.InvalidDocumentDetailId);
                }

                List<DocumentsDto> documents = await _documentServices.GetAllFilesforPin(id);
                if (documents.Count() > 0)
                {
                    string filesPath = Path.Combine(_hostingEnvironment.WebRootPath) + $@"\{GlobalConstants.UploadsFolder}";
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var fileName in documents)
                            {
                                var filePath = Path.Combine(filesPath, fileName.FileUrl);

                                if (System.IO.File.Exists(filePath))
                                {
                                    var fileBytes = System.IO.File.ReadAllBytes(filePath);

                                    var zipEntry = zipArchive.CreateEntry(fileName.FileUrl);
                                    using (var zipStream = zipEntry.Open())
                                    {
                                        zipStream.Write(fileBytes, 0, fileBytes.Length);
                                    }
                                }
                            }

                        }

                        memoryStream.Position = 0;
                        return File(memoryStream.ToArray(), "application/zip", pin + ".zip");
                    }

                }
                return Ok(null);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DocumentController");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getDocumentContent")]
        public async Task<IActionResult> GetDocumentContent(int docId)
        {
            try
            {
                _logger.LogInformation("Executing GetDocumentContent method in DocumentController.");
                if (docId <= 0)
                {
                    return BadRequest(ErrorMessages.InvalidDocumentId);
                }
                string fileFolderPath = Path.Combine(_hostingEnvironment.WebRootPath) + $@"\{GlobalConstants.UploadsFolder}";
                DocumentsDto documents = await _documentServices.GetFilePath(docId);
                string filepath = documents.FileUrl;
                string filetype = documents.FileType;
                if (string.IsNullOrEmpty(filepath))
                {
                    return BadRequest(ErrorMessages.FileNotFound);
                }
                byte[] fileContent = null;
                filepath = Path.Combine(fileFolderPath + filepath);
                if (System.IO.File.Exists(filepath))
                {
                    fileContent = System.IO.File.ReadAllBytes(filepath);
                }
                return Ok(new { fileContent, filetype });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DocumentController");
                return StatusCode(500, ex.Message);
            }
        }

        
    }
}
