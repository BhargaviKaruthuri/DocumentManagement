using DocumentManagement.Data;
using DocumentManagement.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManagement.Web.Api;
using DocumentManagement.Data.Domain;
using DocumentManagement.Service.Mapper;

namespace DocumentManagement.Tests
{
    [TestFixture]
    public class DocumentControllerTests
    {
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly DocumentController _controller;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<ILogger<DocumentController>> _mockLogger;
        private string _mockWebRootPath;

        ModelMock modelMock = new ModelMock();

        public DocumentControllerTests()
        {
            _mockDocumentService = new Mock<IDocumentService>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<DocumentController>>();
            _mockWebRootPath = Path.GetTempPath();
            _mockWebHostEnvironment.Setup(m => m.WebRootPath).Returns(_mockWebRootPath);
            _controller = new DocumentController(_mockWebHostEnvironment.Object, _mockDocumentService.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "draw", "1" },
                { "start", "0" },
                { "length", "10" },
                { "sortColumn", "Name" },
                { "sortColumnDirection","asc" },
                { "searchValue", "" },
                { "pageSize","10" },
                { "skip", "0" },
                {"recordsTotal" , "0" }

            });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }


        [Test]
        public async Task AddDocumet_WithValidRequest()
        {
            //Arrange

            var documentDetailsJson = JsonConvert.SerializeObject(modelMock.documentDetailsDto);
            var files = new List<IFormFile> { modelMock.GetMockFormFile("file1.txt") };

            _mockDocumentService.Setup(service => service.addDocument(It.IsAny<DocumentDetailsDto>()))
               .ReturnsAsync(true);

            // Act
            var result = await _controller.AddDocumet(documentDetailsJson, files) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.SuccessStatusCode));
            Assert.That(result.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task AddDocument_InvalidDocumentDetails_ReturnsBadRequest()
        {
            // Arrange
            var invalidDocumentDetails = "{}";
            var files = new List<IFormFile>();

            // Act
            var result = await _controller.AddDocumet(invalidDocumentDetails, files) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.BadRequestStatusCode));
            Assert.That(result.Value, Is.EqualTo(ErrorMessages.InvalidDocumentDetails));
        }
        [Test]
        public async Task AddDocument_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange

            var documentDetailsJson = JsonConvert.SerializeObject(modelMock.documentDetailsDto);
            var files = new List<IFormFile> { modelMock.GetMockFormFile("file1.txt") };

            _mockDocumentService.Setup(service => service.addDocument(It.IsAny<DocumentDetailsDto>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AddDocumet(documentDetailsJson, files) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.InternalServerStatusCode));
            Assert.That(result.Value, Is.EqualTo(ModelMockingData.ErrorMessageforInternalServer));
        }

        [Test]
        public async Task GetAllDocuments_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new DataTableRequest(new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "draw", "1" },
                { "start", "0" },
                { "length", "10" },
                { "sortColumn", "Name" },
                { "sortColumnDirection","asc" },
                { "searchValue", "" },
                { "pageSize","10" },
                { "skip", "0" },
                {"recordsTotal" , "0" }
            }));
            var expectedResponse = new DataTableResponse<DocumentDetailsDto>
            {
                Draw = "1",
                RecordsTotal = 2,
                RecordsFiltered = 2,
                Data = new List<DocumentDetailsDto>
                {
                    new DocumentDetailsDto { PropertyIdentityNumber =123,Name="Test1" ,Description="Test1 description" },
                    new DocumentDetailsDto { PropertyIdentityNumber =456,Name="Test2" ,Description="Test2 description"  }
                }
            };

            _mockDocumentService.Setup(service => service.GetAllDocuments(It.IsAny<DataTableRequest>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAllDocuments() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.SuccessStatusCode));
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task GetAllDocuments_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockDocumentService.Setup(service => service.GetAllDocuments(It.IsAny<DataTableRequest>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetAllDocuments() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.InternalServerStatusCode));
            Assert.That(result.Value, Is.EqualTo(ModelMockingData.ErrorMessageforInternalServer));
        }

        [Test]
        public async Task ViewAllDocsRelatedtoPIN_WithValidRequest_ReturnsOk()
        {
            //Arrange
            var documentDetailsModel = new DocumentDetailsDto();
            documentDetailsModel = modelMock.documentDetailsDto;
            documentDetailsModel.Documents = modelMock.documentsDto;


            _mockDocumentService.Setup(service => service.ViewAllDocsRelatedtoPIN(It.IsAny<int>())).ReturnsAsync(documentDetailsModel);

            //Act
            var result = await _controller.ViewAllDocsRelatedtoPIN(ModelMockingData.DocId) as ObjectResult;

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.SuccessStatusCode));
            Assert.That(result.Value, Is.EqualTo(documentDetailsModel));
        }
        [Test]
        public async Task ViewAllDocsRelatedtoPIN_InvaliDocumentId_ReturnsBadRequest()
        {
            //Arrange
            // -- done at ModelsMockingData class

            // Act
            var result = await _controller.ViewAllDocsRelatedtoPIN(ModelMockingData.BadReqDocId) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.BadRequestStatusCode));
            Assert.That(result.Value, Is.EqualTo(ErrorMessages.InvalidDocumentDetailId));

        }

        [Test]
        public async Task ViewAllDocsRelatedtoPIN_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockDocumentService.Setup(service => service.ViewAllDocsRelatedtoPIN(It.IsAny<int>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.ViewAllDocsRelatedtoPIN(ModelMockingData.DocId) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.InternalServerStatusCode));
            Assert.That(result.Value, Is.EqualTo(ModelMockingData.ErrorMessageforInternalServer));

        }
        [Test]
        public async Task DownloadAllDocsforPin_WithValidRequest_ReturnsOk()
        {
            //Arrange
            _mockDocumentService.Setup(service => service.GetAllFilesforPin(It.IsAny<int>())).ReturnsAsync(modelMock.documentsDto);

            //Act
            var result = await _controller.DownloadAllDocsforPin(ModelMockingData.DocId, ModelMockingData.Pin) as FileContentResult;

            //Assert

            Assert.That(result.FileDownloadName, Is.EqualTo(ModelMockingData.ZipName));
            Assert.That(result.ContentType, Is.EqualTo(ModelMockingData.ZipContentType));

        }
        [Test]
        public async Task DownloadAllDocsforPin_InvalidId_ReturnsBadRequest()
        {
            //Arrange
            // -- done at ModelsMockingData class

            //Act
            var result = await _controller.DownloadAllDocsforPin(ModelMockingData.BadReqDocId, ModelMockingData.Pin) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.BadRequestStatusCode));
            Assert.That(result.Value, Is.EqualTo(ErrorMessages.InvalidDocumentDetailId));

        }

        [Test]
        public async Task DownloadAllDocsforPin_ExceptionThrown_ReturnsInternalServer()
        {
            //Assert
            _mockDocumentService.Setup(service => service.GetAllFilesforPin(It.IsAny<int>())).ThrowsAsync(new Exception("Test exception"));

            //Act
            var result = await _controller.DownloadAllDocsforPin(ModelMockingData.DocId, ModelMockingData.Pin) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.InternalServerStatusCode));
            Assert.That(result.Value, Is.EqualTo(ModelMockingData.ErrorMessageforInternalServer));


        }
        [Test]
        public async Task GetDocumentContent_WithValidRequest_ReturnsOk()
        {
            //Arrange
            _mockDocumentService.Setup(service => service.GetFilePath(It.IsAny<int>())).ReturnsAsync(modelMock.docsDto);

            //Act
            var result = await _controller.GetDocumentContent(ModelMockingData.DocId) as ObjectResult;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.SuccessStatusCode));
        }

        [Test]
        public async Task GetDocumentContent_WithInvalidDocId_ReturnsBadRequest()
        {
            //Arrange
            // -- done at ModelsMockingData class

            //Act
            var result = await _controller.GetDocumentContent(ModelMockingData.BadReqDocId) as ObjectResult;

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.BadRequestStatusCode));
            Assert.That(result.Value, Is.EqualTo(ErrorMessages.InvalidDocumentId));
        }

        [Test]
        public async Task GetDocumentContent_ExceptionThrown_ReturnsInternalServer()
        {
            //Assert
            _mockDocumentService.Setup(service => service.GetFilePath(It.IsAny<int>())).ThrowsAsync(new Exception("Test exception"));

            //Act
            var result = await _controller.GetDocumentContent(ModelMockingData.DocId) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ModelMockingData.InternalServerStatusCode));
            Assert.That(result.Value, Is.EqualTo(ModelMockingData.ErrorMessageforInternalServer));


        }
    }
}
