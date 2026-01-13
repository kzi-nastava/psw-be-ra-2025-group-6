using System.Globalization;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/admin/wallet")]
    [ApiController]
    public class AdminWalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly INotificationService _notificationService;

        public AdminWalletController(IWalletService walletService, INotificationService notificationService)
        {
            _walletService = walletService;
            _notificationService = notificationService;
        }

        [HttpPost("top-up")]
        public ActionResult<WalletDto> TopUp([FromBody] WalletTopUpDto dto)
        {
            try
            {
                var result = _walletService.TopUp(dto.TouristId, dto.Amount);
                var amountText = dto.Amount.ToString("0.##", CultureInfo.InvariantCulture);

                _notificationService.Create(new NotificationDto
                {
                    RecipientId = dto.TouristId,
                    SenderId = User.PersonId(),
                    Content = $"Uplaceno vam je {amountText} Adventure Coin-a",
                    ReferenceId = result.Id
                });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
