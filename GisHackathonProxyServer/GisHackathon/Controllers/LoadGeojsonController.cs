using Microsoft.AspNetCore.Mvc;
using GisHackathon.JsonClasses;

namespace ToSamaraApiServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoadGeojsonController : ControllerBase
{
    [HttpGet(Name = "GetGeojson")]
    public async Task<ActionResult<string>> Get(string eventid, string dyfi)
    {
        var httpClient = new HttpClient();

        var geoJsonObj = await httpClient.GetFromJsonAsync<GeoJsonInitialClass>($"https://earthquake.usgs.gov/fdsnws/event/1/query?eventid={eventid}&format=geojson");
        if (geoJsonObj == null)
            return NotFound();

        var updateTime = geoJsonObj.properties.products.dyfi[0].updateTime;

        var url = $"https://earthquake.usgs.gov/product/dyfi/{eventid}/us/{updateTime}/{dyfi}.geojson";
        return RedirectPermanent(url);
    }
}