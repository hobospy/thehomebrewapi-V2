using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using thehomebrewapi.Controllers;
using thehomebrewapi.Entities;
using thehomebrewapi.Services;

namespace thehomebrewapi.tests
{
    public class brewControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetBrew_InvalidId_ReturnsNotFound()
        {
            var invalidId = 5;
            var homeBrewRepositoryMock = new Mock<IHomeBrewRepository>();
            var mapperMock = new Mock<IMapper>();
            var brewsController = new BrewsController(homeBrewRepositoryMock.Object, mapperMock.Object);

            homeBrewRepositoryMock.Setup(hbr => hbr.GetBrew(It.IsAny<int>(), false)).Returns((Brew)null);

            var result = brewsController.GetBrew(invalidId);

            var notFoundObjectResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundObjectResult);
        }

        [Test]
        public void GetBrew_ValidId_ReturnsOk()
        {
            var brew = new Brew()
            {
                ID = 1,
                ABV = 2.3,
                BrewDate = new DateTime(2021, 1, 1),
                BrewingNotes = "No notes",
                BrewedState = 0,
                Name = "New brew",
                Rating = 3.2
            };

            var homeBrewRepositoryMock = new Mock<IHomeBrewRepository>();
            var mapperMock = new Mock<IMapper>();
            var brewsController = new BrewsController(homeBrewRepositoryMock.Object, mapperMock.Object);

            homeBrewRepositoryMock.Setup(hbr => hbr.GetBrew(It.IsAny<int>(), false)).Returns(brew);

            var result = brewsController.GetBrew(5);

            var okObjectResult = result as OkObjectResult;
            Assert.AreEqual(brew, okObjectResult.Value);
        }

        [Test]
        public void GetBrew_ValidIdNoTastingNotes_ReturnsOkWithBrewAndNoTastingNotes()
        {
            var brew = new Brew()
            {
                ID = 1,
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
                        ID = 1,
                        BrewID = 1,
                        Date = new DateTime(2021, 1, 4),
                        Note = "First new note"
                    },
                    new TastingNote
                    {
                        ID = 2,
                        BrewID = 1,
                        Date = new DateTime(2021, 1, 12),
                        Note = "Second new note"
                    }
                }
            };

            var homeBrewRepositoryMock = new Mock<IHomeBrewRepository>();
            var mapperMock = new Mock<IMapper>();
            var brewsController = new BrewsController(homeBrewRepositoryMock.Object, mapperMock.Object);

            homeBrewRepositoryMock.Setup(hbr => hbr.GetBrew(It.IsAny<int>(), false)).Returns(brew);

            var result = brewsController.GetBrew(5);

            var okObjectResult = result as OkObjectResult;
            Assert.AreEqual(brew, okObjectResult.Value);
        }
    }
}