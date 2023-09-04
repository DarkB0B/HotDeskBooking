using HotDeskBooking.Controllers;
using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotDeskTests
{
    public class LocationsControllerTests
    {
        private readonly Mock<ILocations> _mockLocationService;
        private readonly LocationsController _controller;

        public LocationsControllerTests()
        {
            _mockLocationService = new Mock<ILocations>();
            _controller = new LocationsController(_mockLocationService.Object);
        }

        [Fact]
        public async void GetAllLocations_ReturnsAllLocations()
        {
            var testLocations = new List<Location> { new Location() };
            _mockLocationService.Setup(x => x.GetLocations()).ReturnsAsync(testLocations);

            var result = await _controller.GetAllLocations();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Location>>(okResult.Value);
            Assert.Equal(testLocations, returnValue);
        }
        [Fact]
        public async void RemoveLocation_DeskExistsInLocation_ReturnsBadRequest()
        {
            int testLocationId = 1;
            _mockLocationService.Setup(x => x.DeleteLocation(It.IsAny<int>()))
                .Throws(new InvalidOperationException("Can't delete location with existing desks."));

            var result = await _controller.RemoveLocation(testLocationId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Can't delete location with existing desks.", badRequestResult.Value);
        }
        [Fact]
        public async void RemoveLocation_ValidId_ReturnsOk()
        {
            _mockLocationService.Setup(x => x.DeleteLocation(It.IsAny<int>())).Returns(Task.CompletedTask);

            var result = await _controller.RemoveLocation(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void AddLocation_ValidLocation_ReturnsOk()
        {
            var testLocation = new Location();
            _mockLocationService.Setup(x => x.AddLocation(It.IsAny<Location>())).Returns(Task.CompletedTask);

            var result = await _controller.AddLocation(testLocation);

            Assert.IsType<OkResult>(result);
        }
    }
}
