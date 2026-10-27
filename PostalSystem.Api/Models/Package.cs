using System;
using System.Collections.Generic;

namespace PostalSystem.Api.Models
{
    public class Package
    {
        public Guid TrackingGuid { get; set; } = Guid.NewGuid();
        public string DeliveryOTP { get; set; } = string.Empty;
        public int FinalDestinationCityId { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public bool IsFlaggedAsFragile { get; set; } = false;
        public PackageStatus CalculatedStatus { get; set; } = PackageStatus.Registered;
        public bool IsCanceled { get; set; } = false;
        
        public City? FinalDestinationCity { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}