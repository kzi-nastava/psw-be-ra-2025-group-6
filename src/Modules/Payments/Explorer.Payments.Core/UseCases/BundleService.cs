using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class BundleService : IBundleService
{
    private readonly IBundleRepository _bundleRepository;
    private readonly ITourDataProvider _tourDataProvider;
    private readonly IMapper _mapper;

    public BundleService(IBundleRepository bundleRepository, ITourDataProvider tourDataProvider, IMapper mapper)
    {
        _bundleRepository = bundleRepository;
        _tourDataProvider = tourDataProvider;
        _mapper = mapper;
    }

    public BundleDto Create(long authorId, CreateBundleDto dto)
    {
        ValidateToursOwnership(authorId, dto.TourIds);

        var bundle = new Bundle(authorId, dto.Name, dto.Price, dto.TourIds);
        var result = _bundleRepository.Create(bundle);
        return _mapper.Map<BundleDto>(result);
    }

    public BundleDto Update(long authorId, long bundleId, UpdateBundleDto dto)
    {
        var bundle = _bundleRepository.Get(bundleId);
        
        if (bundle.AuthorId != authorId)
            throw new ForbiddenException("You can only update your own bundles");

        ValidateToursOwnership(authorId, dto.TourIds);

        bundle.Update(dto.Name, dto.Price, dto.TourIds);
        var result = _bundleRepository.Update(bundle);
        return _mapper.Map<BundleDto>(result);
    }

    public void Delete(long authorId, long bundleId)
    {
        var bundle = _bundleRepository.Get(bundleId);

        if (bundle.AuthorId != authorId)
            throw new ForbiddenException("You can only delete your own bundles");

        if (bundle.Status == BundleStatus.PUBLISHED)
            throw new InvalidOperationException("Published bundles cannot be deleted. Archive them instead.");

        _bundleRepository.Delete(bundleId);
    }

    public BundleDto Get(long id)
    {
        var bundle = _bundleRepository.Get(id);
        return _mapper.Map<BundleDto>(bundle);
    }

    public List<BundleDto> GetByAuthor(long authorId)
    {
        var bundles = _bundleRepository.GetByAuthor(authorId);
        return _mapper.Map<List<BundleDto>>(bundles);
    }

    public List<BundleDto> GetPublished()
    {
        var bundles = _bundleRepository.GetPublished();
        return _mapper.Map<List<BundleDto>>(bundles);
    }

    public BundleDto Publish(long authorId, long bundleId)
    {
        var bundle = _bundleRepository.Get(bundleId);

        if (bundle.AuthorId != authorId)
            throw new ForbiddenException("You can only publish your own bundles");

        var publishedToursCount = _tourDataProvider.GetPublishedToursCount(bundle.TourIds.ToList());
        
        bundle.Publish(publishedToursCount);
        var result = _bundleRepository.Update(bundle);
        return _mapper.Map<BundleDto>(result);
    }

    public BundleDto Archive(long authorId, long bundleId)
    {
        var bundle = _bundleRepository.Get(bundleId);

        if (bundle.AuthorId != authorId)
            throw new ForbiddenException("You can only archive your own bundles");

        bundle.Archive();
        var result = _bundleRepository.Update(bundle);
        return _mapper.Map<BundleDto>(result);
    }

    public double GetTotalToursPrice(List<long> tourIds)
    {
        return _tourDataProvider.GetTotalPrice(tourIds);
    }

    private void ValidateToursOwnership(long authorId, List<long> tourIds)
    {
        var toursAreOwned = _tourDataProvider.VerifyToursOwnership(authorId, tourIds);
        if (!toursAreOwned)
            throw new ForbiddenException("You can only add your own tours to a bundle");
    }
}
