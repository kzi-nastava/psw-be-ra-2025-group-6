using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.API.Contracts;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/shopping-cart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITourService _tourService;

        public ShoppingCartController(IShoppingCartService shoppingCartService, ITourService tourService)
        {
            _shoppingCartService = shoppingCartService;
            _tourService = tourService;
        }

        [HttpGet]
        public ActionResult<ShoppingCartDto> Get()
        {
            if (!TryGetTouristId(out var touristId, out var errorResult)) return errorResult;
            var result = _shoppingCartService.GetByTouristId(touristId);
            return Ok(result);
        }

        [HttpPost("items/{tourId:long}")]
        public ActionResult<ShoppingCartDto> AddItem(long tourId)
        {
            try
            {
                if (!TryGetTouristId(out var touristId, out var errorResult)) return errorResult;
                var tour = _tourService.Get(tourId); 

                var result = _shoppingCartService.AddItem(touristId, tour.Id, tour.Name, tour.Price);
                return Ok(result);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("already in the cart"))
            {
                return Conflict(ApiErrorFactory.Create(HttpContext, ApiErrorCodes.Conflict, "Payment/Order creation failed.", ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiErrorFactory.Create(HttpContext, ApiErrorCodes.ValidationError, "Payment/Order creation failed.", ex.Message));
            }
        }
        
        [HttpDelete("items/{tourId:long}")]
        public ActionResult<ShoppingCartDto> RemoveItem(long tourId)
        {
            try
            {
                if (!TryGetTouristId(out var touristId, out var errorResult)) return errorResult;
                var result = _shoppingCartService.RemoveItem(touristId, tourId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiErrorFactory.Create(HttpContext, ApiErrorCodes.NotFound, "Payment/Order update failed.", ex.Message));
            }
        }

        [HttpPost("checkout")]
        public ActionResult<List<TourPurchaseTokenDto>> Checkout()
        {
            try
            {
                if (!TryGetTouristId(out var touristId, out var errorResult)) return errorResult;
                var result = _shoppingCartService.Checkout(touristId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiErrorFactory.Create(HttpContext, ApiErrorCodes.ValidationError, "Payment/Order creation failed.", ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiErrorFactory.Create(HttpContext, ApiErrorCodes.NotFound, "Payment/Order creation failed.", ex.Message));
            }
        }

        private bool TryGetTouristId(out long touristId, out ActionResult errorResult)
        {
            if (User.TryPersonId(out touristId))
            {
                errorResult = new EmptyResult();
                return true;
            }

            errorResult = Unauthorized(ApiErrorFactory.Create(
                HttpContext,
                ApiErrorCodes.AuthRequired,
                "Login required to perform payment.",
                "User is not recognized (missing tourist profile)."));
            return false;
        }
    }
}
