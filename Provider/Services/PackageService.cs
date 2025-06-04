using Microsoft.EntityFrameworkCore;
using Provider.Data;
using Provider.Models;

namespace Provider.Services;

public class PackageService : IPackageService
{
    private readonly PackagesDbContext _context;
    private readonly DbSet<Package> _packages;

    public PackageService(PackagesDbContext context)
    {
        _context = context;
        _packages = _context.Set<Package>();
    }

    public async Task<ServiceResponse<List<Package>>> GetAllPackagesAsync()
    {
        try
        {
            var response = await _packages.Include(p => p.Benefits).ToListAsync();
            return new ServiceResponse<List<Package>>
            {
                Success = true,
                Message = "Packages retrieved successfully.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<Package>>
            {
                Success = false,
                Error = $"Something went wrong when trying to get packages: {ex.Message}",
                Data = []
            };
        }
    }

    public async Task<ServiceResponse<List<Package>>> GetPackagesForEventAsync(string eventId)
    {
        try
        {
            var response = await _packages
            .Include(p => p.Benefits)
            .Where(p => p.EventId == eventId)
            .OrderBy(p => p.Price)
            .ToListAsync();

            return new ServiceResponse<List<Package>>
            {
                Success = true,
                Message = "Packages retrieved successfully.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<Package>>
            {
                Success = false,
                Error = $"Something went wrong when trying to get packages: {ex.Message}",
                Data = []
            };
        }
    }
    public async Task<ServiceResponse<List<Package>?>> AddPackagesAsync(List<PackageRegistrationDto> packages)
    {
        if (packages == null || packages.Count == 0)
        {
            return new ServiceResponse<List<Package>?>
            {
                Success = false,
                Error = "No packages provided.",
                Data = []
            };
        }
        List<Package> returnPackages = [];
        try
        {
            foreach (var package in packages)
            {
                var packageEntity = new Package
                {
                    Name = package.Name,
                    Price = package.Price,
                    Description = package.Description,
                    Seated = package.Seated,
                    Placement = package.Placement,
                    Benefits = package.Benefits,
                    EventId = package.EventId
                };

                foreach (var benefit in packageEntity.Benefits!)
                {
                    benefit.PackageId = packageEntity.Id;
                }

                var response = await _packages.AddAsync(packageEntity);
                returnPackages.Add(response.Entity);
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<List<Package>?>
            {
                Success = true,
                Message = "Package added successfully.",
                Data = returnPackages
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<Package>?>
            {
                Success = false,
                Error = $"Something went wrong when trying to add package: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse<Package>> UpdatePackage(Package updatedPackage)
    {
        try
        {
            var existingPackage = await _packages.FindAsync(updatedPackage.Id);
            if (existingPackage == null)
            {
                return new ServiceResponse<Package>
                {
                    Success = false,
                    Error = "Package not found."
                };
            }
            existingPackage.Name = updatedPackage.Name;
            existingPackage.Price = updatedPackage.Price;
            existingPackage.Description = updatedPackage.Description;
            existingPackage.Seated = updatedPackage.Seated;
            existingPackage.Placement = updatedPackage.Placement;
            existingPackage.Benefits = updatedPackage.Benefits;
            await _context.SaveChangesAsync();
            return new ServiceResponse<Package>
            {
                Success = true,
                Message = "Package updated successfully.",
                Data = existingPackage
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Package>
            {
                Success = false,
                Error = $"Something went wrong when trying to update package: {ex.Message}",
                Data = updatedPackage
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeletePackage(string packageId)
    {
        try
        {
            var package = await _packages.FindAsync(packageId);
            if (package == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Error = "Package not found.",
                    Data = false
                };
            }
            _packages.Remove(package);
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Package deleted successfully.",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Error = $"Something went wrong when trying to delete package: {ex.Message}",
                Data = false
            };
        }
    }
}
