using HotDeskBooking.Controllers;
using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotDeskTests
{
    public class ReservationsControllerTests
    {
        private Mock<IReservations> mockReservationsService = new Mock<IReservations>();
        private Mock<IDesks> mockDeskService = new Mock<IDesks>();

        private ReservationsController controller;

        public ReservationsControllerTests()
        {
            controller = new ReservationsController(mockReservationsService.Object, mockDeskService.Object);
        }

        [Fact]
        public async void GetReservations_ReturnsOK()
        {
            mockReservationsService.Setup(r => r.GetReservations())
                .ReturnsAsync(new List<Reservation>());

            var result = await controller.GetReservations();

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async void CreateReservation_ReturnsOK()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);
            int deskId = 1;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Some Value";
            controller.ControllerContext.HttpContext = httpContext;

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "UserId")
        };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            httpContext.User = principal;

            mockReservationsService.Setup(r => r.CreateReservation(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Reservation());

            var result = await controller.CreateReservation(startDate, endDate, deskId);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void CreateReservation_ExistingReservationInTimePeriod_ReturnsBadRequest()
        {

            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);
            int deskId = 1;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer SomeValidToken";
            var userClaims = new List<Claim>()
    {
        new Claim(ClaimTypes.Name, "12345") 
    };

            var claimsIdentity = new ClaimsIdentity(userClaims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            httpContext.User = claimsPrincipal;

            controller.ControllerContext.HttpContext = httpContext;

            mockReservationsService.Setup(r => r.CreateReservation(startDate, endDate, deskId, It.IsAny<string>()))
                .Throws(new Exception("There is already a reservation in the selected time period."));

            var result = await controller.CreateReservation(startDate, endDate, deskId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("There is already a reservation in the selected time period.", badRequestResult.Value);
        }



        [Fact]
        public async void CreateReservation_NoAuthorizationHeader_ReturnsBadRequest()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);
            int deskId = 1;

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext = httpContext;

            var result = await controller.CreateReservation(startDate, endDate, deskId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Authorization header is missing.", badRequestResult.Value);
        }


    }
}
