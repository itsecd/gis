using Newtonsoft.Json;

namespace GisHackathon.JsonClasses
{
    public class GeoJsonInitialClass
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties
    {
        public Products products { get; set; }
    }

    public class Products
    {
        public SimpleArray[] dyfi { get; set; }
    }

    public class SimpleArray
    {
        public long updateTime { get; set; }
        public Contents contents { get; set; }
    }

    public class Contents
    {
        [JsonProperty(PropertyName = "dyfi_zip.geojson")]
        public Dyfi_zipGeoJson dyfi_zipgeojson { get; set; }

        [JsonProperty(PropertyName = "dyfi_geo_10km.geojson")]
        public Dyfi_zipGeoJson dyfi_geo_10kmgeojson { get; set; }

        [JsonProperty(PropertyName = "dyfi_geo_1km.geojson")]
        public Dyfi_zipGeoJson dyfi_geo_1kmgeojson { get; set; }
    }

    public class Dyfi_zipGeoJson
    {
        public string url { get; set; }
    }
}
