using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using UserManagement.DTO;


namespace UserManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JSONFetchController : Controller
{
    private readonly HttpClient _httpClient;

    public JSONFetchController()
    {
        _httpClient = new();
        _httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("fetchMyData")]
    public async Task<IActionResult> FetchUserData()
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {

            var posts = await GetJsonPlaceholderData<List<Post>>($"posts?userId={userId}");

            var albums = await GetJsonPlaceholderData<List<Album>>($"albums?userId={userId}");

            var todos = await GetJsonPlaceholderData<List<Todo>>($"todos?userId={userId}");

            var userData = new
            {
                Posts = posts,
                Albums = albums,
                Todos = todos
            };

            return Ok(userData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private async Task<T> GetJsonPlaceholderData<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);

        var json = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(json)!;
    }
}
