using ContactList.Web.Common.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RedisController : ControllerBase
{
    private readonly IRedisService _redisService;

    public RedisController(IRedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpGet("loggedinuser")]
    public async Task<IActionResult> GetLoggedInUser()
    {
        try
        {
            var username = await _redisService.GetDataAsync<string>("LoggedInUser");
            return Ok(username ?? "");
        }
        catch (Exception ex)
        {
            // Optional: log the error
            // _logger.LogError(ex, "Error retrieving logged-in user");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving the logged-in user.", error = ex.Message });
        }
    }

}