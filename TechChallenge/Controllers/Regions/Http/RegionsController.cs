using Domain.Contact.Entity;
using Domain.Contact.Service;
using Domain.Region.Entity;
using Domain.Region.Service;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechChallenge.Controllers.Regions.Dto;
using TechChallenge.Helpers.Mappers;
using TechChallenge1.Controllers.Contacts.Dto;
using TechChallenge1.Helpers.Mappers;

namespace TechChallenge.Controllers.Regions.Http
{
    [Route("api/[controller]")]
    [SwaggerTag("Endpoints to manage regions")]
    [Produces("application/json")]
    public class RegionsController : Controller
    {
        private readonly IContactService _contactsService;
        private readonly IRegionService _regionService;

        public RegionsController(IContactService contactsService,
            IRegionService regionService)
        {
            _contactsService = contactsService;
            _regionService = regionService;
        }

        /// <summary>
        /// Add a new region
        /// </summary>
        /// <param name="dto">Region DTO</param>
        /// <response code="201">Region created with success</response>
        /// <response code="400">Bad Request</response>
        [HttpPost("")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddRegion([FromBody] RegionPostDto dto)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var region = await _regionService.GetByDDD(dto.DDD);

            if (region != null)
                return StatusCode(StatusCodes.Status400BadRequest, "region already exist");

            var regionEntity = RegionMapper.MapRegionPost(dto);

            await _regionService.AddAsync(regionEntity);

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Get all regions
        /// </summary>
        /// <response code="200">All Regions</response>
        /// <response code="400">Bad Request</response>
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<RegionEntity>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetContactsByRegion()
        {
            var region = await _regionService.GetAllRegions();

            return StatusCode(StatusCodes.Status200OK, region);
        }
    }
}
