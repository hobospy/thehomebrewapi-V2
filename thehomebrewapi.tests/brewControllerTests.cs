using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using thehomebrewapi.Contexts;
using thehomebrewapi.Controllers;
using thehomebrewapi.Entities;
using thehomebrewapi.Helpers;
using thehomebrewapi.Models;
using thehomebrewapi.Services;

namespace thehomebrewapi.tests
{
    public class brewControllerTests
    {
        private const string VALID_MEDIA_TYPE = "application/json";
        private const int VALID_ID = 1;
        private const int INVALID_ID = 999;

        private readonly Brew _brew = new Brew()
        {
            Id = VALID_ID,
            ABV = 2.3,
            BrewDate = new DateTime(2021, 1, 1),
            BrewingNotes = "No notes",
            BrewedState = 0,
            Name = "New brew",
            Rating = 3.2,
            TastingNotes = new List<TastingNote>()
            {
                new TastingNote
                {
                    Id = 1,
                    BrewID = 1,
                    Date = new DateTime(2021, 1, 4),
                    Note = "First new note"
                },
                new TastingNote
                {
                    Id = 2,
                    BrewID = 1,
                    Date = new DateTime(2021, 1, 12),
                    Note = "Second new note"
                }
            }
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetBrew_InvalidId_ReturnsNotFound()
        {
            var homeBrewRepositoryMock = new Mock<IHomeBrewRepository>();
            var mapperMock = new Mock<IMapper>();
            var propertyMappingServiceMock = new Mock<IPropertyMappingService>();
            var brewsController = new BrewsController(homeBrewRepositoryMock.Object, mapperMock.Object, propertyMappingServiceMock.Object);

            var result = brewsController.GetBrew(INVALID_ID, VALID_MEDIA_TYPE);
            var notFoundObjectResult = result as NotFoundResult;

            Assert.IsNotNull(notFoundObjectResult);
        }

        [Test]
        public void GetBrew_InvalidMediaTypes_ReturnsBadRequest()
        {
            var homeBrewRepositoryMock = new Mock<IHomeBrewRepository>();
            var mapperMock = new Mock<IMapper>();
            var propertyMappingServiceMock = new Mock<IPropertyMappingService>();
            var brewsController = new BrewsController(homeBrewRepositoryMock.Object, mapperMock.Object, propertyMappingServiceMock.Object);

            var result = brewsController.GetBrew(INVALID_ID, "BadMediaType");
            var badRequestResult = result as BadRequestResult;

            Assert.IsNotNull(badRequestResult);
        }

        [Test]
        public void GetBrew_ValidId_ReturnsOk()
        {
            var brewDto = new BrewDto()
            {
                Id = _brew.Id,
                Name = _brew.Name,
                BrewDate = _brew.BrewDate,
                BrewedState = _brew.BrewedState,
                BrewingNotes = _brew.BrewingNotes,
                ABV = _brew.ABV,
                Rating = _brew.Rating
            };
            var expandoBrewDto = new ExpandoObject();
            var dict = (IDictionary<string, object>) expandoBrewDto;
            foreach(var property in brewDto.GetType().GetProperties())
                dict.Add(property.Name, property.GetValue(brewDto));

            var homeBrewRepositoryMock = new Mock<IHomeBrewRepository>();
            var mapperMock = new Mock<IMapper>();
            var propertyMappingServiceMock = new Mock<IPropertyMappingService>();
            var dataManipulationMock = new Mock<IDataManipulation>();
            var brewsController = new BrewsController(homeBrewRepositoryMock.Object, mapperMock.Object, propertyMappingServiceMock.Object);

            homeBrewRepositoryMock.Setup(hbr => hbr.GetBrew(It.Is<int>(i => i == VALID_ID), false)).Returns(_brew);
            dataManipulationMock.Setup(dm => dm.ShapeData<BrewWithoutAdditionalInfoDto>(It.IsAny<BrewWithoutAdditionalInfoDto>(), It.IsAny<string>())).Returns(expandoBrewDto);
            DataManipulationExtensions.Implementation = dataManipulationMock.Object;

            var result = brewsController.GetBrew(VALID_ID, VALID_MEDIA_TYPE);

            var okObjectResult = result as OkObjectResult;

            Assert.IsNotNull(okObjectResult);
        }

        [Test]
        public void GetBrew_ValidIdNoTastingNotes_ReturnsOkWithBrewAndNoTastingNotes()
        {
            var brewDto = new BrewDto()
            {
                Id = _brew.Id,
                Name = _brew.Name,
                BrewDate = _brew.BrewDate,
                BrewedState = _brew.BrewedState,
                BrewingNotes = _brew.BrewingNotes,
                ABV = _brew.ABV,
                Rating = _brew.Rating
            };

            var expandoBrewDto = new ExpandoObject();
            var dict = (IDictionary<string, object>)expandoBrewDto;
            foreach (var property in brewDto.GetType().GetProperties())
                dict.Add(property.Name, property.GetValue(brewDto));

            var homeBrewRepositoryMock = new Mock<IHomeBrewRepository>();
            var mapperMock = new Mock<IMapper>();
            var propertyMappingServiceMock = new Mock<IPropertyMappingService>();
            var dataManipulationMock = new Mock<IDataManipulation>();
            var brewsController = new BrewsController(homeBrewRepositoryMock.Object, mapperMock.Object, propertyMappingServiceMock.Object);

            homeBrewRepositoryMock.Setup(hbr => hbr.GetBrew(It.Is<int>(i => i == VALID_ID), false)).Returns(_brew);
            dataManipulationMock.Setup(dm => dm.ShapeData<BrewWithoutAdditionalInfoDto>(It.IsAny<BrewWithoutAdditionalInfoDto>(), It.IsAny<string>())).Returns(expandoBrewDto);
            DataManipulationExtensions.Implementation = dataManipulationMock.Object;

            var result = brewsController.GetBrew(VALID_ID, VALID_MEDIA_TYPE);

            var okObjectResult = result as OkObjectResult;
            dataManipulationMock.Verify(dm => dm.ShapeData(It.IsAny<BrewWithoutAdditionalInfoDto>(), It.IsAny<string>()), Times.AtLeastOnce());
            Assert.AreEqual(expandoBrewDto, okObjectResult?.Value);
        }

        [Test]
        public void GetBrew_ValidIdTastingNotes_ReturnsOkWithBrewAndTastingNotes()
        {
            var brewDto = new BrewDto()
            {
                Id = _brew.Id,
                Name = _brew.Name,
                BrewDate = _brew.BrewDate,
                BrewedState = _brew.BrewedState,
                BrewingNotes = _brew.BrewingNotes,
                ABV = _brew.ABV,
                Rating = _brew.Rating,
                TastingNotes = new List<TastingNoteDto>(),
                Recipe = new RecipeDto()
            };
            var expandoBrewDto = new ExpandoObject();
            var dict = (IDictionary<string, object>)expandoBrewDto;
            foreach (var property in brewDto.GetType().GetProperties())
                dict.Add(property.Name, property.GetValue(brewDto));

            var homeBrewRepositoryMock = new Mock<IHomeBrewRepository>();
            var mapperMock = new Mock<IMapper>();
            var propertyMappingServiceMock = new Mock<IPropertyMappingService>();
            var dataManipulationMock = new Mock<IDataManipulation>();
            var brewsController = new BrewsController(homeBrewRepositoryMock.Object, mapperMock.Object, propertyMappingServiceMock.Object);

            homeBrewRepositoryMock.Setup(hbr => hbr.GetBrew(It.Is<int>(i => i == VALID_ID), true)).Returns(_brew);
            dataManipulationMock.Setup(dm => dm.ShapeData<BrewDto>(It.IsAny<BrewDto>(), It.IsAny<string>())).Returns(expandoBrewDto);
            DataManipulationExtensions.Implementation = dataManipulationMock.Object;

            var result = brewsController.GetBrew(VALID_ID, VALID_MEDIA_TYPE, true);

            var okObjectResult = result as OkObjectResult;
            dataManipulationMock.Verify(dm => dm.ShapeData(It.IsAny<BrewDto>(), It.IsAny<string>()), Times.AtLeastOnce());
            Assert.AreEqual(expandoBrewDto, okObjectResult.Value);
        }
    }
}