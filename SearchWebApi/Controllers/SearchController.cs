using Microsoft.AspNetCore.Mvc;
using SearchWebApi.Models;
using SearchWebApi.Services.Interfaces;

namespace SearchWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SearchController(ISearchService searchService) : ControllerBase
    {
        private readonly ISearchService _searchService = searchService;

        [HttpPost]
        public async Task<IActionResult> SearchResults([FromBody] SearchCriteriaRequest searchRequest) 
        {
            var results = await _searchService.Search(searchRequest);
            return StatusCode((int)results.Status, results);
        }

        [HttpGet("getsuggestions")]
        public async Task<IActionResult> GetSearchSuggestions([FromQuery] string query) 
        {
            var results = await _searchService.GetSearchSuggestions(query);
            return StatusCode((int)results.Status, results);
        }

        [HttpGet]
        public string GetString() 
        {
            return "Check for API testing";
        }
    }
}
