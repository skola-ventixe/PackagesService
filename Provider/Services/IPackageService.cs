using Provider.Models;

namespace Provider.Services
{
    public interface IPackageService
    {
        Task<ServiceResponse<List<Package>?>> AddPackagesAsync(List<PackageRegistrationDto> package);
        Task<ServiceResponse<bool>> DeletePackage(string packageId);
        Task<ServiceResponse<List<Package>>> GetAllPackagesAsync();
        Task<ServiceResponse<List<Package>>> GetPackagesForEventAsync(string eventId);
        Task<ServiceResponse<Package>> UpdatePackage(Package updatedPackage);
    }
}