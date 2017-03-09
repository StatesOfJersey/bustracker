

function drawData(data, duration) {

    // STOPS
    var stopData = data["stops"];
    if (stopData != null) {
        drawStop(stopData);
    }

    var busData = data["minimumInfoUpdates"];
    drawBus(busData, duration);
}

function drawBus(busData, duration) {
    // Binds data to Id
    var points = svg.selectAll('.busItem')
        .data(busData, function (d) { return d["bus"]; });

    var prom = getBusRoutes();

    $.when(prom).then(function (routes) {
        //console.log(busData);
        //localStorage.buses;
        // Enter bus item
        var pointsEnter = points
            .enter()
            .append('g')
            .attr('class', 'busItem')
            .attr("transform", (function (d) {
                return convertBusPosition(d);
            }))
            .style("opacity", (function (d) {
                return getBusOpacity(d);
            }))
            .style("visibility", (function (d) {
                return getBusVisibility(d);
            }))
            .on('click', busClicked);




        // Enter circle
        pointsEnter
             .append('path').attr({ d: busShape })
             .attr("transform", (function (d) {
                 return convertBusPositionAndBearing(d);
             }))
             .style("visibility", "hidden");



        /////////////
        //   XMAS Logic

        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!

        var thisYear = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd
        }
        if (mm < 10) {
            mm = '0' + mm
        }
        var dateCheck = dd + '/' + mm + '/' + thisYear;

        var dateFrom = "14/12/" + thisYear;
        var dateTo = "27/12/" + thisYear;

        var d1 = dateFrom.split("/");
        var d2 = dateTo.split("/");
        var c = dateCheck.split("/");

        var from = new Date(d1[2], parseInt(d1[1]) - 1, d1[0]);  // -1 because months are from 0 to 11
        var to = new Date(d2[2], parseInt(d2[1]) - 1, d2[0]);
        var check = new Date(c[2], parseInt(c[1]) - 1, c[0]);


        if (check > from && check < to) {

            ////XMAS
            var xmas1 = 'M67.5,31.5c-3.3-5.8-11-11-18.5-11c-10.8,0-19.5,8.8-19.5,19l36,1c-2-1-2-7-2-7c0-3,2,1,2,1L67.5,31.5z'
            var xmas2 = 'M62.4,48.5c-8.1-6-21.7-6-29.8,0c-2,1.3-4.5,1-5.6-1.1c0-0.1,0-0.1-0.1-0.1c-1.2-2.1-0.2-5.5,2.2-7.1c11.2-7.5,25.6-7.5,36.8,0c2.5,1.6,3.4,5,2.2,7.1c0,0.1,0,0.1-0.1,0.1C66.9,49.5,64.4,49.8,62.4,48.5z'
            var xmas3 = 'M69.1,39c-2.812,0-5.1-2.288-5.1-5.1s2.288-5.1,5.1-5.1c2.812,0,5.101,2.288,5.101,5.1S71.912,39,69.1,39z M69.1,29.8c-2.261,0-4.1,1.839-4.1,4.1c0,2.261,1.839,4.1,4.1,4.1s4.101-1.839,4.101-4.1C73.2,31.639,71.36,29.8,69.1,29.8z'

            pointsEnter
                .append('path').attr({ d: xmas1 })
                .attr("transform", (function (d) {
                    return xmasPosition(d);
                }))
                .style("stroke", d3.rgb(59, 63, 65))
                .style("stroke-width", "1")
                .style("fill", "red");

            pointsEnter
                .append('path').attr({ d: xmas2 })
                .attr("transform", (function (d) {
                    return xmasPosition(d);
                }))
                .style("stroke", d3.rgb(59, 63, 65))
                .style("stroke-width", "1")
                 .style("fill", "white");

            pointsEnter
                .append('path').attr({ d: xmas3 })
                .attr("transform", (function (d) {
                    return xmasPosition(d);
                }))
                .style("stroke", d3.rgb(59, 63, 65))
                .style("stroke-width", "1")
                 .style("fill", "white");

            function xmasPosition(d) {
                return "translate(-23,-29) scale(0.50) rotate(0, 11, 15)"
            }
        }

        pointsEnter
           .append('path').attr({ d: busShape })
           .attr("transform", (function (d) {
               return convertBusPositionAndBearing(d);
           }))
           .style("stroke", d3.rgb(59, 63, 65))
           .style("stroke-width", "1.5")
           .style("fill", (function (d) {
               return getBusColourHex(d["line"], routes);
           }));





        // Enter text
        pointsEnter
            .append('text')
            .attr("dy", "0.35em")
            .style("text-anchor", "middle")
            .style("font-size", (function (d) {
                if (d["line"].length > 2) {
                    return "9px";
                }
                return "11px";
            }))
            .style('fill', (function (d) {
                return getBusTextColourHex(d["line"], routes);
            }))
            .text(function (d) { return d.line; });

        // Update text when bus changes line e.g 5 becomes 7
        points.select('text')
               .style('fill', (function (d) {
                   return getBusTextColourHex(d["line"], routes);
               }))
               .text(function (d) { return d.line; });

        // Update bus item
        pointsUpdate = points
            .style("opacity", (function (d) {
                return getBusOpacity(d);
            }))
            .style("visibility", (function (d) {
                return getBusVisibility(d);
            }))
            .transition().duration(duration)
            .attr("transform", (function (d) {
                return convertBusPosition(d);
            }))
            .select('path')
                .attr("transform", (function (d) {
                    return convertBusPositionAndBearing(d);
                }))
                .style("fill", (function (d) {
                    return getBusColourHex(d["line"], routes);
                }));


        // Exit bus item
        var pointsExit = points.exit().transition().duration(500).remove();
    });
}

function drawStop(stopData) {
    var stops = svg.selectAll('.stopItem')
       .data(stopData, function (d) { return d["StopNumber"]; });

    var stopsEnter = stops
        .enter()
        .append('g')
        .attr('class', 'stopItem')
        .attr("transform", (function (d) {
            var latlng = new L.latLng(d["Latitude"], d["Longitude"]);
            var myX = map.latLngToLayerPoint(latlng).x;
            var myY = map.latLngToLayerPoint(latlng).y;
            return "translate(" + myX + "," + myY + ")";
        }))
        .on('click', stopClicked);

    var mapZoom = map.getZoom();
    var stopRadius = 8;
    if (mapZoom > 12)
        stopRadius = 12;
    if (mapZoom > 14)
        stopRadius = 16;

    console.log("zoom", mapZoom, "radius", stopRadius);

    stopsEnter.append('rect')
       .attr("width", "2px")
       .attr("height", "20px")
       .attr("x", "0")
       .attr("y", "-20")
       .style('fill', d3.rgb(136, 136, 136));

    stopsEnter.append('circle')
        .attr("r", stopRadius)
        .attr("cx", "1")
        .attr("cy", "-20")
        .style("stroke", "black")
        .style("stroke-width", "1")
        .style("fill", d3.rgb(227, 44, 25));

    /* //this generates a halo around the stops
    stopsEnter.append("circle")
        .attr("r", stopRadius + 10)
        .attr("cx", "1")
        .attr("cy", "-20")
        .style("stroke", "black")
        .style("stroke-width", "1")
        .style("opacity", .01)
        .style("fill", d3.rgb(227, 44, 25));
        */

    var stopsExit = stops.exit().remove();
}


function drawRoutes(routedata) {

    var routes = svg.selectAll('.route');
    routes.remove(); //Clear existing
    if (!isDisplayRoutesOnMapChecked()) {
        return;
    }

    var mode = (map.getZoom() > 14) ? "linear" : "basis";


    var lineFunction = d3.svg.line()
        .x(function (d) {
            var latlng = new L.latLng(d["lat"], d["lon"]);
            return map.latLngToLayerPoint(latlng).x;
        })
        .y(function (d) {
            var latlng = new L.latLng(d["lat"], d["lon"]);
            return map.latLngToLayerPoint(latlng).y;
        }).interpolate(mode);

    var routes = routedata["routes"];
    var routeColour;
    var lineStyle;


    for (var i = 0; i < routes.length; i++) {
        var route = routes[i];
        routeColour = route["Colour"];


        if (route["RouteCoordinates"] != null && getRouteVisibility(route) === 'visible') {
            var lastSection = '';
            var sectionBoundaries = [0];

            for (var j = 0; j < route["RouteCoordinates"].length; j++) {
                var coordinateItem = route["RouteCoordinates"][j];

                if (coordinateItem["splitSection"] != null && lastSection != coordinateItem["splitSection"]) {
                    lastSection = coordinateItem["splitSection"];
                    sectionBoundaries.push(j);
                }
            }

            for (var k = 0; k < sectionBoundaries.length; k++) {
                var firstPointOfSection = route["RouteCoordinates"][sectionBoundaries[k]];
                lineStyle = (firstPointOfSection["direction"] == "I") ? "5, 5" : "none"; // I=Inbound

                if (sectionBoundaries.length == 1) { //just one section so output it
                    var lineGraph = svg.append("path")
                      .attr("d", lineFunction(route["RouteCoordinates"]))
                      .attr('class', 'route')
                      .attr("stroke", routeColour)
                      .attr("stroke-width", 2)
                      .attr("fill", "none")
                      .attr("stroke-dasharray", lineStyle);
                } else if (k == sectionBoundaries.length - 1) { //last one in the array so from here to the end
                    var lineGraph = svg.append("path")
                      .attr("d", lineFunction(route["RouteCoordinates"].slice(sectionBoundaries[k], route["RouteCoordinates"].length)))
                      .attr('class', 'route')
                      .attr("stroke", routeColour)
                      .attr("stroke-width", 2)
                      .attr("fill", "none")
                      .attr("stroke-dasharray", lineStyle);
                } else { // a mid route section
                    var lineGraph = svg.append("path")
                     .attr("d", lineFunction(route["RouteCoordinates"].slice(sectionBoundaries[k], sectionBoundaries[k + 1])))
                     .attr('class', 'route')
                     .attr("stroke", routeColour)
                     .attr("stroke-width", 2)
                     .attr("fill", "none")
                     .attr("stroke-dasharray", lineStyle);
                }
            }
        }
    }

    var points = svg.selectAll('.busItem');
    if (points.length > 0) {

        d3.selection.prototype.moveToFront = function () {
            return this.each(function () {
                this.parentNode.appendChild(this);
            });
        };

        points.moveToFront();
    }

}



function busClicked(d) {
    var beforeMapHeight, topMinimum, topCorrection;

    beforeMapHeight = busTrackerPageBeforeMapHeight();
    topMinimum = beforeMapHeight + 53;
    topCorrection = 65;

    if (contextMenuShowing == d.bus) {
        d3.event.preventDefault();
        d3.select(".popup").remove();
        contextMenuShowing = false;
    } else {

        d3.select(".popup").remove();
        d3_target = d3.select(d3.event.target);
        d3.event.preventDefault();
        contextMenuShowing = d.bus;
        d = d3_target.datum();

        var mapWidth = d3.select("#map").node().getBoundingClientRect().width;

        // Build the popup  
        if (d["cat"] == "Public Bus") {

            var top = d3.event.pageY - 120 - beforeMapHeight;

            if (top < topMinimum) {
                top = topCorrection;
            }
            var left = d3.event.pageX - 100;
            if (left < 0) {
                left = 10;
            } else if (left > (mapWidth - 200)) {
                left = mapWidth - 210;
            }

            var popup = d3.select("#map")
                .append("div")
                .attr("class", "popup")
                .style("left", left + "px")
                .style("top", top + "px");

            popup.append("input")
                .attr("type", "button")
                .attr("class", "closePopupBtn")
                .attr("onclick", "closePopup()");

            var prom = getBusRoutes();

            $.when(prom).then(function (routes) {
                popup.append("h2").text("Route: " + d["line"] + " - " + getBusLineDestination(d["line"], routes, d["direction"]));
                popup.append("h4").text("Updated: " + convertTime(d["age"]));

                // libertybus.je doesn't redirect to mobile site properly, so check device type
                var liberyTimetableSite = "http://www.libertybus.je/routes_times/timetables/" + d.line + "/true";
                if (typeof window.orientation !== 'undefined') {
                    liberyTimetableSite = "http://m.libertybus.je/index.php?content=Timetables&show_route=" + d.line + "&short=TRUE";
                }
                popup.append("p")
                    .append("a")
                    .attr("href", liberyTimetableSite)
                    .text("Timetable");
            });


        } else {

            var top = d3.event.pageY - 90 - beforeMapHeight;
            if (top < topMinimum) {
                top = topCorrection;
            }
            var left = d3.event.pageX - 100;
            if (left < 0) {
                left = 10;
            } else if (left > (mapWidth - 200)) {
                left = mapWidth - 210;
            }

            // School
            var popup = d3.select("#map")
               .append("div")
               .attr("class", "popup")
               .style("left", left + "px")
               .style("top", top + "px");

            popup.append("input")
                .attr("type", "button")
                .attr("class", "closePopupBtn")
                .attr("onclick", "closePopup()");

            popup.append("h2").text("School Bus: " + d["line"]);
            popup.append("h4").text("Updated: " + convertTime(d["age"]));

        }
    }
};

function getBusLineDestination(lineNumber, busRoutes, direction) {

    if (direction === 'inbound') return "Liberation Station";

    var routes = busRoutes["routes"];
    // iterate over each element in the array
    for (var i = 0; i < routes.length; i++) {
        // look for the entry with a matching `code` value
        if (routes[i].Number === lineNumber) {
            return routes[i].Name;
        }
    }
    return "#0000FF"; //Return blue
}


function closePopup() {
    d3.select(".popup").remove();
    contextMenuShowing = false;
}


function stopClicked(d) {
    var beforeMapHeight, topMinimum, topCorrection;

    beforeMapHeight = busTrackerPageBeforeMapHeight();
    topMinimum = beforeMapHeight + 53;
    topCorrection = 65;

    if (contextMenuShowing == d.StopNumber) {
        d3.event.preventDefault();
        d3.select(".popup").remove();
        contextMenuShowing = false;
    } else {

        d3.select(".popup").remove();
        d3_target = d3.select(d3.event.target);
        d3.event.preventDefault();
        contextMenuShowing = d.StopNumber;
        d = d3_target.datum();
        var mapWidth = d3.select("#map").node().getBoundingClientRect().width;


        var top = d3.event.pageY - 190 - beforeMapHeight;
        if (top < topMinimum) {
            top = topCorrection;
        }
        var left = d3.event.pageX - 155;
        if (left < 0) {
            left = 10;
        } else if (left > (mapWidth - 310)) {
            left = mapWidth - 320;
        }

        // Build the popup  
        var popup = d3.select("#map")
            .append("div")
            .attr("class", "popup")
            .style("left", left + "px")
            .style("top", top + "px")
            .style("width", "300px")
            .style("min-height", "180px");

        popup.append("input")
             .attr("type", "button")
             .attr("class", "closePopupBtn")
             .attr("onclick", "closePopup()");

        popup.append("h2").attr("class", "busStopHeading").text(d["StopName"] + " (" + d["StopNumber"] + ")");

        if (localStorage.locEnabled == 1 && isZoomed == false) {
            popup.append("h4").text("Distance: " + d["DistanceAwayInMetres"] + "m").style("color", "black");
        }

        popup.append("h4")
            .attr("class", "loadingMessage")
            .text("Loading...");
        //https://sojbuslivetimespublic.azurewebsites.net/api/Values/BusStop/
        d3.json("https://sojbuslivetimespublic.azurewebsites.net//api/Values/BusStop/" + d["StopNumber"], function (data) {
            drawEtaTable(popup, data);
            d3.select(".loadingMessage").remove();
        });
    }
};

function drawEtaTable(popup, data) {
    if (data != null) {
        if (data.length === 0) {
            popup.append("h4").text("No buses due in next few hours.").style("text-align", "center").style("color", "black");
        } else {
            var table = popup.append("table").style("width", "280px").style("text-align", "left");
            var columnHeaders = ["ETA", "Time", "Route", "Destination"];

            for (var i = 0; i < columnHeaders.length; i++) {
                table.append("th").text(columnHeaders[i]).style("text-align", "left").style("color", "black");
            }
            var anyNotTracked = false;
            //Create each row in the table
            for (var route in data) {
                var b = data[route];
                var tr = table.append("tr");
                var eta = convertTimeToEta(b.ETA);
                if (b.IsTracked === false) {
                    eta = eta + "*";
                    anyNotTracked = true;
                }
                tr.append("td").text(eta).style("text-align", "left").style("color", "black");
                tr.append("td").text(getShortTime(b.ETA)).style("color", "black");
                tr.append("td").text(b.ServiceNumber).style("text-align", "left").style("color", "black");
                tr.append("td").text(b.Destination).style("text-align", "left").style("color", "black");
            }
            if (anyNotTracked) {
                popup.append("p").text("*This is the scheduled time of arrival not the tracked time.").style("width", "265px").style("margin-top", "5px").style("color", "black").style("font-size", "100%");
            }
        }
    } else {
        popup.append("h4").text("Connection error.").style("text-align", "center"); //API returned no data
    }
}

function getShortTime(time) {
    var due = new Date(time);
    return getTime(due);
}

function convertTimeToEta(time) {

    var now = new Date();
    var due = new Date(time);

    var totalSec = (due - now) / 1000;
    var hours = parseInt(totalSec / 3600) % 24;
    var minutes = parseInt(totalSec / 60) % 60;

    if (hours == 0) {
        if (minutes == 0) {
            return "< 1m";
        }
        return minutes + "m";
    }
    return hours + "h " + minutes + "m";
}



////////////////// HELPER FUNCTIONS //////////////////
function convertBusPosition(d) {
    var latlng = new L.latLng(d["lat"], d["lon"]);
    var myX = map.latLngToLayerPoint(latlng).x;
    var myY = map.latLngToLayerPoint(latlng).y;
    return "translate(" + myX + "," + myY + ")";
}


// TODO: Wobble nightmare
// http://stackoverflow.com/questions/12494447/can-d3s-transition-and-rotation-play-well-together
function convertBusPositionAndBearing(d) {
    return "translate(-9,-12) scale(0.85) rotate(" + d.bearing + ", 11, 15)";
}

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

function getBusColors(lineNumber, busRoutes) {
    return hexToRgb(getBusColourHex(lineNumber, busRoutes));
}

function getBusColourHex(lineNumber, busRoutes) {
    var routes = busRoutes["routes"];
    // iterate over each element in the array
    for (var i = 0; i < routes.length; i++) {
        // look for the entry with a matching `code` value
        if (routes[i].Number === lineNumber) {
            return routes[i].Colour;
        }
    }
    return "#0000FF"; //Return blue
}

function getBusTextColourHex(lineNumber, busRoutes) {
    var routes = busRoutes["routes"];
    // iterate over each element in the array
    for (var i = 0; i < routes.length; i++) {
        // look for the entry with a matching `code` value
        if (routes[i].Number === lineNumber) {
            return routes[i].ColourInverse;
        }
    }
    return "#FFFFFF"; //Return white
}




function getBusOpacity(d) {

    if (d["age"] > 180) {
        return 0.3;
    }
    return 1;
}

function getRouteVisibility(busRoute) {
    var selected = getUserPreferencesForRoutesFromStorageOrCheckboxes().split(',');
    if (selected.indexOf(busRoute["Number"]) === -1) {
        return "visible";
    } else {
        return "hidden";
    }
}

function getBusVisibility(bus) {
    var selected = getUserPreferencesForRoutesFromStorageOrCheckboxes().split(',');

    if (bus.cat === "School Bus") {
        if (selected.indexOf("school") === -1) {
            return "visible";
        } else {
            return "hidden";
        }
    }

    if (selected.indexOf(bus.line) === -1) {
        return "visible";
    } else {
        return "hidden";
    }
}

function convertTime(busAge) {

    if (busAge < 180) {
        return busAge + " seconds ago";
    } else if (busAge < 240) {
        return "3 mins ago";
    } else if (busAge < 300) {
        return "4 mins ago";
    }

    return "Over 5 mins ago";
}



