
using Microsoft.AspNetCore.Http;

namespace Presentation
{
    public class LocalStorageService : ILocalStorageService
    {
        public LocalStorageService() { }

        public async Task<string> SaveImageToLocalFileSystem(IFormFile image, string fileName)
        {
            var imagePath = Path.Combine("Assets/Images", fileName);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), imagePath);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return fullPath;
        }
    }
}
