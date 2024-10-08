using Domain.Contact.Entity;
using Domain.Contact.Repository;
using Domain.Contact.Service;
using Domain.Region.Entity;
using Domain.Region.Service;
using Infraestructure.Context;
using Infraestructure.Repository.ContactsRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Drawing;
using TechChallenge1.Controllers.Contacts.Dto;
using TechChallenge1.Controllers.Contacts.Http;

namespace Tests
{
    public class ContactsTest
    {
        Mock<IContactService> mockContactService;
        Mock<IRegionService> mockRegionService;
        public ContactsTest()
        {
            mockContactService = new Mock<IContactService>();
            mockRegionService = new Mock<IRegionService>();
        }

        #region MOCK DATA
        private static ContactPostDto dtoPostInvalidRegion = new ContactPostDto
        {
            Active = true,
            CellphoneNumber = "968061073",
            Email = "teste.xpto@gmail.com",
            Name = "Test",
            RegionId = Guid.Empty,
        };
        public static IEnumerable<object[]> GetInvalidContactRegionPostDtoData()
        {
            yield return new object[] { dtoPostInvalidRegion, "RegionId must not be an empty guid" };
        }

        private static ContactPostDto dtoPostInvalidNumber = new ContactPostDto
        {
            Active = true,
            CellphoneNumber = "1",
            Email = "teste.xpto@gmail.com",
            Name = "Test",
            RegionId = Guid.NewGuid(),
        };
        public static IEnumerable<object[]> GetInvalidContactNumberPostDtoData()
        {
            yield return new object[] { dtoPostInvalidNumber, "cellphone number must have 9 digits" };
        }

        private static ContactPostDto dtoPostInvalidEmail = new ContactPostDto
        {
            Active = true,
            CellphoneNumber = "968061072",
            Email = "teste.xptocom",
            Name = "Test",
            RegionId = Guid.NewGuid(),
        };
        public static IEnumerable<object[]> GetInvalidContactEmailPostDtoData()
        {
            yield return new object[] { dtoPostInvalidEmail, "email is not valid" };
        }

        private static ContactPostDto dtoPostInvalidName = new ContactPostDto
        {
            Active = true,
            CellphoneNumber = "968061071",
            Email = "teste.xpto@gmail.com",
            Name = null,
            RegionId = Guid.NewGuid(),
        };
        public static IEnumerable<object[]> GetInvalidContactNamePostDtoData()
        {
            yield return new object[] { dtoPostInvalidName, "name must not be null" };
        }

        private static RegionEntity validRegion = new RegionEntity
        {
            Active = true,
            CreationDate = DateTime.Now,
            DDD = "11",
            Description = "Test",
            Id = Guid.NewGuid(),
        };
        #endregion

        [Theory]
        [MemberData(nameof(GetInvalidContactNumberPostDtoData))]
        [MemberData(nameof(GetInvalidContactRegionPostDtoData))]
        [MemberData(nameof(GetInvalidContactEmailPostDtoData))]
        [MemberData(nameof(GetInvalidContactNamePostDtoData))]
        public async Task ShouldReturnErrorAllPostDto(ContactPostDto dto, string expectedErrorMsg)
        {
            var validator = new ContactPostValidate();
            var result = validator.Validate(dto);
            Assert.False(result.IsValid);
            Assert.Equal(result.Errors.FirstOrDefault()?.ErrorMessage, expectedErrorMsg);
        }

        [Fact]
        public async Task ShouldReturnErrorPostNotFoundRegion()
        {
            ContactPostDto dto = new ContactPostDto
            {
                Active = true,
                CellphoneNumber = "968061073",
                Email = "teste.xpto@gmail.com",
                Name = "Test",
                RegionId = Guid.Empty,
            };
            var controller = new ContactsController(mockContactService.Object, mockRegionService.Object);

            var result = await controller.AddContact(dto);
            var objectResult = result as ObjectResult;
            var statusCode = objectResult.StatusCode;
            var message = (string)objectResult?.Value ?? "";

            Assert.Equal("region not found", message.ToLower());
            Assert.Equal(statusCode, StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task ShouldReturnOKPost()
        {
            var regionGUID = Guid.NewGuid();
            ContactPostDto dto = new ContactPostDto
            {
                Active = true,
                CellphoneNumber = "968061073",
                Email = "teste.xpto@gmail.com",
                Name = "Test",
                RegionId = regionGUID,
            };
            var mockRegionService = new Mock<IRegionService>();
            mockRegionService.Setup(m => m.GetById(regionGUID)).Returns(Task.FromResult(validRegion));

            var mockContactService = new Mock<IContactService>();
            mockContactService.Setup(m => m.AddAsync(It.IsAny<ContactEntity>())).Returns(Task.CompletedTask);

            var controller = new ContactsController(mockContactService.Object, mockRegionService.Object);

            var result = await controller.AddContact(dto);
            var objectResult = result as StatusCodeResult;
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        }

        //[Fact]
        //public async Task ShouldAddContactOnRepository()
        //{
        //    var mockSet = new Mock<DbSet<ContactEntity>>();
        //    var data = new List<ContactEntity>
        //    {
        //        new ContactEntity { Active = true, CellphoneNumber = "9680610101", CreationDate = DateTime.MinValue, Email = "test@email.com", Id = Guid.NewGuid(), Name = "Test", RegionId = Guid.NewGuid() },
        //    }.AsQueryable();

        //    var newData = data.FirstOrDefault();
        //    newData.Id = Guid.NewGuid();

        //    var mockContext = new Mock<TechChallengeContext>();
        //    mockContext.Setup(c => c.Contacts).Returns(mockSet.Object);
        //    mockContext.Setup(c => c.Set<ContactEntity>()).Returns(mockSet.Object);

        //    var repo = new ContactRepository(mockContext.Object);
        //    await repo.AddAsync(newData);

        //    mockSet.Verify(m => m.Add(It.IsAny<ContactEntity>()), Times.Once());
        //    mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        //}
    }
}