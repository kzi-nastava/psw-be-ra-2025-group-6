using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tours/journals")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly IJournalService _journalService;


        public JournalController(IJournalService journalService)
        {
            _journalService = journalService;
        }

        private long GetTouristId()
        {
            return User.PersonId();
        }

        [HttpPost]
        public async Task<ActionResult<JournalDto>> Create([FromBody] JournalDto journalDto)
        {
            journalDto.TouristId = GetTouristId();

            var result = await _journalService.Create(journalDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<List<JournalDto>>> GetAllForTourist()
        {
            var touristId = GetTouristId();
            var journals = await _journalService.GetAllByTouristId(touristId);

            return Ok(journals);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<JournalDto>> GetById(long id)
        {
            var journal = await _journalService.GetById(id);

            if (journal.TouristId != GetTouristId())
            {
                return Forbid();
            }

            return Ok(journal);
        }
        [HttpPut("{id:long}")]
        public async Task<ActionResult<JournalDto>> Update(long id, [FromBody] JournalDto journalDto)
        {
            try
            {
                journalDto.Id = id;

                var existingJournal = await _journalService.GetById(id);
                if (existingJournal == null)
                    return NotFound();

                if (existingJournal.TouristId != GetTouristId())
                    return Forbid();

                journalDto.TouristId = GetTouristId();

                var result = await _journalService.Update(journalDto);
                return Ok(result);
            }
            catch (NotFoundException nf)
            {
                return NotFound(new { title = "Not Found", detail = nf.Message });
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { title = "Validation Error", detail = argEx.Message });
            }
            catch (Exception ex)
            {
                // loguj ex.Message, ex.StackTrace
                return StatusCode(500, new { title = "Internal Server Error", detail = ex.Message });
            }
        }


        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var existingJournal = await _journalService.GetById(id);
            if (existingJournal.TouristId != GetTouristId())
            {
                return Forbid();
            }
            await _journalService.Delete(id);
            return NoContent();
        }
        [HttpGet("search/{query}")]
        public async Task<ActionResult<List<JournalDto>>> Search(string query)
        {
            var touristId = GetTouristId();
            var journals = await _journalService.GetAllByTouristId(touristId);

            query = query.ToLower();

            var filtered = journals.Where(j =>
                (!string.IsNullOrEmpty(j.Name) && j.Name.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(j.Location) && j.Location.ToLower().Contains(query))
            ).ToList();

            return Ok(filtered);
        }


    }



}

