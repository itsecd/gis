using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System.Text.Json;

namespace population.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PopulationController : ControllerBase
    {
        [HttpPost]
        [Route("population")]
        public IActionResult Population([FromBody] JsonElement body)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(body);

            var serializer = GeoJsonSerializer.Create();
            using var stringReader = new StringReader(json.Replace("Polygon", "MultiLineString"));
            using var jsonReader = new JsonTextReader(stringReader);
            var features = serializer.Deserialize<FeatureCollection>(jsonReader);

            if (features == null)
                return BadRequest();

            var polygonCollection = new FeatureCollection();

            foreach (var feature in features)
            {
                var coordinates = feature.Geometry.Coordinates;
                var coor = new Coordinate[feature.Geometry.Coordinates.Length + 1];
                for (var i = 0; i < feature.Geometry.Coordinates.Length; i++)
                    coor[i] = coordinates[i];

                coor[^1] = coordinates[0];

                var f = new Feature(new Polygon(new LinearRing(coor)), feature.Attributes);
                polygonCollection.Add(f);
            }

            var wktWriter = new WKTWriter();

            var population = 0;

            foreach (var feature in polygonCollection)
            {
                var geometry = feature.Geometry;

                var wkt = wktWriter.Write(geometry);
                using var sqliteConnection = new SqliteConnection("Data Source=population-fr.gpkg");
                sqliteConnection.EnableExtensions();
                sqliteConnection.Open();
                sqliteConnection.LoadExtension("mod_spatialite");

                using var command = new SqliteCommand(@"SELECT population FROM population WHERE ST_Contains(geom, ST_GeomFromText($wkt, 4326));", sqliteConnection);
                command.Parameters.AddWithValue("$wkt", wkt);

                //for every feature return all rows
                using var reader = command.ExecuteReader();

                while (reader.Read())
                    population += reader.GetInt32(0);
            }
            return Ok(population);
        }
    }
}