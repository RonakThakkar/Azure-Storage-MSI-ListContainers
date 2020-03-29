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
                // This code is to print token
                
                
                
                
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                TokenCredential tokenCredential = new AzureStorageTokenCredentialHelper().GetTokenCredentials(azureServiceTokenProvider);
                stringBuilder.AppendLine("Access Token : " + tokenCredential.Token);
                stringBuilder.AppendLine();

                CloudBlobClient cloudBlobClient = new CloudBlobClientFactory().Create(id);
                List<CloudBlobContainer> containers = cloudBlobClient.ListContainers().ToList();

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
    }
}
