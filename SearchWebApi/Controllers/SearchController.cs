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
            return Ok(await _searchService.Search(searchRequest));
        }

        [HttpGet]
        public string GetString() 
        {
            return "Check for API testing";
        }
    }
}
