var airportTimer;
var carParkingTimer;
var busTrackerPageTimeoutTimer;
var reloadRequested = false;
var eatSafeBootDone = false;
var eatSafeScripts = ['http://ajax.googleapis.com/ajax/libs/angularjs/1.2.25/angular.js', '/Scripts/dirPagination.js', 'http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0', '/Scripts/app.js?t=20141020'];


$(document).bind("pagebeforeload", function (event, data) {
    if (window.location.pathname == '/') {
        if (!IsMultiPageOk()) {
            event.preventDefault();
            RequestReload();
        }
    }
});

$(document).bind("pageinit", function () {

    SetupPopStateCheck();

    $(".appTable tbody tr:nth-child(even)").attr("id", "even");

    $("a:not([href])").each(function () {
        var element = $(this);
        var text = element.text();
        if (text.indexOf('+') == 0) {
            element.attr("href", "tel:" + text.replace(/\s+/g, ''));
        }
    });

    $(".navHomeMenu li, .navSubMenu li").each(function () {
        var match = $(this).has("#jobgeolocreq");
        if (match != undefined) {
            if (!navigator.geolocation) {
                match.remove();
            }
        }
    });

    $(function ($) {
        var SVGsupport = false;
        try {
            var svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
            SVGsupport = typeof svg.createSVGPoint == 'function';
        }
        catch (e)
        { }
        if (SVGsupport) {
            $('body').addClass('svg');
        }
        else {
            $('body').addClass('nosvg');
        }
    });

});

function reSizeBusTrackerMap() {
    var windowHeight, busTrackerPage, uiHeaderHeight, busTrackerContent, busTrackerContentOverhead, subheaderHeight, beforeHtmlHeight, afterHtmlHeight, newMapHeight, mapWrapper, busMenu, buttonWrapperHeight;

    windowHeight = $(window).outerHeight();
    busTrackerPage = ('#busTrackerPage');
    if (busTrackerPage) {
        //uiHeaderHeight = $(busTrackerPage).find('.ui-header').outerHeight();
        uiHeaderHeight = 42; // Had to hard code at it does not always work?
        buttonWrapperHeight = $(busTrackerPage).find('#button-wrapper').outerHeight();
        busTrackerContent = $(busTrackerPage).find('#busTrackerContent');
        busTrackerContentOverhead = busTrackerContent.outerHeight() - busTrackerContent.height();
        subheaderHeight = $(busTrackerPage).find('.subheader').outerHeight();
        beforeHtmlHeight = $(busTrackerPage).find('.beforeHtmlContent').outerHeight();
        afterHtmlHeight = $(busTrackerPage).find('.afterHtmlContent').outerHeight();
        newMapHeight = windowHeight - uiHeaderHeight - busTrackerContentOverhead - subheaderHeight - beforeHtmlHeight - afterHtmlHeight;
        mapWrapper = $(busTrackerPage).find('#map-wrapper');
        if (mapWrapper) {
            mapWrapper.height(newMapHeight);
        }

        busMenu = $(busTrackerPage).find('.busMenu');
        if (busMenu) {
            busMenu.height(newMapHeight - buttonWrapperHeight);
        }
    }
}

function busTrackerPageBeforeMapHeight() {
    var correction, busTrackerPage, uiHeaderHeight, subheaderHeight, beforeHtmlHeight;

    correction = 0;
    busTrackerPage = ('#busTrackerPage');
    if (busTrackerPage) {
        //uiHeaderHeight = $(busTrackerPage).find('.ui-header').outerHeight();
        uiHeaderHeight = 42; // Had to hard code at it does not always work?
        subheaderHeight = $(busTrackerPage).find('.subheader').outerHeight();
        beforeHtmlHeight = $(busTrackerPage).find('.beforeHtmlContent').outerHeight();
        correction = uiHeaderHeight + subheaderHeight + beforeHtmlHeight;
    }

    return correction;
}

//If args based to disable timeout then ignore
$('#busTrackerPage').live("pageinit", function () {
    $(window).resize(function () {
        reSizeBusTrackerMap();
    });
    reSizeBusTrackerMap();
    busTrackerPageInit();
    if (!isTimeoutDisabled()) {
        busTrackerPageTimeoutTimer = self.setInterval(function () {
            ClearBusTrackerPageTimeoutTimer();
            $.mobile.changePage('/Content/Pages/TrackMyBus');
        }, 1800000);
    }
});

function isTimeoutDisabled() {
    var params = MainQueryString;

    if (typeof (params.disableTimeout) !== 'undefined') {
        if (params.disableTimeout === "true") {
            return true;
        }
    }
    return false;
}

$('#eatSafePage').live("pageinit", function () {
    if (!eatSafeBootDone) {
        loadAndExecuteScripts(eatSafeScripts, 0, function () {
            EatSafeDirPaginationCode();
            EatSafeAppCode();
            EatSafeCheckPage();
            angular.bootstrap(document, ['health']);
            eatSafeBootDone = true;
        });
    }
});

function loadAndExecuteScripts(aryScriptUrls, index, callback) {
    $.getScript(aryScriptUrls[index], function () {
        if (index + 1 <= aryScriptUrls.length - 1) {
            loadAndExecuteScripts(aryScriptUrls, index + 1, callback);
        } else {
            if (callback) {
                callback();
            }
        }
    });
}

$('#airportArrivalsPage').live('pageinit', function (event) {
    $(".appTable#airportarrivals tr").on("click", function (event, ui) {
        var place = AirportLiveArrHelper();
        var scheduledTime = $(this).find("td").eq(0).html();
        var flightNumber = $(this).find("td").eq(1).html();
        $.mobile.changePage(place + '?flight=' + flightNumber.replace(/\s+/g, '') + '&time=' + scheduledTime.replace(/:\s*/g, ''));
    })
});

$('#liveFlightArrivalPage').live('pageinit', function (event) {
    $(".flightArrRfrsh", $(this)).bind("click", function (event, ui) {
        var page = $(this).closest('#liveFlightArrivalPage');
        GetFlightArrival(page);
    })

    function GetFlightArrival(page) {
        var flightNumber = $(".flightNum", page).text();
        var time = $(".scheduledTime", page).text();
        var updatenode = $("#liveFlightArr", page);
        var flightNumberRep = flightNumber.replace(/\s+/g, '');
        var timeRep = time.replace(/:\s*/g, '');
        $.ajax({
            url: FlightArrivalDataHelper(),
            data: { "flight": flightNumberRep, "time": timeRep },
            type: 'POST',
            headers: { "cache-control": "no-cache" },
            beforeSend: function () {
                $.mobile.loading('show');
            },
            complete: function () {
                $.mobile.loading('hide');
            },
            //error: searchFailed,
            success: function (data) {
                $(updatenode).html(data).trigger("create");
            }
        });
    }

    $(".flightArrBtn input[type='radio']", $(this)).change(function () {
        var selection = $(this).val();
        if (selection == 'on') {
            if (airportTimer == undefined) {
                var page = $(this).closest('#liveFlightArrivalPage');
                airportTimer = self.setInterval(function () { GetFlightArrival(page) }, 60000);
            }
        }
        else {
            ClearAirportTimer();
        }
    });
});

$('#airportDeparturesPage').live('pageinit', function (event) {
    $(".appTable#airportdepartures tr").on("click", function (event, ui) {
        var place = AirportLiveDepHelper();
        var scheduledTime = $(this).find("td").eq(0).html();
        var flightNumber = $(this).find("td").eq(1).html();
        $.mobile.changePage(place + '?flight=' + flightNumber.replace(/\s+/g, '') + '&time=' + scheduledTime.replace(/:\s*/g, ''));
    })
});

$('#liveFlightDeparturePage').live('pageinit', function (event) {
    $(".flightDepRfrsh", $(this)).bind("click", function (event, ui) {
        var page = $(this).closest('#liveFlightDeparturePage');
        GetFlightDeparture(page);
    })

    function GetFlightDeparture(page) {
        var flightNumber = $(".flightNum", page).text();
        var time = $(".scheduledTime", page).text();
        var updatenode = $("#liveFlightDep", page);
        var flightNumberRep = flightNumber.replace(/\s+/g, '');
        var timeRep = time.replace(/:\s*/g, '');
        $.ajax({
            url: FlightDepartureDataHelper(),
            data: { "flight": flightNumberRep, "time": timeRep },
            type: 'POST',
            headers: { "cache-control": "no-cache" },
            beforeSend: function () {
                $.mobile.loading('show');
            },
            complete: function () {
                $.mobile.loading('hide');
            },
            //error: searchFailed,
            success: function (data) {
                $(updatenode).html(data).trigger("create");
            }
        });
    }

    $(".flightDepBtn input[type='radio']", $(this)).change(function () {
        var selection = $(this).val();
        if (selection == 'on') {
            if (airportTimer == undefined) {
                var page = $(this).closest('#liveFlightDeparturePage');
                airportTimer = self.setInterval(function () { GetFlightDeparture(page) }, 60000);
            }
        }
        else {
            ClearAirportTimer();
        }
    });
});


function ClearAirportTimer() {
    if (airportTimer != undefined) {
        airportTimer = window.clearInterval(airportTimer);
    }
}

function ClearCarParkingTimer() {
    if (carParkingTimer != undefined) {
        carParkingTimer = window.clearInterval(carParkingTimer);
    }
}

function ClearBusTrackerTimer() {
    if (myTimer != undefined) {
        myTimer = window.clearInterval(myTimer);
    }
}

function ClearBusTrackerPageTimeoutTimer() {
    if (busTrackerPageTimeoutTimer != undefined) {
        busTrackerPageTimeoutTimer = window.clearInterval(busTrackerPageTimeoutTimer);
    }
}

$('#liveFlightArrivalPage').live('pageremove', function (event) {
    ClearAirportTimer();
});

$('#liveFlightDeparturePage').live('pageremove', function (event) {
    ClearAirportTimer();
});

$('#carParkingLiveUpdatesPage').live('pageremove', function (event) {
    ClearCarParkingTimer();
});

$('#busTrackerPage').live('pageremove', function (event) {
    ClearBusTrackerTimer();
    ClearBusTrackerPageTimeoutTimer();
});


$(window).unload(function () {
    ClearAirportTimer();
    ClearCarParkingTimer();
    ClearBusTrackerTimer();
    ClearBusTrackerPageTimeoutTimer();
});

$('#jobsLatestGeoPage').live('pageshow', function (event) {

    var x = document.getElementById("jobsData");
    var msghdr = "<strong>This page is only available if you are currently in Jersey. ";

    function getLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(getJobs, showError, { timeout: 5000 });
        }
        else {
            x.innerHTML = msghdr + "Geolocation is not supported by this browser.</strong>";
        }
    }
    function getJobs(position) {
        GetJobsData(position.coords.latitude, position.coords.longitude);
    }

    function showError(error) {
        var message;
        switch (error.code) {
            case error.PERMISSION_DENIED:
                message = "Please allow your device to detect your location."
                break;
            case error.POSITION_UNAVAILABLE:
                message = "Location information is unavailable."
                break;
            case error.TIMEOUT:
                message = "The request to get user location timed out."
                break;
            case error.UNKNOWN_ERROR:
                message = "An unknown error occurred."
                break;
        }
        x.innerHTML = msghdr + message + "</strong>";
    }

    function GetJobsData(latitude, longitude) {
        $.ajax({
            url: JobsLatestDataHelper(),
            data: { "latitude": latitude, "longitude": longitude },
            type: 'POST',
            headers: { "cache-control": "no-cache" },
            beforeSend: function () {
                $.mobile.loading('show');
            },
            complete: function () {
                $.mobile.loading('hide');
            },
            //error: searchFailed,
            success: function (data) {
                $("#jobsData").html(data).trigger("create");
            }
        });
    }

    getLocation();

});

function SetupPopStateCheck() {
    window.onpopstate = function (event) {
        if (!reloadRequested) {
            if (window.location.pathname == '/') {
                if (!IsMultiPageOk()) {
                    RequestReload();
                }
            }
        };
    }
}

function RequestReload() {
    if (!reloadRequested) {
        reloadRequested = true;
        location.reload();
    }
}

function IsMultiPageOk() {
    result = true;
    var multipagecount = $('meta[name=sojmobilemultipagecount]').attr("content");
    if (multipagecount) {
        if (multipagecount > 1) {
            if ($(".navSubMenu").length == 0) {
                result = false;
            }
        }
        if (multipagecount >= 1) {
            if ($(".navHomeMenu").length == 0) {
                result = false;
            }
        }
    }
    return result;
}




$('#carParkingLiveUpdatesPage').live('pageinit', function (event) {
    $(".liveCarParkingRfrsh", $(this)).bind("click", function (event, ui) {
        var page = $(this).closest('#carParkingLiveUpdatesPage');
        GetCarParkingUpdate(page);
    });

    function GetCarParkingUpdate(page) {
        var updatenode = $("#liveCarParking", page);
        $.ajax({
            url: CarParkingDataHelper(),
            type: 'POST',
            headers: { "cache-control": "no-cache" },
            beforeSend: function () {
                $.mobile.loading('show');
            },
            complete: function () {
                $.mobile.loading('hide');
            },
            //error: searchFailed,
            success: function (data) {
                $(updatenode).html(data).trigger("create");
            }
        });
    }

    $(".liveCarParkingBtn input[type='radio']", $(this)).change(function () {
        var selection, page;
        selection = $(this).val();
        if (selection == 'on') {
            if (carParkingTimer == undefined) {
                page = $(this).closest('#carParkingLiveUpdatesPage');
                carParkingTimer = self.setInterval(function () {
                    GetCarParkingUpdate(page);
                }, 60000);
            }
        } else {
            ClearCarParkingTimer();
        }
    });
});


//http://stackoverflow.com/a/979995 *Gets URL arguments, could be moved to common place
var MainQueryString = function () {
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