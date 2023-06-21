using Microsoft.AspNetCore.Http;

namespace Presentation
{
    public interface ILocalStorageService
    {
        Task<string> SaveImageToLocalFileSystem(IFormFile image, string fileName);
    }
}
