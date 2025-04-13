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
        var username = await _redisService.GetDataAsync<string>("LoggedInUser");
        return Ok(username ?? "");
    }
}