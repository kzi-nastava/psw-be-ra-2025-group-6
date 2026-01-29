using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.UseCases.Administration
{
    public class BlogLocationService : IBlogLocationService
    {
        private readonly IBlogLocationRepository _locationRepository;
        private readonly IMapper _mapper;
        private const double CoordinateTolerance = 0.0001;

        public BlogLocationService(IBlogLocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public BlogLocationDto CreateOrGet(BlogLocationDto dto)
        {
            var existing = _locationRepository.FindByCoordinates(dto.Latitude, dto.Longitude, CoordinateTolerance);
            if (existing != null)
            {
                return _mapper.Map<BlogLocationDto>(existing);
            }

            var entity = _mapper.Map<BlogLocation>(dto);
            var created = _locationRepository.Create(entity);
            return _mapper.Map<BlogLocationDto>(created);
        }

        public BlogLocationDto GetById(long id)
        {
            var location = _locationRepository.GetById(id)
                ?? throw new KeyNotFoundException($"Location with Id {id} not found.");
            return _mapper.Map<BlogLocationDto>(location);
        }

        public List<BlogLocationDto> GetAll()
        {
            var entities = _locationRepository.GetAll();
            return entities.Select(e => _mapper.Map<BlogLocationDto>(e)).ToList();
        }
    }
}
