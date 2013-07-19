using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ComponentModel.DataAnnotations;

namespace CitizenPortal.Models
{
    public class CommentEntity : TableEntity
    {
        public CommentEntity()
        {
            this.RowKey = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now.ToUniversalTime();
            this.PostedOn = DateTime.Now.ToUniversalTime();
            this.Status = "New";
            this.Notify = false;
            this.ParentType = "Dataset";
            this.Type = "General comment";
        }

        public CommentEntity(string catalog, string dataset)
            : this()
        {
            this.PartitionKey = catalog;
            this.DatasetId = dataset;
        }

        [Required]
        public string DatasetId { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Username { get; set; }

        public DateTime PostedOn { get; set; }
        public string Status { get; set; }
        public bool Notify { get; set; }
        public string ParentType { get; set; }
        public string Type { get; set; }
        public string Error { get; set; }
    }
}