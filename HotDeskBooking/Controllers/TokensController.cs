using HotDeskBooking.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotDeskBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly IUsers _usersService;

        public TokensController(IUsers usersService)
        {
            _usersService = usersService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateToken(string username, string password)
        {
            try
            {
                string token = await _usersService.Login(username, password);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
