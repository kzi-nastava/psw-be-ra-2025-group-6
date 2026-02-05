using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/clubs")]
    [ApiController]
    public class ClubOwnerController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubOwnerController(IClubService clubService)
        {
            _clubService = clubService;
        }

        /// <summary>
        /// Change club status (Active/Closed) - Owner only
        /// </summary>
        [HttpPatch("{clubId}/status")]
        public ActionResult<ClubDto> ChangeStatus(long clubId, [FromBody] ChangeClubStatusDto request)
        {
            try
            {
                var ownerId = User.PersonId();
                var result = _clubService.ChangeStatus(clubId, request.Status, ownerId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{clubId}/members")]
        public ActionResult<List<ClubMemberDto>> GetMembers(long clubId)
        {
            try
            {
                var result = _clubService.GetMembers(clubId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Invite a user to the club by username - Owner only (Club must be Active)
        /// </summary>
        [HttpPost("{clubId}/members")]
        public ActionResult<ClubMemberDto> InviteMember(long clubId, [FromBody] InviteToClubDto request)
        {
            try
            {
                var ownerId = User.PersonId();
                var result = _clubService.InviteMember(clubId, request.Username, ownerId);
                return CreatedAtAction(nameof(GetMembers), new { clubId }, result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove a member from the club - Owner only (Club must be Active)
        /// </summary>
        [HttpDelete("{clubId}/members/{memberId}")]
        public ActionResult RemoveMember(long clubId, long memberId)
        {
            try
            {
                var ownerId = User.PersonId();
                _clubService.RemoveMember(clubId, memberId, ownerId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
