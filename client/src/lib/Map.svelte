<script lang="ts">
    import maplibregl, {BoxZoomHandler, MouseRotateHandler, type MapLibreEvent, RasterBoundsArray, GeoJSONSource, GeoJSONFeature, MapMouseEvent } from 'maplibre-gl';
    import { onMount } from 'svelte';
    import { Protocol, PMTiles } from 'pmtiles'
    import 'maplibre-gl/dist/maplibre-gl.css';
    
    let map: maplibregl.Map;

    let protocol = new Protocol();
    maplibregl.addProtocol("pmtiles",protocol.tile);
    // pmtiles url
    let PMTILES_URL = "https://storage.yandexcloud.net/pmtest/population.pmtiles";
    const p = new PMTiles(PMTILES_URL)
    // this is so we share one instance across the JS code and the map renderer
    protocol.add(p);

    const features = {
        "type" : "FeatureCollection",
	    "name" : "CSV",
	    "features" : []
    };
    let allFeatures = [];

    async function addPopulationPmtilesLayer() {
        map.addSource('population', {
            type: "vector",
            url: "pmtiles://" + PMTILES_URL,
            attribution: '© <a href="https://openstreetmap.org">OpenStreetMap</a>'
        });
        map.addLayer(
        {
            'id' : 'population',
            'type': 'fill',
            'source-layer': 'populationfr',
            'source' : 'population',
            "layout": {
                "visibility": "visible"
            },
            'paint': {

                'fill-color': {
                    "type": "interval",
                    "stops": [
                        [
                            {
                                "zoom": 1,
                                "value": 2
                            },
                        "rgba(115, 193, 35, 1)"
                        ],
                        [
                            {
                                "zoom": 1,
                                "value": 5
                            },
                        "rgba(179, 201, 12, 1)"
                        ],
                        [
                            {
                                "zoom": 1,
                                "value": 10
                            },
                        "rgba(201, 125, 12, 1)"
                        ],
                        [
                            {
                                "zoom": 1,
                                "value": 50
                            },
                        "rgba(187, 58, 10, 1)"
                        ],
                        [
                            {
                                "zoom": 1,
                                "value": 100
                            },
                        "rgba(201, 26, 12, 1)"
                        ],
                        [
                            {
                                "zoom": 1,
                                "value": 200
                            },
                        "rgba(103, 9, 3, 1)"
                        ],
                        [
                            {
                                "zoom": 1,
                                "value": 300
                            },
                        "rgba(70, 4, 2, 1)"
                        ]
                    ],
                    "default": "rgba(161, 173, 161, 1)",
                    "property": "population"
                    },
                    "fill-antialias": false
            }
        });
    }

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
        },'water_intermittent');
    }

    // Earthquake animation
    const timer = ms => new Promise(res => setTimeout(res, ms))
    export let animation_trigger: boolean;
    let earthquake_rendered: boolean = false;
    let earthquake_continues: boolean = false;

    async function earthquake_animation(ms:number) {
        earthquake_continues = true;
        let i:number = 0; 
        for (let feature of allFeatures) {
            if (earthquake_rendered) {
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
        if (!earthquake_continues) {
            // Add earthquake layer
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
        });
        
        map.on('load', () => {
            addPopulationPmtilesLayer();
        })

        map.on('click', 'earthquakes', function (e) {
            //@ts-ignore
            var coordinates = e.features[0].geometry.coordinates.slice();
            var properties = e.features[0].properties;

            while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
                coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360;
            }

            new maplibregl.Popup({className: 'point-info-popup'})
            .setLngLat(coordinates)
            .setHTML(`<p><font size=2 color=#0f1417>${JSON.stringify(properties, null, 2)}</font></p>`)
            .addTo(map);
        });

        map.on('mouseenter', 'earthquakes', function () {
            map.getCanvas().style.cursor = 'pointer';
        });

        map.on('mouseleave', 'earthquakes', function() {
            map.getCanvas().style.cursor='';
        })
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

