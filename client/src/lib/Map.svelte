<script lang="ts">
    import maplibregl, {BoxZoomHandler, MouseRotateHandler, type MapLibreEvent, RasterBoundsArray, GeoJSONSource, GeoJSONFeature } from 'maplibre-gl';
    import { onMount } from 'svelte';
    import 'maplibre-gl/dist/maplibre-gl.css';

    let map: maplibregl.Map;

    const features = {
        "type" : "FeatureCollection",
	    "name" : "CSV",
	    "features" : []
    };
    let allFeatures = [];

    async function addGeoJsonLayer(id, geoJsonUrl, source, type) {
        const response = await fetch(geoJsonUrl);
        const featureCollection = await response.json();

        allFeatures = featureCollection.features.sort((a ,b) => {
            return a.time < b.time
        });       
        
        map.addSource('earthquakes', {
            type: "geojson",
            data: features
        });
        map.addLayer(
        {
            'id' : 'earthquakes',
            'type': 'circle',
            'source' : 'earthquakes',
            'paint': 
            {
                "circle-color": "red",
                "circle-blur": 1,
                "circle-opacity": 0.5,
                "circle-radius" : {
                    "property" : "mag",
                    "stops": [[4, 10], [8, 100]]
                }
            }
        });
    }

    onMount(() => {
        map = new maplibregl.Map({
            container: 'map',
            style: 'https://api.maptiler.com/maps/5a2e698d-bf96-462a-a122-fbd86ebc7aae/style.json?key=r9k1CXRP7YCqI9zWOIjp',
            zoom: 0
        })
        map.on('load', async () => {
            await addGeoJsonLayer("earthquakes_layer", "query_geojson.geojson", "earthquakes", "circle");            
            
            for (let feature of allFeatures) {            
                setInterval(() => {
                    features.features.push(feature);
                    (map.getSource('earthquakes') as GeoJSONSource).setData(features);                  
                }, 200);
            }                        
        });          
    })

</script>

<div id='map'>

</div>

<style>
    #map {
        position: absolute;
        left: 0;
        top: 0;
        bottom: 0;
        right: 0;
    }
</style>

