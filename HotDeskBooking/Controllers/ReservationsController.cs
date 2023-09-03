using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotDeskBooking.Controllers
{
    [Authorize(Roles ="Admin, Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservations _reservationsService;
        private readonly IDesks _deskService;

        public ReservationsController(IReservations reservationsService, IDesks deskService)
        {
            _reservationsService = reservationsService;
            _deskService = deskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            try
            {             
                return Ok(await _reservationsService.GetReservations());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(DateTime startDate, DateTime endDate, int deskId)
        {
            try
            {
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return BadRequest("Authorization header is missing.");
                }
                var userId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token.");
                }
                Reservation reservation = await _reservationsService.CreateReservation(startDate, endDate, deskId, userId);
                await _reservationsService.AddReservation(reservation);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateReservation(int reservationId, int deskId)
        {
            try
            {
                await _reservationsService.UpdateReservation(reservationId, deskId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
