using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
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
            var touristId = User.PersonId();
            var result = _shoppingCartService.GetByTouristId(touristId);
            return Ok(result);
        }

        [HttpPost("items/{tourId:long}")]
        public ActionResult<ShoppingCartDto> AddItem(long tourId)
        {
            try
            {
                var touristId = User.PersonId();
                var tour = _tourService.Get(tourId); 

                var result = _shoppingCartService.AddItem(touristId, tour.Id, tour.Name, tour.Price);
                return Ok(result);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("already in the cart"))
            {
                return Conflict(new { message = ex.Message }); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
        }
        
        [HttpDelete("items/{tourId:long}")]
        public ActionResult<ShoppingCartDto> RemoveItem(long tourId)
        {
            try
            {
                var touristId = User.PersonId();
                var result = _shoppingCartService.RemoveItem(touristId, tourId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("checkout")]
        public ActionResult<List<TourPurchaseTokenDto>> Checkout()
        {
            try
            {
                var touristId = User.PersonId();
                var result = _shoppingCartService.Checkout(touristId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("checkout-with-coupon")]
        public ActionResult<List<TourPurchaseTokenDto>> CheckoutWithCoupon([FromBody] CheckoutWithCouponDto request)
        {
            try
            {
                var touristId = User.PersonId();
                var result = _shoppingCartService.CheckoutWithCoupon(touristId, request.CouponCode);
                return Ok(result);
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

        [HttpPost("purchase-bundle/{bundleId:long}")]
        public ActionResult<PaymentRecordDto> PurchaseBundle(long bundleId)
        {
            try
            {
                var touristId = User.PersonId();
                var result = _shoppingCartService.PurchaseBundle(touristId, bundleId);
                return Ok(result);
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
