<script lang="ts">
    import maplibregl, {BoxZoomHandler, MouseRotateHandler, type MapLibreEvent, RasterBoundsArray } from 'maplibre-gl';
    import { onMount } from 'svelte';
    import 'maplibre-gl/dist/maplibre-gl.css';

    let map: maplibregl.Map;

    function addGeoJsonLayer(id, data, source, type) {
        map.addSource(source, {
          type: "geojson",
          data: data
        })
        map.addLayer(
        {
            'id' : id,
            'type': type,
            'source' : source,
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
        map.on('load', () => {
            addGeoJsonLayer("earthquakes_layer", "query_geojson.geojson", "earthquakes", "circle");
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

