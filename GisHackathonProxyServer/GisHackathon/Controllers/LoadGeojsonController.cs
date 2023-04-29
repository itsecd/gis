using Microsoft.AspNetCore.Mvc;
using GisHackathon.JsonClasses;

namespace ToSamaraApiServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoadGeojsonController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventid">string of event Id</param>
    /// <param name="name">name of url: dyfi_zip/dyfi_geo_10km/dyfi_geo_1km/FFM</param>
    /// <returns></returns>
    [HttpGet(Name = "GetGeojson")]
    public async Task<ActionResult<string>> Get(string eventid, string name)
    {
        var httpClient = new HttpClient();

        var geoJsonObj = await httpClient.GetFromJsonAsync<GeoJsonInitialClass>($"https://earthquake.usgs.gov/fdsnws/event/1/query?eventid={eventid}&format=geojson");
        if (geoJsonObj == null)
            return NotFound();

        var updateTime = geoJsonObj.properties.products.dyfi[0].updateTime;

        string url;
        if (name.ToUpper() == "FFM")
            url = "https://earthquake.usgs.gov/product/finite-fault/us6000jllz_1/us/1676951251912/FFM.geojson";
        else
            url = $"https://earthquake.usgs.gov/product/dyfi/{eventid}/us/{updateTime}/{name}.geojson";
        return RedirectPermanent(url);
    }
}