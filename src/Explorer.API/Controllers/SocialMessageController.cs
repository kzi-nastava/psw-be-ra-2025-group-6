using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "registeredUserPolicy")]
    [Route("api/social-messages")]
    [ApiController]
    public class SocialMessageController : ControllerBase
    {
        private readonly ISocialMessageService _socialMessageService;

        public SocialMessageController(ISocialMessageService socialMessageService)
        {
            _socialMessageService = socialMessageService;
        }

        [HttpPost]
        public async Task<ActionResult<SocialMessageDto>> Create([FromBody] SocialMessageDto messageDto)
        {
            messageDto.SenderId = User.PersonId();
            var result = await _socialMessageService.Create(messageDto);
            return Ok(result);
        }

        [HttpGet("{otherUserId:long}")]
        public async Task<ActionResult<PagedResult<SocialMessageDto>>> GetConversation(long otherUserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var myUserId = User.PersonId();
            var result = await _socialMessageService.GetConversation(myUserId,otherUserId,page,pageSize);
            return Ok(result);
        }
    }
}
