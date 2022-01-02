using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
    [Route("api/brews/{brewId}/tastingNotes")]
    public class TastingNotesController : ExtendedControllerBase
    {
        private readonly IHomeBrewRepository _homebrewRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public TastingNotesController(IHomeBrewRepository homeBrewRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService)
        {
            _homebrewRepository = homeBrewRepository ??
                throw new ArgumentNullException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        /// <summary>
        /// Gets a list of tasting notes associated with the supplied brew
        /// </summary>
        [HttpGet(Name = "GetTastingNotes")]
        [HttpHead]
        [ProducesResponseType(typeof(IEnumerable<TastingNoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<IDictionary<string, object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<TastingNoteDto>> GetTastingNotes(
            int brewId,
            [FromQuery] TastingNotesResourceParameters tastingNotesResourceParameters,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<TastingNoteDto, Entities.TastingNote>
                (tastingNotesResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            tastingNotesResourceParameters.BrewId = brewId;
            var tastingNotes = _homebrewRepository.GetTastingNotes(tastingNotesResourceParameters);

            var paginationMetaData = new
            {
                totalCount = tastingNotes.TotalCount,
                pageSize = tastingNotes.PageSize,
                currentPage = tastingNotes.CurrentPage,
                totalPages = tastingNotes.TotalPages
            };

            Response.Headers.Add(this.PAGINATION_HEADER,
                JsonSerializer.Serialize(paginationMetaData));

            var shapedTastingNotes = _mapper.Map<IEnumerable<TastingNoteDto>>(tastingNotes)
                                            .ShapeData(null);

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForTastingNotes(tastingNotesResourceParameters,
                                tastingNotes.HasNext,
                                tastingNotes.HasPrevious);

                var shapedTastingNotesWithLinks = shapedTastingNotes.Select(tastingNote =>
                {
                    var tastingNoteAsDictionary = tastingNote as IDictionary<string, object>;
                    var tastingNoteLinks = CreateLinksForTastingNote(brewId, (int)tastingNoteAsDictionary["Id"]);
                    tastingNoteAsDictionary.Add(this.LINKS, tastingNoteLinks);
                    return tastingNoteAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedTastingNotesWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }

            return Ok(shapedTastingNotes);
        }

        /// <summary>
        /// Gets the tasting note associated with the supplied brew and note ID
        /// </summary>
        [HttpGet("{id}", Name = "GetTastingNote")]
        [ProducesResponseType(typeof(TastingNoteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTastingNote(int brewId, int id,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            if (!_homebrewRepository.BrewExists(brewId))
            {
                return NotFound();
            }

            var tastingNote = _homebrewRepository.GetTastingNote(brewId, id);

            if (tastingNote == null)
            {
                return NotFound();
            }

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForTastingNote(brewId, id);

                var linkedResourcesToReturn = _mapper.Map<TastingNoteDto>(tastingNote).ShapeData(null) as IDictionary<string, object>;

                linkedResourcesToReturn.Add(this.LINKS, links);

                return Ok(linkedResourcesToReturn);
            }

            return Ok(_mapper.Map<TastingNoteDto>(tastingNote).ShapeData(null));
        }

        /// <summary>
        /// Create a new tasting note to be associated with the supplied brew ID
        /// </summary>
        [HttpPost(Name = "CreateTastingNote")]
        [ProducesResponseType(typeof(TastingNoteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IDictionary<string, object>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TastingNoteDto> CreateTastingNote(int brewId,
            [FromBody] TastingNoteForCreationDto tastingNote,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            if (!_homebrewRepository.BrewExists(brewId))
            {
                return NotFound();
            }

            var includeAdditionalInfo = true;
            var finalTastingNote = _mapper.Map<Entities.TastingNote>(tastingNote);

            _homebrewRepository.AddTastingNote(finalTastingNote);
            finalTastingNote.Brew = _homebrewRepository.GetBrew(brewId, includeAdditionalInfo);
            _homebrewRepository.Save();

            var tastingNoteToReturn = _mapper.Map<Models.TastingNoteDto>(finalTastingNote);

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForTastingNote(brewId, tastingNoteToReturn.Id);

                var linkedResourcesToReturn = tastingNote.ShapeData(null) as IDictionary<string, object>;

                linkedResourcesToReturn.Add(this.LINKS, links);

                return CreatedAtRoute(
                    "GetTastingNote",
                    new { brewId, id = linkedResourcesToReturn["Id"] },
                    linkedResourcesToReturn);
            }

            return CreatedAtRoute(
                    "GetTastingNote",
                    new { brewId, id = tastingNoteToReturn.Id },
                    tastingNoteToReturn);
        }

        /// <summary>
        /// Updates an existing tasting note based on the brew and tasting note ID, the tasting note
        /// model supplied is what is pushed into the persistent storage.
        /// </summary>
        [HttpPut("{id}", Name = "UpdateTastingNote")]
        [ProducesResponseType(typeof(IDictionary<string, object>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateTastingNote(int brewId,
            int id,
            [FromBody] TastingNoteForUpdateDto tastingNote,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            if (!_homebrewRepository.BrewExists(brewId))
            {
                return NotFound();
            }

            var tastingNoteEntity = _homebrewRepository.GetTastingNote(brewId, id);
            if (tastingNoteEntity == null)
            {
                return NotFound();
            }

            // TODO: Check BrewId/Brew isn't skipped
            _mapper.Map(tastingNote, tastingNoteEntity);
            _homebrewRepository.UpdateTastingNote(tastingNoteEntity);
            _homebrewRepository.Save();

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForTastingNote(brewId, id);

                var linkedResourcesToReturn = _mapper.Map<TastingNoteDto>(tastingNote)
                                                .ShapeData(null) as IDictionary<string, object>;

                linkedResourcesToReturn.Add(this.LINKS, links);

                return Ok(linkedResourcesToReturn);
            }

            return NoContent();
        }

        /// <summary>
        /// Updates an existing tasting note based on the brew and tasting note ID, only
        /// the properties supplied will be updated
        /// </summary>
        [HttpPatch("{id}", Name = "PartiallyUpdateTastingNote")]
        [ProducesResponseType(typeof(IDictionary<string, object>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult PartiallyUpdateTastingNote(int brewId,
            int id,
            [FromBody] JsonPatchDocument<TastingNoteForUpdateDto> patchDoc,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            if (!_homebrewRepository.BrewExists(brewId))
            {
                return NotFound();
            }

            var tastingNoteEntity = _homebrewRepository.GetTastingNote(brewId, id);
            if (tastingNoteEntity == null)
            {
                return NotFound();
            }

            var tastingNoteToPatch = _mapper.Map<TastingNoteForUpdateDto>(tastingNoteEntity);

            patchDoc.ApplyTo(tastingNoteToPatch, ModelState);

            if (!_homebrewRepository.BrewExists(brewId))
            {
                ModelState.AddModelError(
                            "Brew ID",
                            $"The associated brew id [{brewId}] for the tasting note must be valid.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(tastingNoteToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(tastingNoteToPatch, tastingNoteEntity);
            _homebrewRepository.UpdateTastingNote(tastingNoteEntity);
            _homebrewRepository.Save();

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForTastingNote(brewId, id);

                var linkedResourcesToReturn = _mapper.Map<TastingNoteDto>(tastingNoteEntity)
                                                .ShapeData(null) as IDictionary<string, object>;

                linkedResourcesToReturn.Add(this.LINKS, links);

                return Ok(linkedResourcesToReturn);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes the tasting note with the supplied brew and tasting note ID
        /// </summary>
        [HttpDelete("{id}", Name = "DeleteTastingNote")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteTastingNote(int brewId, int id)
        {
            if (!_homebrewRepository.BrewExists(brewId))
            {
                return NotFound();
            }

            var tastingNoteEntity = _homebrewRepository.GetTastingNote(brewId, id);
            if (tastingNoteEntity == null)
            {
                return NotFound();
            }

            _homebrewRepository.DeleteTastingNote(tastingNoteEntity);
            _homebrewRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public ActionResult GetTastingNotesOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.GET}, {this.HEAD}, {this.OPTIONS}");
            return Ok();
        }

        [HttpOptions("{id}")]
        public ActionResult GetTastingNoteOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.DELETE}, {this.GET}, {this.OPTIONS}, {this.PATCH}, {this.POST}, {this.PUT}");
            return Ok();
        }

        #region Private functions

        private string CreateTastingNotesResourceUri(
            TastingNotesResourceParameters tastingNotesResourceParameters,
            ETypeOfResourceUri type)
        {
            var linkProps = new ExpandoObject() as IDictionary<string, Object>;

            switch (type)
            {
                case ETypeOfResourceUri.PreviousPage:
                    linkProps.Add("orderBy", tastingNotesResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", tastingNotesResourceParameters.PageNumber - 1);
                    linkProps.Add("pageSize", tastingNotesResourceParameters.PageSize);
                    linkProps.Add("searchQuery", tastingNotesResourceParameters.SearchQuery);

                    return Url.Link("GetTastingNotes", linkProps);

                case ETypeOfResourceUri.NextPage:
                    linkProps.Add("orderBy", tastingNotesResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", tastingNotesResourceParameters.PageNumber + 1);
                    linkProps.Add("pageSize", tastingNotesResourceParameters.PageSize);
                    linkProps.Add("searchQuery", tastingNotesResourceParameters.SearchQuery);

                    return Url.Link("GetTastingNotes", linkProps);

                case ETypeOfResourceUri.Current:
                default:
                    linkProps.Add("orderBy", tastingNotesResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", tastingNotesResourceParameters.PageNumber);
                    linkProps.Add("pageSize", tastingNotesResourceParameters.PageSize);
                    linkProps.Add("searchQuery", tastingNotesResourceParameters.SearchQuery);

                    return Url.Link("GetTastingNotes", linkProps);
            }
        }

        private IEnumerable<LinkDto> CreateLinksForTastingNote(int brewId, int id)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetTastingNote", new { brewId, id }),
                this.SELF,
                this.GET));

            links.Add(
                new LinkDto(Url.Link("UpdateTastingNote", new { brewId, id }),
                "update_tastingNote",
                this.PUT));

            links.Add(
                new LinkDto(Url.Link("PartiallyUpdateTastingNote", new { brewId, id }),
                "partially_update_tastingNote",
                this.PATCH));

            links.Add(
                new LinkDto(Url.Link("DeleteTastingNote", new { brewId, id }),
                "delete_tastingNote",
                this.DELETE));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTastingNotes(
            TastingNotesResourceParameters tastingNotesResourceParameters,
            bool hasNext,
            bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(CreateTastingNotesResourceUri(
                    tastingNotesResourceParameters, ETypeOfResourceUri.Current),
                    this.SELF,
                    this.GET));

            if (hasNext)
            {
                links.Add(
                new LinkDto(CreateTastingNotesResourceUri(
                    tastingNotesResourceParameters, ETypeOfResourceUri.NextPage),
                    this.NEXT_PAGE,
                    this.GET));
            }

            if (hasPrevious)
            {
                links.Add(
                new LinkDto(CreateTastingNotesResourceUri(
                    tastingNotesResourceParameters, ETypeOfResourceUri.PreviousPage),
                    this.PREVIOUS_PAGE,
                    this.GET));
            }

            return links;
        }

        #endregion Private functions
    }
}