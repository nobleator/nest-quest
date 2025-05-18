namespace NestQuest.Services;

public class ListingServiceFactory
{
    private readonly IEnumerable<IListingService> _listingServices;

    public ListingServiceFactory(IEnumerable<IListingService> listingServices)
    {
        _listingServices = listingServices;
    }

    public IEnumerable<IListingService> GetAllServices() => _listingServices;
}
