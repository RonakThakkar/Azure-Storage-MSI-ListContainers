using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Azure_Storage_MSI_ListContainers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageBlobController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                // Get the initial access token and the interval at which to refresh it.

                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                var tokenAndFrequency = await AzureStorageTokenRenewerAsync(azureServiceTokenProvider, CancellationToken.None);

                // Create storage credentials using the initial token, and connect the callback function
                // to renew the token just before it expires
                TokenCredential tokenCredential = new TokenCredential(tokenAndFrequency.Token,
                                                                        AzureStorageTokenRenewerAsync,
                                                                        azureServiceTokenProvider,
                                                                        tokenAndFrequency.Frequency.Value);

                stringBuilder.AppendLine("Access Token : " + tokenCredential.Token);
                stringBuilder.AppendLine();

                StorageCredentials storageCredentials = new StorageCredentials(tokenCredential);

                Uri uri = new Uri(string.Format("https://{0}.blob.core.windows.net", id));
                CloudBlobClient cloudBlobClient = new CloudBlobClient(uri, storageCredentials);
                List<CloudBlobContainer> containers = cloudBlobClient.ListContainers().ToList();

                //CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("quickstartblobs");
                //await cloudBlobContainer.CreateIfNotExistsAsync();

                stringBuilder.AppendLine("ConnectionStatus = Connected successfully");
                stringBuilder.AppendLine("Containers Count = " + containers.Count);

                foreach (CloudBlobContainer container in containers)
                {
                    stringBuilder.AppendLine("container.Name");
                }

                return stringBuilder.ToString();

            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine(ex.Message);
            }
            return stringBuilder.ToString();
        }

        private async Task<NewTokenAndFrequency> AzureStorageTokenRenewerAsync(Object state, CancellationToken cancellationToken)
        {
            const string storageResource = "https://storage.azure.com/";

            // Use the same token provider to request a new token.
            var authResult = await ((AzureServiceTokenProvider)state).GetAuthenticationResultAsync(storageResource);

            // Renew the token 5 minutes before it expires.
            var next = (authResult.ExpiresOn - DateTimeOffset.UtcNow) - TimeSpan.FromMinutes(5);
            if (next.Ticks < 0)
            {
                next = default(TimeSpan);
                Console.WriteLine("Renewing token...");
            }

            // Return the new token and the next refresh time.
            return new NewTokenAndFrequency(authResult.AccessToken, next);
        }
    }
}
