
var departuresTimer;

function initializeBusTimes() {
    //Load the stopId from URL args if not present default to liberation station
    var params = QueryString;
    if (typeof (params.stopId) !== 'undefined') {
        processStopNumber(params.stopId);
    }
    else {
        processStopNumber("2465");
    }
}

function reload() {
    processStopNumber(localStorage.currentStop);
}

function processStopNumber(stopCode) {
    localStorage.currentStop = stopCode;
    switch (stopCode) {
        case "0": //not set
            changeStop(false);
            break;
        case "2465": //Liberation station
            $('#stopname').text("Liberation Station");
            loadLiveDepartures(stopCode);
            break;
        default: //Any other stop, we could change the API to get the name of the stop code
            $('#stopname').text("Bus stop - " + stopCode);
            loadLiveDepartures(stopCode);
            break;
    }
}

function startAutoRefresh() {
    if (departuresTimer == undefined) {
        departuresTimer = self.setInterval(function () { reload() }, 60000);
    }
}

function cancelAutoRefresh() {
    departuresTimer = window.clearInterval(departuresTimer);
}

function confirmNewStop() {
    var stop = $('#stopNumber').val();
    if (!inputValid(stop)) {
        $('#stopNumber').focus();
        return;
    }
    $('#stopNumber').val('');
    window.location.href = "/Bus/LiveDepartures?stopId=" + stop;
}

function inputValid(inputtedStop) {
    if (inputtedStop === '' || !$.isNumeric(inputtedStop) || inputtedStop.length !== 4) return false;
    return true;
}

function changeStop(showError) {
    if (showError) {
        localStorage.currentStop = 0;
        $('#validationMessage').show();
    } else {
        $('#validationMessage').hide();
    }
    $('#headers').hide();
    $('#resultsListing').show();
    $('#stopNumber').val('');
    $('#stopNumber').focus();
}

var loadingData;

//Handle codes that do not exist or have no buses anytime soon
function loadLiveDepartures(busStopId) {
    if (loadingData) return;
    loadingData = true;
    $('#headers').show();
    $('#status').text("Loading departures..");
    $('#departures').empty();
    $('#resultsListing').hide();
    var timesProm = getLiveDepartureTimes(busStopId);
    $.when(timesProm).then(function (routeNames) {
        generateDeparturesTable(routeNames);
        var time = new Date();
        $('#received').text(time.toTimeString().replace(/.*(\d{2}:\d{2}:\d{2}).*/, "$1"));
        $('#resultsListing').show();
        $('#validationMessage').hide();
        loadingData = false;
    }, function (ex) {
        loadingData = false;
        changeStop(true);
    });
}

function generateDeparturesTable(routes) {
    var table = $('<table id="stops">').addClass('appTable');
    var columnHeaders = ["Route", "Destination", "Scheduled", "Departs"];
    for (var i = 0; i < columnHeaders.length; i++) {
        $('<th>', { style: 'text-align: left; padding: 7px;', text: columnHeaders[i] }).appendTo(table);
    }

    var busRoutesProm = getBusRoutes();
    $.when(busRoutesProm).then(function (routeNames) {
        //Dynamically create departures table
        for (var route in routes) {
            if (routes.hasOwnProperty(route)) {
                var b = routes[route];
                var due = new Date(b.ETA);
                var tr = $('<tr>');
                $('<td style="padding:5px"><div style="background:' + getBusColourHex(b.ServiceNumber, routeNames) + '; height:29px; width:29px">' + '<span style="color:' + getBusTextColors(b.ServiceNumber) + '; display: inline-block; font-weight:bold; margin-top:8px">' + b.ServiceNumber + '</span>' + '</div></td>').appendTo(tr);
                $('<td>', { style: 'text-align:left; padding:8px; width:99%;' }).text(b.Destination).appendTo(tr);
                //due.toLocaleTimeString(navigator.language, { hour: '2-digit', minute: '2-digit' }
                $('<td>', { style: 'text-align:left; padding:8px' }).text(getTime(due)).appendTo(tr);
                $('<td>', { style: 'text-align:left; padding:8px;' }).text(getDurationUntilTime(b.ETA)).appendTo(tr);
                tr.appendTo(table);
            }
        }
        $('#departures').append(table);
    });
}

function getTime(fullDateTime) {
    return checkTime(fullDateTime.getHours()) + ":" + checkTime(fullDateTime.getMinutes());
}

function checkTime(i) {
    return (i < 10) ? "0" + i : i;
}

function getDurationUntilTime(time) {

    var now = new Date();
    var due = new Date(time);
    var totalSec = (due - now) / 1000;
    var hours = parseInt(totalSec / 3600) % 24;
    var minutes = parseInt(totalSec / 60) % 60;

    if (hours === 0) {
        if (minutes === 0) {
            return "< 1m";
        }
        return minutes + "m";
    }
    return hours + "h " + minutes + "m";
}

//Todo: Swap to live
function getLiveDepartureTimes(busStop) {
    var defer = $.Deferred();
    d3.json("http://uat-sojbuslivetimespublic.azurewebsites.net/api/Values/BusStop/" + busStop, function (error, data) {
        if (!error) {
            data.sort(function (a, b) {
                return new Date(a.ETA) - new Date(b.ETA);
            });
            defer.resolve(data);
        }
        else {
            console.log("Connection Error.");
            defer.reject("Incorrect stop code or connection error.");
        }
    });
    return defer.promise();
}

//Gets URL arguments, could be moved to common place
//http://stackoverflow.com/a/979995 
var QueryString = function () {
    var queryString = {};
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        // If first entry with this name
        if (typeof queryString[pair[0]] === "undefined") {
            queryString[pair[0]] = decodeURIComponent(pair[1]);
            // If second entry with this name
        } else if (typeof queryString[pair[0]] === "string") {
            var arr = [queryString[pair[0]], decodeURIComponent(pair[1])];
            queryString[pair[0]] = arr;
            // If third or later entry with this name
        } else {
            queryString[pair[0]].push(decodeURIComponent(pair[1]));
        }
    }
    return queryString;
}();


