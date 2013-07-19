using System.Configuration;

namespace CitizenPortal.Helper
{
    public class OGDIHelper
    {
        public const string FilterParam = "&$filter={0} {1} {2}";

        public static string CatalogListUrl
        {
            get
            {
                return ConfigHelper.Config.DataServiceUrl + "/AvailableEndpoints?$top=all&$orderby=alias";
            }
        }

        public static string CatalogMetadataUrl
        {
            get
            {
                return ConfigHelper.Config.DataServiceUrl + "/{0}/TableMetadata?$top=all";
            }
        }

        public static string DatasetUrl
        {
            get
            {
                return ConfigHelper.Config.DataServiceUrl + "/{0}/{1}";
            }
        }

        public static string DownloadCSVUrl
        {
            get
            {
                return ConfigHelper.Config.DataBrowserUrl + "/DataBrowser/DownloadCsv?container={0}&entitySet={1}&filter=NOFILTER";
            }
        }

        public static string DownloadDaisyUrl
        {
            get
            {
                return ConfigHelper.Config.DataBrowserUrl + "/DataBrowser/Download?container={0}&entitySet={1}&filter=NOFILTER";
            }
        }
    }
}