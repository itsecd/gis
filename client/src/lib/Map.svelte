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

    async function addGeoJsonLayer(geoJsonUrl) {
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

    // Earthquake animation
    const timer = ms => new Promise(res => setTimeout(res, ms))
    export let animation_trigger: boolean;
    let earthquake_rendered: boolean = false;
    let earthquake_continues: boolean = false;

    async function earthquake_animation(ms:number) {
        console.log(`cont=${earthquake_continues} setting to true anyway`);
        console.log(`rend=${earthquake_rendered}`);
        earthquake_continues = true;
        let i:number = 0; 
        for (let feature of allFeatures) {
            if (earthquake_rendered) {
                console.log('ANIMATION: already rendered, leaving...')
                earthquake_continues = false;
                break;
            }
            console.log(i);
            console.log(feature);

            features.features.push(feature);
            //@ts-ignore
            (map.getSource('earthquakes') as GeoJSONSource).setData(features);
            i++;
            await timer(ms); 
        }

        //earthquake_rendered = true;
        earthquake_continues = false;
    }

    async function start_earthquake(ms: number) {
        console.log('vla');
        if (!earthquake_continues) {
            console.log(`added geojson_layer`);
            await addGeoJsonLayer("query_geojson.geojson");

            // Start animation
            console.log('started animation');
            await earthquake_animation(ms);
            earthquake_continues = true;
        }
        else {
            map.removeLayer('earthquakes');
            map.removeSource('earthquakes');
            // Clear loaded features
            allFeatures = [];
            features.features = [];
            // Earthquake is not rendered from now
            console.log('disable');
            earthquake_rendered = false;
            earthquake_continues = false;
        }

        console.log(`earthquake_continues=${earthquake_continues}`);
        console.log(`earthquake_rendered=${earthquake_rendered}`);
    }
    
    onMount(() => {
        map = new maplibregl.Map({
            container: 'map',
            style: 'https://api.maptiler.com/maps/5a2e698d-bf96-462a-a122-fbd86ebc7aae/style.json?key=r9k1CXRP7YCqI9zWOIjp',
            zoom: 0
        })
        map.on('load', async () => {
            //await addGeoJsonLayer("query_geojson.geojson");            
            console.log(Object.keys(allFeatures).length);   
        
            //await earthquake_animation(200);                        
        });          
    })

    $: animation_trigger, start_earthquake(200);
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

