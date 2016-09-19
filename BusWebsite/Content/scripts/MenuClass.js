
var stopRadiusHasChanged = false;

function openMenu() {
    setMenuVisibility(d3.select(".busMenu").node().getBoundingClientRect().width === 0);
}

function setMenuVisibility(isVisible) {
    if (isVisible) {
        d3.select(".busMenu")
            .transition()
            .duration(50)
            .style("width", "300px");

        d3.select(".busMenuContent")
            .transition()
            .delay(50)
            .style("display", "");

        d3.select(".btnStyle")
            .classed("highlight", true);

        d3.select(".backIcon")
            .style("display", "");


    } else {
        d3.select(".busMenu")
            .transition()
            .duration(50)
            .style("width", "0px");

        d3.select(".busMenuContent")
            .style("display", "none");

        d3.select(".btnStyle")
            .classed("highlight", false);

        d3.select(".backIcon")
            .style("display", "none");

        // Only update stop radius on menu close
        if (stopRadiusHasChanged != false) {

            if (stopRadiusHasChanged == 1) {
                stopRadiusHasChanged = 0;
            }
            stopRadius = stopRadiusHasChanged;
            stopRadiusHasChanged = false;
            getUserLocation(true);
        }
    }
}

function closeSplash() {
    init();
    d3.select(".splashScreen").remove();
    localStorage.hasLaunched = 1;
}

function getSelectionData() {
    if (typeof (Storage) !== "undefined") {
        if (localStorage.selectionList) {
            return localStorage.selectionList;
        }
    }
    //Go off checkboxes as in incognito
    return getCheckedRoutesString();
}


function setBusRoutes(defaultCheckedItems) {

    $("#menuLoading").show();
    var container = document.getElementById('busroutes');
    var busrPromise = getBusRoutes();

    $.when(busrPromise
    ).then(function (busr) {
        //Get and sort the routes asec by numerical value
        localStorage.setItem('busRoutes', JSON.stringify(busr));
        var routes = busr["routes"];
        routes.sort(function (a, b) {
            return parseInt(getBusNumberWithoutLetters(a.Number)) - parseInt(getBusNumberWithoutLetters(b.Number));
        });

        var arraylist;
        if (defaultCheckedItems !== undefined) {
            arraylist = defaultCheckedItems.split(',');
        }
        var count = 0;
        var anyNotChecked = false;
        for (var route in routes) {
            var isChecked = false;
            var busName = tidyBusName(routes[route].Name);
            var busNumber = routes[route].Number;
            var newId = "chk" + count;
            //See if route should be checked by default
            if (defaultCheckedItems !== undefined && arraylist != null) {
                isChecked = contains(arraylist, busNumber);
            }
            if (!isChecked && !anyNotChecked) {
                anyNotChecked = true;
            }
            //Styles should be in css except for background
            $('<input/>', { type: 'checkbox', defaultValue: 10, checked: isChecked, id: newId, value: busNumber, name: "busChk" }).appendTo(container);
            var routeName = '<span style="vertical-align: middle; line-height: 32px; float: left">' + busName + '</span>' + '<div style="float: right; width: 32px; height: 32px; background:' + getBusColourHex(busNumber, busr) + ';" text-align: center;>' + '<span  style="color:' + getBusTextColors(busNumber) + '; font-weight:bold; vertical-align: middle; line-height: 32px; display:table; margin:0 auto;">' + busNumber + '</span>' + '</div>';
            $('<label />', { 'for': "chk" + count, html: routeName }).appendTo(container);
            count++;
        }
        //Everything was selected so select all should be ticked
        if (!anyNotChecked) { $('.selectAll').attr('checked', true).checkboxradio("refresh"); }
        $("[type=checkbox]").checkboxradio();
        $(document).on('change', '[type=checkbox]', function () {
            selectedRoutesChanged();
        });
        $("#menuRoutes").removeClass('.menuhide');
        $("#menuRoutes").show();
        $("#menuLoading").hide();
    });
}

function contains(a, obj) {
    var i = a.length;
    while (i--) {
        if (a[i] === obj) {
            return true;
        }
    }
    return false;
}

function tidyBusName(busName) {
    if (busName === "") {
        return "Unknown";
    }
    return busName;
}

function getBusNumberWithoutLetters(busNumber) {
    return busNumber.match(/\d+/)[0];
}

function selectedRoutesChanged() {
    setSelectionData();
    var prom = getBusRoutes();
    $.when(prom).then(function (buses) {
        localStorage.setItem('busRoutes', JSON.stringify(buses));
        drawRoute(buses);
    });
    getData(0, true);
}

function isDisplayRoutesOnMapChecked() {
    return document.getElementById('showRoutes').checked;
}

function getCheckedRoutesString() {
    var checkedRoutes = $("input[name=busChk]:checked").map(
        function () { return this.value; }).get().join(",");

    if ($(school).attr("checked")) {
        if (checkedRoutes !== "") {
            checkedRoutes = checkedRoutes + ",school";
        } else {
            checkedRoutes = "school";
        }
    }

    return checkedRoutes;
}

//include buses
function setSelectionData() {

    var selectionStr = getCheckedRoutesString();

    if (selectionStr.length === 0) {
        // Null localstorage will return 'undefinded' and default to private mode showing all buses.
        selectionStr = ".";
    }
    localStorage.selectionList = selectionStr;
}

// TODO: Duplication needs cleanup
function loadUpdateInterval() {
    var rangeVal, interval, duration, intervalObject;
    interval = 5000;

    if (localStorage.updateInterval !== undefined) {
        interval = localStorage.updateInterval;  // Default
    }

    // Set UI label
    rangeVal = $('#rangeVal');
    if (interval == 5000) {
        rangeVal.attr("value", "1");
        d3.select("#rangeValLabel").html("Every 5 seconds");
    } else if (interval == 15000) {
        rangeVal.attr("value", "2");
        d3.select("#rangeValLabel").html("Every 15 seconds");
    } else if (interval == 30000) {
        rangeVal.attr("value", "3");
        d3.select("#rangeValLabel").html("Every 30 seconds");
    } else if (interval == 60000) {
        rangeVal.attr("value", "4");
        d3.select("#rangeValLabel").html("Every 1 minute");
    }
    rangeVal.slider("refresh");

    duration = interval;
    if (duration >= 15000) {
        duration = 10000;
    }
    intervalObject = {
        update: interval,
        duration: duration
    };
    return intervalObject;
}

function changeUpdateInterval(intValue) {

    var interval;
    var labelStr = "";
    switch (intValue) {
        case "1":
            interval = 5000;
            labelStr = "Every 5 seconds";
            break;
        case "2":
            interval = 15000;
            labelStr = "Every 15 seconds";
            break;
        case "3":
            interval = 30000;
            labelStr = "Every 30 seconds";
            break;
        case "4":
            interval = 60000;
            labelStr = "Every 1 minute";
            break;
    }

    clearInterval(myTimer);

    var duration = interval;
    if (isLocalStorageNameSupported()) {
        localStorage.updateInterval = interval;
    }
    if (duration >= 15000) {
        duration = 10000;
    }

    myTimer = setInterval(function () { getData(duration, true); }, interval);
    return labelStr;
}

function loadStopRadius() {

    var stopRadiusVal;

    stopRadius = 500;
    if (localStorage.stopRadius !== undefined) {
        stopRadius = localStorage.stopRadius;  // Default
    }


    if (stopRadius == 1) {
        stopRadius = 0;
    }

    // Set UI label
    stopRadiusVal = $('#stopRadiusVal');
    if (stopRadius == 0) {
        stopRadiusVal.attr("value", "0");
        d3.select("#stopRadiusValLabel").html("Hide stops");
    } else if (stopRadius == 250) {
        stopRadiusVal.attr("value", "1");
        d3.select("#stopRadiusValLabel").html("250 metres");
    }
    else if (stopRadius == 500) {
        stopRadiusVal.attr("value", "2");
        d3.select("#stopRadiusValLabel").html("500 metres");
    } else if (stopRadius == 2500) {
        stopRadiusVal.attr("value", "3");
        d3.select("#stopRadiusValLabel").html("2500 metres");
    } else if (stopRadius == 5000) {
        stopRadiusVal.attr("value", "4");
        d3.select("#stopRadiusValLabel").html("5000 metres");
    } else if (stopRadius == 10000) {
        stopRadiusVal.attr("value", "5");
        d3.select("#stopRadiusValLabel").html("All stops");
    }
    stopRadiusVal.slider("refresh");
}

function changeStopRadius(intValue) {

    var labelStr = "";
    switch (intValue) {
        case "0":
            stopRadiusHasChanged = 1;
            labelStr = "Hide stops";
            break;
        case "1":
            stopRadiusHasChanged = 250;
            labelStr = "250 metres";
            break;
        case "2":
            stopRadiusHasChanged = 500;
            labelStr = "500 metres";
            break;
        case "3":
            stopRadiusHasChanged = 2500;
            labelStr = "2500 metres";
            break;
        case "4":
            stopRadiusHasChanged = 5000;
            labelStr = "5000 metres";
            break;
        case "5":
            stopRadiusHasChanged = 10000;
            labelStr = "All stops";
            break;
    }


    if (isLocalStorageNameSupported()) {
        localStorage.stopRadius = stopRadiusHasChanged;
    }
    return labelStr;
}

function getUserLocation(isFirstCall) {

    var locEnabled = 0;
    if (typeof (Storage) !== "undefined") {
        if (localStorage.locEnabled) {
            locEnabled = localStorage.locEnabled;
        }
    }

    if (locEnabled == 1) {
        $(".locationCb").prop('checked', true).checkboxradio('refresh');
        if (navigator.geolocation) {
            d3.selectAll(".userLocation").remove();
            navigator.geolocation.getCurrentPosition(function (position) {

                drawLocation(position)

                // User is zoomed in on stops so don't update userLocation
                if (map.getZoom() < 16 || stopRadius == 0) {
                    userLocation = "&lat=" + position.coords.latitude + "&lon=" + position.coords.longitude;
                }

                if (isFirstCall) {
                    getData(0, true);
                }


            }, function (error) {
                console.log('Location failed with error: ' + error.code);
                if (isFirstCall) {
                    getData(0, true);
                }
            }, { timeout: 5000 });
        } else {
            // Browser not supported
            d3.select("#locationSection").remove();
            if (isFirstCall) {
                getData(0, true);
            }
        }
    } else {
        d3.select(".userLocation").remove();
        $(".locationCb").prop('checked', false).checkboxradio('refresh');
        if (isFirstCall) {
            getData(0, true);
        }
    }

}

function drawLocation(position) {

    // Show user location
    svg.append('circle')
        .attr('class', 'userLocation')
        .attr("transform", (function () {
            var latlng = new L.latLng(position.coords.latitude, position.coords.longitude);
            var myX = map.latLngToLayerPoint(latlng).x;
            var myY = map.latLngToLayerPoint(latlng).y;
            return "translate(" + myX + "," + myY + ")";
        }))
        .attr("r", "8")
        .style("stroke", "white")
        .style("stroke-width", "2")
        .style("fill", 'steelblue');

}

function showPrivacyError() {

    if (d3.select(".privacyErrorMenu").node().getBoundingClientRect().height === 0) {
        d3.select(".privacyErrorMenu")
            .transition()
            .duration(50)
            .style("height", "auto");


        d3.select(".privacyErrorMenuContent")
           .transition()
           .delay(50)
           .style("display", "");

    }
}

function closePrivacyError() {
    $('.locationCb').prop('checked', false).checkboxradio('refresh');
    hidePrivacyError();
}


function hidePrivacyError() {

    if (d3.select(".privacyErrorMenu").node().getBoundingClientRect().height != 0) {
        d3.select(".privacyErrorMenu")
            .transition()
            .duration(50)
            .style("height", "0");
        d3.select(".privacyErrorMenuContent")
           .transition()
           .delay(50)
           .style("display", "none");
    }
}


function isLocalStorageNameSupported() {
    var testKey = 'test', storage = window.localStorage;
    try {
        storage.setItem(testKey, '1');
        storage.removeItem(testKey);
        return true;
    } catch (error) {
        return false;
    }
}