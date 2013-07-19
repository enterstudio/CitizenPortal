using System.Xml.Linq;

namespace CitizenPortal.Helper
{
    public class ODataHelper
    {
        public static readonly XNamespace Atom = XNamespace.Get("http://www.w3.org/2005/Atom");
        public static readonly XNamespace M = XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
        public static readonly XNamespace D = XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices");

        public static XName Entry
        {
            get
            {
                return Atom + "entry";
            }
        }

        public static XName Content
        {
            get
            {
                return Atom + "content";
            }
        }

        public static XName Properties
        {
            get
            {
                return M + "properties";
            }
        }

        public static XName Alias
        {
            get
            {
                return D + "alias";
            }
        }

        public static XName Category
        {
            get
            {
                return D + "category";
            }
        }

        public static XName EntitySet
        {
            get
            {
                return D + "entityset";
            }
        }

        public static XName Keywords
        {
            get
            {
                return D + "keywords";
            }
        }

        public static XName Source
        {
            get
            {
                return D + "source";
            }
        }

        public static XName Name
        {
            get
            {
                return D + "name";
            }
        }

        public static XName Description
        {
            get
            {
                return D + "description";
            }
        }

        public static XName Disclaimer
        {
            get
            {
                return D + "disclaimer";
            }
        }

        public static XName AdditionalInfo
        {
            get
            {
                return D + "additionalinfo";
            }
        }

        public static XName GeographicCoverage
        {
            get
            {
                return D + "geographiccoverage";
            }
        }

        public static XName ReleasedDate
        {
            get
            {
                return D + "releaseddate";
            }
        }

        public static XName LastUpdateDate
        {
            get
            {
                return D + "lastupdatedate";
            }
        }
    }
}