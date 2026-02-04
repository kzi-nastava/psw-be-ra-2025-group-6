using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/community/membership-requests")]
    public class MembershipRequestController : ControllerBase
    {
        private readonly IMembershipRequestService _requestService;

        public MembershipRequestController(IMembershipRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost("{clubId:long}")]
        public ActionResult<ClubMembershipRequestDto> SendRequest(long clubId)
        {
            var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            var result = _requestService.SendRequest(clubId, userId);
            return Ok(result);
        }

        [HttpDelete("{id:long}/withdraw")]
        public ActionResult WithdrawRequest(long id)
        {
            var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            _requestService.WithdrawRequest(id, userId);
            return Ok();
        }

        [HttpGet("club/{clubId:long}")]
        public ActionResult<List<ClubMembershipRequestDto>> GetPendingRequests(long clubId)
        {
            var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            var result = _requestService.GetPendingRequestsByClub(clubId, userId);
            return Ok(result);
        }

        [HttpPost("{id:long}/accept")]
        public ActionResult AcceptRequest(long id)
        {
            var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            _requestService.AcceptRequest(id, userId);
            return Ok();
        }

        [HttpPost("{id:long}/reject")]
        public ActionResult RejectRequest(long id)
        {
            var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            _requestService.RejectRequest(id, userId);
            return Ok();
        }
    }
}