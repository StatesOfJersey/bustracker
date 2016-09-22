

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
            .style("stroke", d3.rgb(59, 63, 65))
            .style("stroke-width", "1")
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
                return getBusTextColors(d["line"]);
            }))
            .text(function (d) { return d.line; });

        // Update text when bus changes line e.g 5 becomes 7
        points.select('text')
               .style('fill', (function (d) {
                   return getBusTextColors(d["line"]);
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

    stopsEnter.append('rect')
       .attr("width", "1px")
       .attr("height", "20px")
       .attr("x", "0")
       .attr("y", "-20")
       .style('fill', d3.rgb(136, 136, 136));

    stopsEnter.append('circle')
        .attr("r", "8")
        .attr("cx", "1")
        .attr("cy", "-20")
        .style("stroke", "black")
        .style("stroke-width", "1")
        .style("fill", d3.rgb(227, 44, 25));

    var stopsExit = stops.exit().remove();
}


function drawRoute(routedata) {

    var routes = svg.selectAll('.route');
    routes.remove(); //Clear existing
    if (!isDisplayRoutesOnMapChecked()) {
        return;
    }

    var mode = (zoom > 14)? "linear" : "basis";
    var zoom = map.getZoom();


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
    var lastRouteNumber;
    var lineStyle;


    for (var i = 0; i < routes.length; i++) {
        var route = routes[i];
        routeColour = route["Colour"];


        if (route["direction"] == "I")
            console.log("inbound route found");
        
        if (route["RouteCoordinates"] != null && getRouteVisibility(route) === 'visible') {
            var lastSection;
            var sectionBoundaries = [0];

            for (var j = 0; j < route["RouteCoordinates"].length; j++) {
                var coordinateItem = route["RouteCoordinates"][j];

                if (coordinateItem["splitSection"] != null && lastSection != coordinateItem["splitSection"]) {
                    lastSection = coordinateItem["splitSection"];
                    sectionBoundaries.push([j]);
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
                     .attr("d", lineFunction(route["RouteCoordinates"].slice(sectionBoundaries[k], sectionBoundaries[k+1])))
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
        //http://sojbuslivetimespublic.azurewebsites.net/api/Values/BusStop/
        d3.json("http://uat-sojbuslivetimespublic.azurewebsites.net//api/Values/BusStop/" + d["StopNumber"], function (data) {
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

//Hack: how do we do this? need some rules or foreground from API | Change Text color for light backgrounds
function getBusTextColors(d) {

    if (d === "23" || d === "5" || d === "1A" || d === "1GO" || d === "12A") {
        return d3.rgb(59, 63, 65);
    } else {
        return "white";
    }
}



function getBusOpacity(d) {

    if (d["age"] > 180) {
        return 0.3;
    }
    return 1;
}

function getRouteVisibility(busRoute) {
    var selected = getSelectionData().split(',');
    if (selected.indexOf(busRoute["Number"]) === -1) {
        return "hidden";
    } else {
        return "visible";
    }
}

function getBusVisibility(bus) {
    var selected = getSelectionData().split(',');

    if (bus.cat === "School Bus") {
        if (selected.indexOf("school") === -1) {
            return "hidden";
        } else {
            return "visible";
        }
    }

    if (selected.indexOf(bus.line) === -1) {
        return "hidden";
    } else {
        return "visible";
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



