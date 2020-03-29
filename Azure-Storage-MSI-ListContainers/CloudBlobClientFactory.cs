using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure_Storage_MSI_ListContainers.Controllers
{
    public class CloudBlobClientFactory
    {
        public CloudBlobClient Create(string storageAccountName)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            Uri uri = new Uri(string.Format("https://{0}.blob.core.windows.net", storageAccountName));
            CloudBlobClient cloudBlobClient = new CloudBlobClient(uri, GetStorageCredentials(azureServiceTokenProvider));
            return cloudBlobClient;
        }

        public StorageCredentials GetStorageCredentials(AzureServiceTokenProvider azureServiceTokenProvider, string tenantId = null)
        {
            return GetStorageCredentialsAsync(azureServiceTokenProvider, tenantId).Result;
        }

        public async Task<StorageCredentials> GetStorageCredentialsAsync(AzureServiceTokenProvider azureServiceTokenProvider, string tenantId = null)
        {
            return new StorageCredentials(await GetTokenCredentialsAsync(azureServiceTokenProvider, tenantId));
        }

        public async Task<TokenCredential> GetTokenCredentialsAsync(AzureServiceTokenProvider azureServiceTokenProvider, string tenantId = null)
        {
            return new TokenCredential(await azureServiceTokenProvider.GetAccessTokenAsync("https://storage.azure.com/", tenantId));
        }
    }
}
