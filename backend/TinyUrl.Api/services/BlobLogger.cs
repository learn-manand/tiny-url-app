using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TinyUrl.Api.services
{
    public class BlobLogger : IAppLogger
    {
        private readonly BlobContainerClient _containerClient;

        public BlobLogger(string connectionString, string containerName)
        {
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists(); // creates container if it doesn't exist
        }

        // Existing async method
        public async Task LogAsync(string message)
        {
            try
            {
                string fileName = $"log-{DateTime.UtcNow:yyyy-MM-dd}.txt";
                BlobClient blobClient = _containerClient.GetBlobClient(fileName);

                using var memoryStream = new MemoryStream();
                if (await blobClient.ExistsAsync())
                {
                    await blobClient.DownloadToAsync(memoryStream);
                }

                memoryStream.Position = memoryStream.Length;

                string logEntry = $"{DateTime.UtcNow:O} - {message}{Environment.NewLine}";
                byte[] logBytes = Encoding.UTF8.GetBytes(logEntry);
                memoryStream.Write(logBytes, 0, logBytes.Length);

                memoryStream.Position = 0;
                await blobClient.UploadAsync(memoryStream, overwrite: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        // Synchronous wrapper for endpoints (fire-and-forget)
        public void Log(string message)
        {
            _ = LogAsync(message); // safely fire-and-forget
        }
    }
}