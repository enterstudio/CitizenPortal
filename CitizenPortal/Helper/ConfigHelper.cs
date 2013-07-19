using CitizenPortal.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Web;

namespace CitizenPortal.Helper
{
    public class ConfigHelper
    {
        private static ConfigEntity _Config = null;
        public static ConfigEntity Config
        {
            get
            {
                if (_Config != null)
                {
                    return _Config;
                }

                ConfigEntity config = CacheHelper.Get(CacheHelper.Keys.Config) as ConfigEntity;
                if (config != null)
                {
                    _Config = config;
                    return _Config;
                }

                try
                {
                    string currentUrl = HttpContext.Current.Request.Url.Authority;

                    CloudTable cloudTable = AzureHelper.GetCloudTable("CitizenPortal");
                    if (!cloudTable.Exists())
                    {
                        cloudTable.CreateIfNotExists();
                    }

                    TableOperation retrieveOperation = TableOperation.Retrieve<ConfigEntity>("CitizenPortalConfig", currentUrl);
                    TableResult retrievedResult = cloudTable.Execute(retrieveOperation);
                    _Config = retrievedResult.Result as ConfigEntity;

                    if (_Config == null)
                    {
                        _Config = new ConfigEntity(currentUrl);
                        cloudTable.Execute(TableOperation.InsertOrReplace(_Config));
                    }

                    CacheHelper.Put(CacheHelper.Keys.Config, _Config);

                    return _Config;
                }
                catch (Exception)
                { }

                return null;
            }

            set
            {
                if (value == null)
                {
                    CacheHelper.Remove(CacheHelper.Keys.Config);
                    _Config = null;
                }
                else if (!value.Equals(_Config))
                {
                    CacheHelper.Put(CacheHelper.Keys.Config, value);
                    _Config = value;
                }
            }
        }
    }
}