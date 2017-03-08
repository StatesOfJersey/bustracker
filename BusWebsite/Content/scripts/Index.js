
var contextMenuShowing = false;
var cachedData;
var userLocation = "";
var stopRadius;
var svg;
var map;
var isZoomed = false;
var busShape = 'M 11.433037,3.0577932 V 2.6823471 c 3.566728,0.1877235 6.57029,1.6895035 8.822959,4.1298975 L 11.057591,-8.9564504 1.859187,6.8122446 c 2.2526706,-2.440394 5.443954,-3.942174 8.82296,-4.1298975 V 2.8700706 C 4.1118576,3.2455149 -1.1443739,8.5017464 -1.1443739,15.259759 c 0,6.758013 5.4439541,12.201967 12.2019649,12.201967 6.758012,0 12.201967,-5.443954 12.201967,-12.201967 0,-6.7580126 -5.256231,-12.0142441 -11.826521,-12.2019658 z';
var myTimer;


function init() {

    loadBusStopRadius();


    // MAPBOX (Free for 50,000 views/mon   -    https://www.mapbox.com/pricing/)
    L.mapbox.accessToken = 'pk.eyJ1Ijoic29qYnVzYXBwIiwiYSI6ImNpZjQ5bThmYzAwbjR0Zmx5cjRsYjltZjgifQ.a4TmSDGNTztWHCRq23HzAg';
    var tiles = L.tileLayer('https://api.mapbox.com/v4/sojbusapp.o10pi839/{z}/{x}/{y}.png?access_token=' + L.mapbox.accessToken);

    // Arcgis online (SoJ)
    //var tiles = L.tileLayer('http://tiles.arcgis.com/tiles/ICNdZmCzUU02e5fT/arcgis/rest/services/JerseyBasemap/MapServer/tile/{z}/{y}/{x}');
    // var tiles = L.tileLayer('http://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}');

    // OPEN STREET MAP (no limits)
    //var tiles = L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png');

    /* Set map properties */
    var southWest = L.latLng(49.161755, -2.270375), //Bottom left
     northEast = L.latLng(49.273487, -1.996305), //Top right

    //var northEast = L.latLng(49.474155, -1.601826);
   // var southWest = L.latLng(49.049352, -2.486225);
    bounds = L.latLngBounds(southWest, northEast);

    map = L.map('map', {
        maxBounds: bounds,
        maxZoom: 18,
        minZoom: 10, //12
        zoomControl: false, // Show on right
        layer: tiles
    }).addLayer(tiles);
    L.control.scale().addTo(map);
    L.control.zoom({ position: 'topright' }).addTo(map);
    map.fitBounds(bounds);
    map.setZoom(12);
    map.attributionControl.setPrefix("<a href='mailto:sojwebteam@gov.je?subject=Bus live tracking feedback'>Send feedback</a>")


    /* Initialize the SVG layer */
    map._initPathRoot();

    ///* Create map SVG */
    svg = d3.select("#map").select("svg"),
    g = svg.append("g");


    //Get the bus routes from API and then set the navigation with live bus routes, colours and default selected items
    localStorage.removeItem('busRoutes');
    var prom = getBusRoutes();
    $.when(prom).then(function (buses) {
        localStorage.setItem('busRoutes', JSON.stringify(buses));
        setBusRoutes(getUserPreferencesForRoutesFromStorageOrCheckboxes());
        drawRoutes(buses);
    });

    getUserLocation(true);
    var interval = loadUpdateInterval();

    clearInterval(myTimer);
    myTimer = setInterval(function () { getBusLocationData(interval.duration, true); }, interval.update);

    map.on('moveend', function () {
        if (map.getZoom() >= 16 && stopRadius != 0) {
            userLocation = "&lat=" + map.getCenter().lat + "&lon=" + map.getCenter().lng;
            getBusLocationData(0, true);
            isZoomed = true;
        }
        else {
            isZoomed = false;
        }
    });

    map.on("viewreset", function () {
        // Remove all buses from cache to stop buses jumping into sea on zoom
        svg.selectAll('.busItem').remove();
        svg.selectAll('.stopItem').remove();
        console.log(userLocation);
        var prom = getBusRoutes();
        $.when(prom).then(function (buses) {
            localStorage.setItem('busRoutes', JSON.stringify(buses));
            drawRoutes(buses);
        });
        getBusLocationData(0, false);
        getUserLocation();
    });

    map.on("move", function () {
        var isAndroid = /(android)/i.test(navigator.userAgent);
        if (!isAndroid) { // Bug with android auto-scrolling.
            d3.select(".popup").remove();
            contextMenuShowing = false;
        }
    });

    map.on('click', function () {
        if (d3.select(".busMenu").node().getBoundingClientRect().width != 0) {
            openMenu(); // Close menu
        }
    });
}


function getBusRoutes() {
    var deferred = $.Deferred();
    //Return cached buses jquery promises
    if (typeof (localStorage.busRoutes) !== "undefined" && localStorage.busRoutes !== null) {
        deferred.resolve(JSON.parse(localStorage.getItem('busRoutes')));
    }
    else {
        //ToDo: swap back to azure
        //"https://sojbuslivetimespublic.azurewebsites.net/api/Values/GetRoutes"
        //http://localhost:55421/api/Values/GetRoutes
        d3.json("https://uat-sojbuslivetimespublic.azurewebsites.net/api/Values/GetRoutes", function (error, data) {
            if (!error) {
                deferred.resolve(data);
            }
            else {
                console.log("Connection Error");
                deferred.reject();
            }
        });
    }
    return deferred.promise();
}


function getBusLocationData(duration, reloadData) {

    if (reloadData || typeof cachedData === 'undefined') {
        // If location is turned on, build parameters
        var locationParam = "";
        if (userLocation != "") {
            locationParam = userLocation + "&stopsWithinXMetres=" + stopRadius + "&limitTo=10000";
            hidePrivacyError();
        } else {
            if (d3.select('.locationCb').property('checked') == true) {
                // Location is checked but don't have coordinates throw privacy/location disabled error.
                showPrivacyError();
            }
        }
        d3.json("https://uat-sojbuslivetimespublic.azurewebsites.net/api/Values/GetMin?secondsAgo=3600" + locationParam, function (error, data) {
            if (!error) {
                drawData(data, duration);
                cachedData = data;
                connectionError = false;
            } else {
                // Error
                console.log("Connection Error");
                connectionError = true;
            };
        });
    }
    else {
        drawData(cachedData, duration);
    }

}


