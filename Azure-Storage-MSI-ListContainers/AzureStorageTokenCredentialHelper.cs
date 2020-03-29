using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure_Storage_MSI_ListContainers
{
    public class AzureStorageTokenCredentialHelper
    {
        public TokenCredential GetTokenCredentials(AzureServiceTokenProvider azureServiceTokenProvider, string tenantId = null)
        {
            return GetTokenCredentialsAsync(azureServiceTokenProvider, tenantId).Result;
        }

        public async Task<TokenCredential> GetTokenCredentialsAsync(AzureServiceTokenProvider azureServiceTokenProvider, string tenantId = null)
        {
            return new TokenCredential(await azureServiceTokenProvider.GetAccessTokenAsync("https://storage.azure.com/", tenantId));
        }
    }
}
