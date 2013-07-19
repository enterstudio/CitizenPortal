using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;

namespace CitizenPortal.Helper
{
    public class AzureHelper
    {
        private static CloudStorageAccount _CloudStorageAccount = null;
        private static CloudTableClient _CloudTableClient = null;
        private static CloudBlobClient _CloudBlobClient = null;

        public static CloudTable GetCloudTable(string tableReference)
        {
            if (_CloudStorageAccount == null)
            {
                _CloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["WindowsAzureStorage"].ConnectionString);
            }

            if (_CloudTableClient == null)
            {
                _CloudTableClient = _CloudStorageAccount.CreateCloudTableClient();
            }

            return _CloudTableClient.GetTableReference(tableReference);
        }

        public static CloudBlockBlob GetCloudBlob(string containerReference, string blobReference)
        {
            if (_CloudStorageAccount == null)
            {
                _CloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["WindowsAzureStorage"].ConnectionString);
            }

            if (_CloudBlobClient == null)
            {
                _CloudBlobClient = _CloudStorageAccount.CreateCloudBlobClient();
            }

            CloudBlobContainer blobContainer = _CloudBlobClient.GetContainerReference(containerReference.ToLower());
            blobContainer.CreateIfNotExists();
            blobContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });

            return blobContainer.GetBlockBlobReference(blobReference);
        }
    }
}