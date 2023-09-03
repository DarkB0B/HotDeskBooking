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
    public class DesksController : ControllerBase
    {
        private readonly IDesks _desksService;
        private readonly IUsers _usersService;

        public DesksController(IDesks desksService, IUsers usersService)
        {
            _desksService = desksService;
            _usersService = usersService;
        }
        [HttpGet]
        public async Task<IActionResult> GetDesks(bool? available, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (available == null || available == false) 
                {
                    if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                    {
                        return BadRequest("Authorization header is missing.");
                    }
                    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                    if (string.IsNullOrEmpty(userRole))
                    {
                        return BadRequest("Invalid token.");
                    }
                    if(userRole == "Admin")
                    {
                        return Ok(await _desksService.GetDesks(true));
                    }
                    return Ok(await _desksService.GetDesks());
                }
                else if(available == true && startDate == null)
                {
                    if(endDate == null)
                    {
                        startDate = DateTime.Now.Date;
                        endDate = DateTime.Now.Date;
                        return Ok(await _desksService.GetAvailableDesks(startDate, endDate));
                    }
                    startDate = DateTime.Now.Date;
                    return Ok(await _desksService.GetAvailableDesks(startDate, endDate));
                }
                return Ok(await _desksService.GetAvailableDesks(startDate, endDate));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("{locationId}")]
        public async Task<IActionResult> GetDesksByLocation(int locationId, bool? available, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (available == null || available == false)
                {
                    if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                    {
                        return BadRequest("Authorization header is missing.");
                    }
                    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                    if (string.IsNullOrEmpty(userRole))
                    {
                        return BadRequest("Invalid token.");
                    }
                    if (userRole == "Admin")
                    {
                        return Ok(await _desksService.GetDesksByLocation(locationId, true));
                    }
                    return Ok(await _desksService.GetDesksByLocation(locationId));
                }
                else if (available == true && startDate == null)
                {
                    if (endDate == null)
                    {
                        startDate = DateTime.Now.Date;
                        endDate = DateTime.Now.Date;
                        return Ok(await _desksService.GetAvailableDesksByLocation(locationId, startDate, endDate));
                    }
                    startDate = DateTime.Now.Date;
                    return Ok(await _desksService.GetAvailableDesksByLocation(locationId, startDate, endDate));
                }
                return Ok(await _desksService.GetAvailableDesksByLocation(locationId, startDate, endDate));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        public async Task<IActionResult> ChangeAvailability(int id)
        {
            try
            {
                return Ok(await _desksService.ChangeAvailability(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddNewDeskToLocation(int locationId)
        {
            try
            {
                Desk desk = await _desksService.CreateDesk(locationId);
                await _desksService.AddDesk(desk);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> RemoveDesk(int id)
        {
            try
            {
                await _desksService.DeleteDesk(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
