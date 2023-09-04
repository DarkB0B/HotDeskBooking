using HotDeskBooking.Controllers;
using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace HotDeskTests
{
    public class DesksControllerTests
    {
        private readonly Mock<IDesks> _mockDeskService;
        private readonly Mock<IUsers> _mockUserService;
        private readonly DesksController _controller;

        public DesksControllerTests()
        {
            _mockDeskService = new Mock<IDesks>();
            _mockUserService = new Mock<IUsers>();
            _controller = new DesksController(_mockDeskService.Object, _mockUserService.Object);
        }

        [Fact]
        public async void AddNewDeskToLocation_ValidLocationId_ReturnsOk()
        {
            var testDesk = new Desk();
            _mockDeskService.Setup(x => x.CreateDesk(It.IsAny<int>())).ReturnsAsync(testDesk);

            var result = await _controller.AddNewDeskToLocation(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void RemoveDesk_PendingReservationInDesk_ReturnsBadRequest()
        {
            int testDeskId = 1;
            _mockDeskService.Setup(x => x.DeleteDesk(It.IsAny<int>()))
                .Throws(new InvalidOperationException("Can't delete desk with pending reservations."));

            var result = await _controller.RemoveDesk(testDeskId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Can't delete desk with pending reservations.", badRequestResult.Value);
        }
        
        [Fact]
        public async Task GetDesks_AdminRole_ReturnsUserInformation()
        {
            var mockDesksService = new Mock<IDesks>();
            var mockUsersService = new Mock<IUsers>();

            var desks = new List<Desk>
    {
        new Desk { Reservations = new List<Reservation>
        {
            new Reservation { User = new User { Username = "TestUser" } }
        }, IsAvailable = true }
    };

            mockDesksService.Setup(service => service.GetDesks(true))
                            .ReturnsAsync(desks);

            var controller = new DesksController(mockDesksService.Object, mockUsersService.Object);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Role, "Admin")
    };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Some auth token"; // Mock Authorization header

            var result = await controller.GetDesks(null, null, null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDesks = Assert.IsType<List<Desk>>(okResult.Value);
            Assert.NotNull(returnedDesks.First().Reservations.First().User);
            Assert.Equal("TestUser", returnedDesks.First().Reservations.First().User.Username);
        }
    }  
}