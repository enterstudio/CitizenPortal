using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CitizenPortal.Models
{
    public class ConfigEntity : TableEntity
    {
        public ConfigEntity()
        {
            this.PartitionKey = "CitizenPortalConfig";
        }

        public ConfigEntity(string rowKey)
            : this()
        {
            this.RowKey = rowKey;
        }

        // General

        public string DataServiceUrl { get; set; }
        public string DataBrowserUrl { get; set; }

        public string TwitterAddress { get; set; }
        public string TwitterToken { get; set; }

        public string RssAddress { get; set; }

        public string RecaptchaPublicKey { get; set; }
        public string RecaptchaPrivateKey { get; set; }

        // Global appearance

        public string WebsiteTitle { get; set; }

        public string Logo1Url { get; set; }
        public string Logo2Url { get; set; }


        // Home

        public string HomeImageUrl { get; set; }

        public string HomeTitle { get; set; }

        [AllowHtml]
        public string HomeText { get; set; }


        // License

        public string LicenseImageUrl { get; set; }
        
        [AllowHtml]
        public string LicenseText { get; set; }
        

        // Approach

        public string ApproachImageUrl { get; set; }
        
        [AllowHtml]
        public string ApproachText { get; set; }

        
        // Applications

        [AllowHtml]
        public string Applications { get; set; }


        // Contact

        public string ContactEmail { get; set; }
        public string SMTP_Host { get; set; }
        public string SMTP_Port { get; set; }
        public string SMTP_User { get; set; }
        public string SMTP_Password { get; set; }
        public bool SMTP_EnableSSL { get; set; }


        // Miscellaneous

        public string AdminIp { get; set; }
        public string Error { get; set; }
    }
}