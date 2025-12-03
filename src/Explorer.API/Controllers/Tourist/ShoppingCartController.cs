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
            var touristId = long.Parse(User.Claims.First(i => i.Type == "personId").Value);
            var result = _shoppingCartService.GetByTouristId(touristId);
            return Ok(result);
        }

        [HttpPost("items/{tourId:long}")]
        public ActionResult<ShoppingCartDto> AddItem(long tourId)
        {
            var touristId = long.Parse(User.Claims.First(i => i.Type == "personId").Value);
            var result = _shoppingCartService.AddItem(touristId, tourId);
            return Ok(result);
        }

        [HttpDelete("items/{tourId:long}")]
        public ActionResult<ShoppingCartDto> RemoveItem(long tourId)
        {
            var touristId = long.Parse(User.Claims.First(i => i.Type == "personId").Value);
            var result = _shoppingCartService.RemoveItem(touristId, tourId);
            return Ok(result);
        }
    }
}
