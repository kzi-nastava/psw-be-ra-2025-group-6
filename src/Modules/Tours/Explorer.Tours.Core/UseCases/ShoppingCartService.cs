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
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, ITourRepository tourRepository, ITourPurchaseTokenRepository tokenRepository, IMapper mapper)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _tourRepository = tourRepository;
            _tokenRepository = tokenRepository;
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

        public List<TourPurchaseTokenDto> Checkout(long touristId)
        {
            var cart = _shoppingCartRepository.GetByTouristId(touristId);
            if (cart == null || cart.Items.Count == 0)
            {
                throw new InvalidOperationException("Cannot checkout an empty cart.");
            }

            // Domain metoda kreira tokene i prazni korpu
            var tokens = cart.Checkout();

            // Èuvanje tokena u bazi
            _tokenRepository.CreateBulk(tokens);

            // Update korpe (sada prazna)
            _shoppingCartRepository.Update(cart);

            return _mapper.Map<List<TourPurchaseTokenDto>>(tokens);
        }
    }
}
