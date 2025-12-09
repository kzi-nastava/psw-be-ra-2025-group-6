using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/shopping-cart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
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
                var result = _shoppingCartService.AddItem(touristId, tourId);
                return Ok(result);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("already in the cart"))
            {
                return Conflict(new { message = ex.Message }); // 409 status ispravka (duplikati) 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 status ispravka 
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
                return NotFound(new { message = ex.Message }); // 404 status ispravka 
            }
        }
    }
}
