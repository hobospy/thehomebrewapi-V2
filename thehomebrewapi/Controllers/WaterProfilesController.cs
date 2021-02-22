using AutoMapper;
//using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using thehomebrewapi.Helpers;
using thehomebrewapi.Models;
using thehomebrewapi.ResourceParameters;
using thehomebrewapi.Services;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/waterProfiles")]
    public class WaterProfilesController : ExtendedControllerBase
    {
        private readonly IHomeBrewRepository _homeBrewRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public WaterProfilesController(IHomeBrewRepository homeBrewRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService)
        {
            _homeBrewRepository = homeBrewRepository ??
                throw new ArgumentNullException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        [HttpGet(Name = "GetWaterProfiles")]
        [HttpHead]
        //[HttpCacheExpiration(NoStore = true)]
        public ActionResult<IEnumerable<WaterProfileDto>> GetWaterProfiles(
            [FromQuery] WaterProfileResourceParameters waterProfileResourceParameters,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<WaterProfileDto, Entities.WaterProfile>
                (waterProfileResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var waterProfiles = _homeBrewRepository.GetWaterProfiles(waterProfileResourceParameters);

            var paginationMetaData = new
            {
                totalCount = waterProfiles.TotalCount,
                pageSize = waterProfiles.PageSize,
                currentPage = waterProfiles.CurrentPage,
                totalPages = waterProfiles.TotalPages
            };

            Response.Headers.Add(this.PAGINATION_HEADER,
                JsonSerializer.Serialize(paginationMetaData));

            var shapedWaterProfiles = _mapper.Map<IEnumerable<WaterProfileDto>>(waterProfiles).
                ShapeData(null);

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForWaterProfiles(waterProfileResourceParameters,
                    waterProfiles.HasNext,
                    waterProfiles.HasPrevious);

                var shapedWaterProfilesWithLinks = shapedWaterProfiles.Select(waterProfile =>
                {
                    var waterProfileAsDictionary = waterProfile as IDictionary<string, object>;
                    var waterProfileLinks = CreateLinksForWaterProfile((int)waterProfileAsDictionary["Id"], false);
                    waterProfileAsDictionary.Add(this.LINKS, links);
                    return waterProfileAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedWaterProfilesWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }

            return Ok(shapedWaterProfiles);
        }

        [HttpGet("{id}", Name = "GetWaterProfile")]
        public IActionResult GetWaterProfile(int id,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes,
            bool includeAdditions = false)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var waterProfile = _homeBrewRepository.GetWaterProfile(id, includeAdditions);

            if (waterProfile == null)
            {
                return NotFound();
            }

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForWaterProfile(id, includeAdditions);

                // We aren't supporting shaping of data currently so pass in an empty string
                var linkedResourceToReturn = includeAdditions ?
                    _mapper.Map<WaterProfileDto>(waterProfile).ShapeData(null) as IDictionary<string, object> :
                    _mapper.Map<WaterProfileWithoutAdditionsDto>(waterProfile).ShapeData(null) as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return includeAdditions ?
                Ok(_mapper.Map<WaterProfileDto>(waterProfile).ShapeData(null)) :
                Ok(_mapper.Map<WaterProfileWithoutAdditionsDto>(waterProfile).ShapeData(null));
        }

        [HttpPost(Name = "CreateWaterProfile")]
        public ActionResult<WaterProfileDto> CreateWaterProfile([FromBody] WaterProfileForCreationDto waterProfile,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var includeAdditions = true;
            var finalWaterProfile = _mapper.Map<Entities.WaterProfile>(waterProfile);

            _homeBrewRepository.AddWaterProfile(finalWaterProfile);
            _homeBrewRepository.Save();

            var waterProfileToReturn = _mapper.Map<Models.WaterProfileDto>(finalWaterProfile);

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForWaterProfile(waterProfileToReturn.Id, includeAdditions);

                var linkedResourceToReturn = waterProfileToReturn.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return CreatedAtRoute(
                    "GetWaterProfile",
                    new { id = linkedResourceToReturn["Id"], includeAdditions },
                    linkedResourceToReturn);
            }

            return CreatedAtRoute(
                "GetWaterProfile",
                new { id = finalWaterProfile.Id, includeAdditions },
                waterProfileToReturn);
        }

        [HttpPut("{id}", Name = "UpdateWaterProfile")]
        public ActionResult UpdateWaterProfile(int id,
            [FromBody] WaterProfileForUpdateDto waterProfile,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var includeAdditions = true;
            var waterProfileEntity = _homeBrewRepository.GetWaterProfile(id, includeAdditions);
            if (waterProfileEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(waterProfile, waterProfileEntity);
            _homeBrewRepository.UpdateWaterProfile(waterProfileEntity);
            _homeBrewRepository.Save();

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForWaterProfile(waterProfileEntity.Id, includeAdditions);

                var waterProfileToReturn = _mapper.Map<WaterProfileDto>(waterProfileEntity);
                var linkedResourceToReturn = waterProfileToReturn.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateWaterProfile")]
        public ActionResult PartialUpdateWaterProfile(int id,
            [FromBody] JsonPatchDocument<WaterProfileForUpdateDto> patchDoc,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var includeAdditions = true;
            var waterProfileEntity = _homeBrewRepository.GetWaterProfile(id, includeAdditions);
            if (waterProfileEntity == null)
            {
                return NotFound();
            }

            var waterProfileToPatch = _mapper.Map<WaterProfileForUpdateDto>(waterProfileEntity);

            patchDoc.ApplyTo(waterProfileToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(waterProfileToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(waterProfileToPatch, waterProfileEntity);
            _homeBrewRepository.UpdateWaterProfile(waterProfileEntity);
            _homeBrewRepository.Save();

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForWaterProfile(waterProfileEntity.Id, includeAdditions);

                var waterProfileToReturn = _mapper.Map<WaterProfileDto>(waterProfileEntity);
                var linkedResourceToReturn = waterProfileToReturn.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteWaterProfile")]
        public ActionResult DeleteWaterProfile(int id)
        {
            var waterProfileEntity = _homeBrewRepository.GetWaterProfile(id, false);
            if (waterProfileEntity == null)
            {
                return NotFound();
            }

            _homeBrewRepository.DeleteWaterProfile(waterProfileEntity);

            _homeBrewRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public ActionResult GetWaterProfilesOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.GET}, {this.HEAD}, {this.OPTIONS}");

            return Ok();
        }

        [HttpOptions("{id}")]
        public ActionResult GetWaterProfileOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.DELETE}, {this.GET}, {this.OPTIONS}, {this.PATCH}, {this.POST}, {this.PUT}");

            return Ok();
        }

        #region Private methods
        private IEnumerable<LinkDto> CreateLinksForWaterProfile(int id, bool includeAdditions)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetWaterProfile", new { id, includeAdditions }),
                this.SELF,
                this.GET));

            links.Add(
                new LinkDto(Url.Link("UpdateWaterProfile", new { id }),
                "update_water_profile",
                this.PUT));

            links.Add(
                new LinkDto(Url.Link("PartiallyUpdateWaterProfile", new { id }),
                "partially_update_water_profile",
                this.PATCH));

            links.Add(
                new LinkDto(Url.Link("DeleteWaterProfile", new { id }),
                "delete_water_profile",
                this.DELETE));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForWaterProfiles(
            WaterProfileResourceParameters waterProfileResourceParameters,
            bool hasNext,
            bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(CreateWaterProfilesResourceUri(
                    waterProfileResourceParameters, ETypeOfResourceUri.Current),
                this.SELF,
                this.GET));

            if (hasNext)
            {
                links.Add(
                new LinkDto(CreateWaterProfilesResourceUri(
                    waterProfileResourceParameters, ETypeOfResourceUri.NextPage),
                this.NEXT_PAGE,
                this.GET));
            }

            if (hasPrevious)
            {
                links.Add(
                new LinkDto(CreateWaterProfilesResourceUri(
                    waterProfileResourceParameters, ETypeOfResourceUri.PreviousPage),
                this.PREVIOUS_PAGE,
                this.GET));
            }

            return links;
        }

        private string CreateWaterProfilesResourceUri(
            WaterProfileResourceParameters waterProfileResourceParameters,
            ETypeOfResourceUri type)
        {
            switch(type)
            {
                case ETypeOfResourceUri.PreviousPage:
                    return Url.Link("GetWaterProfiles", new
                    {
                        orderBy = waterProfileResourceParameters.OrderBy,
                        pageNumber = waterProfileResourceParameters.PageNumber - 1,
                        pageSize = waterProfileResourceParameters.PageSize,
                        searchQuery = waterProfileResourceParameters.SearchQuery
                    });
                case ETypeOfResourceUri.NextPage:
                    return Url.Link("GetWaterProfiles", new
                    {
                        orderBy = waterProfileResourceParameters.OrderBy,
                        pageNumber = waterProfileResourceParameters.PageNumber + 1,
                        pageSize = waterProfileResourceParameters.PageSize,
                        searchQuery = waterProfileResourceParameters.SearchQuery
                    });
                case ETypeOfResourceUri.Current:
                default:
                    return Url.Link("GetWaterProfiles", new
                    {
                        orderBy = waterProfileResourceParameters.OrderBy,
                        pageNumber = waterProfileResourceParameters.PageNumber,
                        pageSize = waterProfileResourceParameters.PageSize,
                        searchQuery = waterProfileResourceParameters.SearchQuery
                    });
            }
        }
        #endregion
    }
}
