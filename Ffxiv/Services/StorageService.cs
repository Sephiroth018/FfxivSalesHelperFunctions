using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Ffxiv.Common;
using Microsoft.Extensions.Options;

namespace Ffxiv.Services
{
    public class StorageService
    {
        private readonly BlobServiceClient _client;
        private readonly Config _config;

        public StorageService(IOptions<Config> options, BlobServiceClient client)
        {
            _client = client;
            _config = options.Value;
        }

        public List<string> GetAllItemBlobNames()
        {
            var container = _client.GetBlobContainerClient(_config.BlobContainer);

            var blobs = container.GetBlobs();

            return blobs.Select(b => b.Name).ToList();
        }
    }
}