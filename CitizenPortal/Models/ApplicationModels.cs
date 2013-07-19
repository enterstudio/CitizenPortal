using System.Collections.Generic;

namespace CitizenPortal.Models
{
    public class Application
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
    }

    public class Applications
    {
        public List<Application> AllApplications { get; set; }

        public Applications()
        {
            AllApplications = new List<Application>();
        }
    }
}