using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CitizenPortal.Models
{
    public class RateEntity : TableEntity
    {
        public RateEntity()
        {
            this.PartitionKey = "rates";
            this.RowKey = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now.ToUniversalTime();
            this.RateDate = DateTime.Now.ToUniversalTime();
        }

        public RateEntity(string catalog, string dataset, int rateValue)
            : base()
        {
            this.ItemKey = string.Format("{0}||{1}", catalog, dataset);
            this.RateValue = rateValue;
        }

        public string ItemKey { get; set; }
        public DateTime RateDate { get; set; }
        public int RateValue { get; set; }
        public string User { get; set; }
        public string Error { get; set; }
    }
}