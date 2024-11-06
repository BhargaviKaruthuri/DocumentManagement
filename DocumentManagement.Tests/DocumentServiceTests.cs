using DocumentManagement.Data.Domain;
using DocumentManagement.Data;
using DocumentManagement.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManagement.Service.Mapper;

namespace DocumentManagement.Tests
{
    [TestFixture]
    public class DocumentServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IConfigurationSection> _mockConfigSection;
        private DocumentService _documentService;
        private ApplicationDbContext _context;
        private Mock<IDbConnectionFactory> _mockDbConnectionFactory;
        ModelMock modelMock = new ModelMock();

        [SetUp]
        public void Setup()
        {

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfigSection = new Mock<IConfigurationSection>();

            string connString = "Server=(localdb)\\mssqllocaldb;Database=TestDatabase;Trusted_Connection=True;MultipleActiveResultSets=true";
            _mockConfigSection.Setup(x => x.Value).Returns(connString);
            _mockConfiguration.Setup(c => c.GetSection("ConnectionStrings:DefaultConnection")).Returns(_mockConfigSection.Object);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            _mockDbConnectionFactory = new Mock<IDbConnectionFactory>();

            _documentService = new DocumentService(_context, _mockConfiguration.Object, _mockDbConnectionFactory.Object);

        }

        [Test]
        public async Task addDocument_ShouldAddDocumentsandDetails()
        {
            // Arrange
            var documentDetailsModel = new DocumentDetailsDto();
            documentDetailsModel = modelMock.documentDetailsDto;
            documentDetailsModel.Documents = modelMock.documentsDto;

            // Act
            var result = await _documentService.addDocument(documentDetailsModel);

            // Assert
            Assert.That(result, Is.True);

            var addedDocumentDetails = await _context.DocumentDetails.FirstOrDefaultAsync();
            Assert.That(addedDocumentDetails, Is.Not.Null);
            Assert.That(modelMock.documentDetails.Id, Is.EqualTo(addedDocumentDetails.Id));
            Assert.That(modelMock.documentDetails.PropertyIdentityNumber, Is.EqualTo(addedDocumentDetails.PropertyIdentityNumber));
            Assert.That(modelMock.documentDetails.Name, Is.EqualTo(addedDocumentDetails.Name));
            Assert.That(modelMock.documentDetails.Description, Is.EqualTo(addedDocumentDetails.Description));

            foreach (var document in modelMock.documents)
            {
                Assert.That(modelMock.documentDetails.Id, Is.EqualTo(document.DocumentDetailId));
            }
        }

        [Test]
        public async Task GetAllDocuments_ShouldReturnExpectedResults()
        {
            // Arrange
            var dataTableRequest = new DataTableRequest(draw: "1", start: "0", length: "10", sortColumn: "Name",
                sortColumnDirection: "asc", searchValue: null, pageSize: 10, skip: 0, recordsTotal: 0);

            _mockDbConnectionFactory.Setup(db => db.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>()))
                                    .ReturnsAsync(modelMock.documentsListModel.Count);

            _mockDbConnectionFactory.Setup(db => db.QueryAsync<DocumentDetails>(It.IsAny<string>(), It.IsAny<object>()))
                                    .ReturnsAsync(modelMock.documentsListModel);

            // Act
            var result = await _documentService.GetAllDocuments(dataTableRequest);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Draw, Is.EqualTo(dataTableRequest.Draw));
            Assert.That(result.RecordsTotal, Is.EqualTo(modelMock.documentsListModel.Count));
            Assert.That(result.RecordsFiltered, Is.EqualTo(modelMock.documentsListModel.Count));
            Assert.That(result.Data.Count, Is.EqualTo(modelMock.documentsListModel.Count));
            Assert.That(result.Data[0].Id, Is.EqualTo(1));
            Assert.That(result.Data[0].PropertyIdentityNumber, Is.EqualTo(1234));
            Assert.That(result.Data[0].Name, Is.EqualTo("Document 1"));
            Assert.That(result.Data[0].Description, Is.EqualTo("Test Execution 1"));
            Assert.That(result.Data[1].Id, Is.EqualTo(2));
            Assert.That(result.Data[1].PropertyIdentityNumber, Is.EqualTo(4567));
            Assert.That(result.Data[1].Name, Is.EqualTo("Document 2"));
            Assert.That(result.Data[1].Description, Is.EqualTo("Test Execution 2"));
        }

        [Test]
        public async Task ViewAllDocsRelatedtoPIN_ShouldReturnExpectedResults()
        {
            //Arrange

            _mockDbConnectionFactory.Setup(db => db.QueryAsync<DocumentDetails>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(new List<DocumentDetails> { modelMock.documentDetails });

            _mockDbConnectionFactory.Setup(db => db.QueryAsync<Documents>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(modelMock.documents);

            //Act
            var details = await _documentService.ViewAllDocsRelatedtoPIN(ModelMockingData.DocId);

            //Assert
            Assert.That(details, Is.Not.Null);
            Assert.That(details.Documents, Is.Not.Null);
            Assert.That(details.Id, Is.EqualTo(1));
            Assert.That(details.PropertyIdentityNumber, Is.EqualTo(1234));
            Assert.That(details.Name, Is.EqualTo("Test Document"));
            Assert.That(details.Description, Is.EqualTo("Test Description"));
            foreach (var document in details.Documents)
            {
                Assert.That(document.DocumentDetailId, Is.EqualTo(details.Id));
            }
        }

        [Test]
        public async Task GetAllFilesforPin_ShouldReturnExpectedResults()
        {
            //Arrange

            _mockDbConnectionFactory.Setup(db => db.QueryAsync<Documents>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(modelMock.documents);

            //Act
            var results = await _documentService.GetAllFilesforPin(ModelMockingData.DocId);

            //Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(modelMock.documents.Count));
            Assert.That(results[0].FileName, Is.EqualTo("doc1.pdf"));
            Assert.That(results[0].FileType, Is.EqualTo("pdf"));
            Assert.That(results[1].FileName, Is.EqualTo("doc2.docs"));
            Assert.That(results[1].FileType, Is.EqualTo("docs"));

        }

        [Test]
        public async Task GetFilePath_ShouldReturnExpectedResults()
        {
            //Arrange
            _mockDbConnectionFactory.Setup(db => db.QueryAsync<Documents>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(new List<Documents> { modelMock.docs });

            //Act
            var result = await _documentService.GetFilePath(ModelMockingData.DocId);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileUrl, Is.EqualTo(modelMock.docs.FileUrl));
            Assert.That(result.FileType, Is.EqualTo(modelMock.docs.FileType));
        }
    }
}
