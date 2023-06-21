using Microsoft.AspNetCore.Http;

namespace Presentation
{
    public interface IAwsStorageService
    {
        Task<string> SaveImageToAWSStorage(IFormFile image, string fileName);
    }
}
