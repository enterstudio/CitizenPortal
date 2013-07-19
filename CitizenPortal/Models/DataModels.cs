using CitizenPortal.Helper;
using System.Collections.Generic;

namespace CitizenPortal.Models
{
    public class Catalog
    {
        public string Alias { get; set; }
        public string Description { get; set; }
        public string Disclaimer { get; set; }
    }

    public class Dataset
    {
        public Catalog Catalog { get; set; }
        public string EntitySet { get; set; }
        public string Category { get; set; }
        public string Keywords { get; set; }
        public List<string> KeywordsList { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string AdditionalInfo { get; set; }
        public string GeographicCoverage { get; set; }
        public string ReleaseDate { get; set; }
        public string LastUpdateDate { get; set; }

        public string Url
        {
            get
            {
                return string.Format(OGDIHelper.DatasetUrl, this.Catalog.Alias, this.EntitySet);
            }
        }

        public string JsonLink
        {
            get
            {
                return this.Url + "?format=json";
            }
        }

        public string KmlLink
        {
            get
            {
                return this.Url + "?format=kml";
            }
        }

        public string CSVLink
        {
            get
            {
                return string.Format(OGDIHelper.DownloadCSVUrl, this.Catalog.Alias, this.EntitySet);
            }
        }

        public string DaisyLink
        {
            get
            {
                return string.Format(OGDIHelper.DownloadDaisyUrl, this.Catalog.Alias, this.EntitySet);
            }
        }
    }

    public class Data
    {
        public class DatasetProperty
        {
            public string Value { get; set; }
            public string Description { get; set; }
            public int Counter { get; set; }
        }

        public IEnumerable<Dataset> Datasets { get; set; }
        public IEnumerable<DatasetProperty> Catalogs { get; set; }
        public IEnumerable<DatasetProperty> Categories { get; set; }
        public IEnumerable<DatasetProperty> Keywords { get; set; }
    }
}