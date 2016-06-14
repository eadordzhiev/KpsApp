using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KpsApp
{
    public class Properties
    {

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Crs
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    public class Geometry
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public IList<IList<IList<object>>> Coordinates { get; set; }
    }

    public class Feature
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
    }

    public class GeoJson
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("crs")]
        public Crs Crs { get; set; }

        [JsonProperty("features")]
        public IList<Feature> Features { get; set; }
    }
}