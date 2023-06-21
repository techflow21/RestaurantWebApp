using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Persistence;

namespace Presentation
{
    public class AwsStorageService : IAwsStorageService
    {
        private readonly string _awsAccessKeyId;
        private readonly string _awsSecretAccessKey;
        private readonly string _bucketName;
        private readonly string _awsRegion;
        public AwsStorageService(IOptions<AwsConfiguration> awsConfiguration)
        {
            _awsAccessKeyId = awsConfiguration.Value.AwsAccessKeyId;
            _awsSecretAccessKey = awsConfiguration.Value.AwsSecretAccessKey;
            _bucketName = awsConfiguration.Value.AwsS3BucketName;
            _awsRegion = awsConfiguration.Value.AwsRegion;
        }

        public async Task<string> SaveImageToAWSStorage(IFormFile image, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var awsS3Client = new AmazonS3Client(_awsAccessKeyId, _awsSecretAccessKey, RegionEndpoint.GetBySystemName(_awsRegion));
                var bucketName = _bucketName;
                var objectKey = "Images/" + fileName;
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = memoryStream,
                    ContentType = image.ContentType
                };
                await awsS3Client.PutObjectAsync(request);

                var imageUrl = awsS3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    Expires = DateTime.UtcNow.AddYears(700)
                });

                return imageUrl;
            }
        }



        /*private async Task<string> SaveImageToAWSStorage(IFormFile image, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Upload the image to AWS storage (e.g., Amazon S3)
                var awsS3Client = new AmazonS3Client();
                var bucketName = "your-bucket-name";
                var objectKey = "images/" + fileName;
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = memoryStream,
                    ContentType = image.ContentType
                };
                await awsS3Client.PutObjectAsync(request);

                // Return the URL of the uploaded image in AWS storage
                var imageUrl = awsS3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    Expires = DateTime.UtcNow.AddYears(700)
                });

                return imageUrl;
            }
        }*/
    }
}
