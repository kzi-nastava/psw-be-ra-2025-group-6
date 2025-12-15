using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public ShoppingCartDto GetByTouristId(long touristId)
        {
            var cart = _shoppingCartRepository.GetByTouristId(touristId);

            if (cart == null)
            {
                cart = new ShoppingCart(touristId);
                _shoppingCartRepository.Create(cart); 
            }
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto AddItem(long touristId, long tourId)
        {
            var cart = _shoppingCartRepository.GetByTouristId(touristId);
            if (cart == null)
            {
                cart = new ShoppingCart(touristId);
                _shoppingCartRepository.Create(cart);
            }

            var tour = _tourRepository.Get(tourId);
            cart.AddItem(tour);
            _shoppingCartRepository.Update(cart);

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto RemoveItem(long touristId, long tourId)
        {
            var cart = _shoppingCartRepository.GetByTouristId(touristId);
            if (cart == null)
            {
                throw new KeyNotFoundException("Shopping cart not found for tourist.");
            }

            cart.RemoveItem(tourId);
            _shoppingCartRepository.Update(cart);

            return _mapper.Map<ShoppingCartDto>(cart);
        }
    }
}
