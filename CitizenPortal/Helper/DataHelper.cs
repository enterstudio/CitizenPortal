using CitizenPortal.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CitizenPortal.Helper
{
    public class DataHelper
    {
        private static List<Dataset> _AllDatasets = null;
        public static List<Dataset> AllDatasets
        {
            get
            {
                if (_AllDatasets != null)
                {
                    return _AllDatasets;
                }

                List<Dataset> allDatasets = CacheHelper.Get(CacheHelper.Keys.AllDatasets) as List<Dataset>;
                if (allDatasets != null)
                {
                    _AllDatasets = allDatasets;
                    return _AllDatasets;
                }

                try
                {
                    allDatasets = new List<Dataset>();
                    foreach (XElement catalogEntry in XElement.Load(OGDIHelper.CatalogListUrl).Descendants(ODataHelper.Entry).ToList())
                    {
                        XElement catalogProperties = catalogEntry.Element(ODataHelper.Content).Element(ODataHelper.Properties);

                        // Create catalog instance
                        Catalog catalog = new Catalog()
                        {
                            Alias = catalogProperties.Element(ODataHelper.Alias).Value,
                            Description = (catalogProperties.Element(ODataHelper.Description) != null ? catalogProperties.Element(ODataHelper.Description).Value : string.Empty),
                            Disclaimer = (catalogProperties.Element(ODataHelper.Disclaimer) != null ? catalogProperties.Element(ODataHelper.Disclaimer).Value : string.Empty)
                        };

                        foreach (XElement datasetEntry in XElement.Load(string.Format(OGDIHelper.CatalogMetadataUrl, catalog.Alias)).Descendants(ODataHelper.Entry).ToList())
                        {
                            XElement datasetProperties = datasetEntry.Element(ODataHelper.Content).Element(ODataHelper.Properties);

                            // Create dataset instance
                            Dataset dataset = new Dataset()
                            {
                                Catalog = catalog,
                                EntitySet = datasetProperties.Element(ODataHelper.EntitySet).Value,
                                Category = (datasetProperties.Element(ODataHelper.Category) != null ? datasetProperties.Element(ODataHelper.Category).Value : string.Empty),
                                Keywords = (datasetProperties.Element(ODataHelper.Keywords) != null ? datasetProperties.Element(ODataHelper.Keywords).Value : string.Empty),
                                Source = (datasetProperties.Element(ODataHelper.Source) != null ? datasetProperties.Element(ODataHelper.Source).Value : string.Empty),
                                Name = (datasetProperties.Element(ODataHelper.Name) != null ? datasetProperties.Element(ODataHelper.Name).Value : string.Empty),
                                Description = (datasetProperties.Element(ODataHelper.Description) != null ? datasetProperties.Element(ODataHelper.Description).Value : string.Empty),
                                AdditionalInfo = (datasetProperties.Element(ODataHelper.AdditionalInfo) != null ? datasetProperties.Element(ODataHelper.AdditionalInfo).Value : string.Empty),
                                GeographicCoverage = (datasetProperties.Element(ODataHelper.GeographicCoverage) != null ? datasetProperties.Element(ODataHelper.GeographicCoverage).Value : string.Empty),
                                ReleaseDate = (datasetProperties.Element(ODataHelper.ReleasedDate) != null ? datasetProperties.Element(ODataHelper.ReleasedDate).Value : string.Empty),
                                LastUpdateDate = (datasetProperties.Element(ODataHelper.LastUpdateDate) != null ? datasetProperties.Element(ODataHelper.LastUpdateDate).Value : string.Empty)
                            };

                            // Set keywords list
                            if (!string.IsNullOrEmpty(dataset.Keywords))
                            {
                                dataset.KeywordsList = dataset.Keywords.Split(new string[] { ",", ", " }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower().Trim()).ToList();
                            }

                            // Add dataset to collection
                            allDatasets.Add(dataset);
                        }
                    }

                    allDatasets.Sort((x, y) => string.Compare(x.Name, y.Name));
                    CacheHelper.Put(CacheHelper.Keys.AllDatasets, allDatasets);
                    _AllDatasets = allDatasets;

                    return _AllDatasets;
                }
                catch (Exception)
                { }

                return null;
            }

            set
            {
                if (value == null)
                {
                    CacheHelper.Remove(CacheHelper.Keys.AllDatasets);
                    _AllDatasets = null;
                }
                else if (!value.Equals(_AllDatasets))
                {
                    CacheHelper.Put(CacheHelper.Keys.AllDatasets, value);
                    _AllDatasets = value;
                }
            }
        }

        private static List<RateEntity> _AllRates = null;
        public static List<RateEntity> AllRates
        {
            get
            {
                if (_AllRates != null)
                {
                    return _AllRates;
                }

                List<RateEntity> allRates = CacheHelper.Get(CacheHelper.Keys.AllRates) as List<RateEntity>;
                if (allRates != null)
                {
                    _AllRates = allRates;
                    return _AllRates;
                }

                try
                {
                    TableQuery<RateEntity> query = new TableQuery<RateEntity>();
                    _AllRates = AzureHelper.GetCloudTable("RateEntry").ExecuteQuery(query).ToList();
                    CacheHelper.Put(CacheHelper.Keys.AllRates, _AllRates);

                    return _AllRates;
                }
                catch (Exception)
                { }

                return null;
            }

            set
            {
                if (value == null)
                {
                    CacheHelper.Remove(CacheHelper.Keys.AllRates);
                    _AllRates = null;
                }
                else if (!value.Equals(_AllRates))
                {
                    CacheHelper.Put(CacheHelper.Keys.AllRates, value);
                    _AllRates = value;
                }
            }
        }
    }
}
