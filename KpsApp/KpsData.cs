using System.Collections.Generic;

namespace KpsApp
{
    public class KpsData
    {
        public IList<Zone> Zones { get; set; }
    }
    public class Geoposition
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class Zone
    {
        public string Name { get; set; }

        public IList<Geoposition> Positions { get; set; }

        public double ThreatLevel { get; set; }
    }
}