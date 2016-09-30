using Microsoft.WindowsAzure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;
using Tracking_Common;

namespace Tracking_API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        private static Lazy<ConnectionMultiplexer> lazyRedisConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(CloudConfigurationManager.GetSetting("Redis.Cache.Key"));
        });

        public static ConnectionMultiplexer RedisConnection
        {
            get
            {
                return lazyRedisConnection.Value;
            }
        }

        private const int BUS_CACHE_TIMESPAN_SECONDS = 1200; //1200 = 20 minutes
        private const int ROUTE_CACHE_TIMESPAN_SECONDS = 43200;
        private const int STOP_CACHE_TIMESPAN_SECONDS = 43200; //43200s = 12 hour cache since bus stops do not change that often
        private const int STOP_ETA_CACHE_TIMESPAN_SECONDS = 5; //we don't want to cache this very much at all but we do want to prevent a denial of service attack on the underlying data source

        #region v1 feed provided for stability for app developers to rely on an exact feed version
        /// <summary>
        /// Get information for a specific bus stop, v1 feed provided for stability for app developers to rely on an exact feed version
        /// </summary>
        /// <param name="id">The bus stop ID (as painted on the road for the TextMyBus service)</param>
        /// <returns>Information on estimated time of arrival for various buses at a given stop</returns>
        [HttpGet]
        [Route("api/Values/v1/BusStop/{id:int}")]
        public HttpResponseMessage BusStopv1(int id)
        {
            return BusStop(id); //provided for stability for app developers to rely on an exact feed version
        }

        /// <summary>
        /// Get bus locations and bus stops (if lat and lon provided) within x metres of the provided location, v1 feed provided for stability for app developers to rely on an exact feed version
        /// </summary>
        /// <param name="secondsAgo">The number of seconds to look back for updates</param>
        /// <param name="lat">The latitude of the current user's location</param>
        /// <param name="lon">The longtitude of the current user's location</param>
        /// <param name="stopsWithinXMetres">The number of metres within which to return bus stops</param>
        /// <param name="limitTo">The maximum number of bus stops to return in the update, in case the radius is too large</param>
        /// <returns>An array of bus locations and an optional array of bus stops</returns>
        [HttpGet]
        [Route("api/Values/v1/{secondsAgo:int?}/{lat:decimal?}/{lon:decimal?}/{stopsWithinXMetres:int?}/{limitTo:int?}")]
        public HttpResponseMessage Getv1(int? secondsAgo = null, decimal? lat = null, decimal? lon = null, int? stopsWithinXMetres = 500, int? limitTo = 10)
        {
            return Get(secondsAgo, lat, lon, stopsWithinXMetres, limitTo); //provided for stability for app developers to rely on an exact feed version
        }


        /// <summary>
        /// Get a minimum information version of bus locations and bus stops (if lat and lon provided) within x metres of the provided location, v1 feed provided for stability for app developers to rely on an exact feed version
        /// </summary>
        /// <param name="secondsAgo">The number of seconds to look back for updates</param>
        /// <param name="lat">The latitude of the current user's location</param>
        /// <param name="lon">The longtitude of the current user's location</param>
        /// <param name="stopsWithinXMetres">The number of metres within which to return bus stops</param>
        /// <param name="limitTo">The maximum number of bus stops to return in the update, in case the radius is too large</param>
        /// <returns>An array of bus locations and an optional array of bus stops</returns>
        [HttpGet]
        [Route("api/Values/v1/GetMin/{secondsAgo:int?}/{lat:decimal?}/{lon:decimal?}/{stopsWithinXMetres:int?}/{limitTo:int?}")]
        public HttpResponseMessage GetMinv1(int? secondsAgo = null, decimal? lat = null, decimal? lon = null, int? stopsWithinXMetres = 500, int? limitTo = 10)
        {
            return GetMin(secondsAgo, lat, lon, stopsWithinXMetres, limitTo); //provided for stability for app developers to rely on an exact feed version
        }

        /// <summary>
        /// Get the list of routes available, v1 feed provided for stability for app developers to rely on an exact feed version
        /// </summary>
        /// <returns>Route numebrs, colours and names</returns>
        [HttpGet]
        [Route("api/Values/v1/GetRoutes")]
        public HttpResponseMessage GetRoutesv1()
        {
            return GetRoutes(); //provided for stability for app developers to rely on an exact feed version
        }

        #endregion

        /// <summary>
        /// Get information for a specific bus stop
        /// </summary>
        /// <param name="id">The bus stop ID (as painted on the road for the TextMyBus service)</param>
        /// <returns>Information on estimated time of arrival for various buses at a given stop</returns>
        [HttpGet]
        [Route("api/Values/BusStop/{id:int}")]
        public HttpResponseMessage BusStop(int id)
        {

            MemoryCache mc = MemoryCache.Default;
            var busETAs = mc[StopInformationkey(id)] as List<BusETA>;
            if (busETAs == null)
            {
                WebClient wc = new WebClient();
                byte[] raw = wc.DownloadData("http://jersey.connect.vixtechnology.com/Text/WebDisplay.aspx?stopRef=" + id.ToString());

                string webData = System.Text.Encoding.UTF8.GetString(raw);

                // var gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                // var gmtDateTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, gmtZone).AddHours(-1);
                busETAs = BusETA.ConvertVixXhtmlToBusETAs(webData, DateTime.Now, id);

                mc.Add(StopInformationkey(id), busETAs, DateTimeOffset.UtcNow.AddSeconds(STOP_ETA_CACHE_TIMESPAN_SECONDS));
            }

            return Request.CreateResponse(HttpStatusCode.OK, busETAs);
        }

        /// <summary>
        /// Get bus locations and bus stops (if lat and lon provided) within x metres of the provided location
        /// </summary>
        /// <param name="secondsAgo">The number of seconds to look back for updates</param>
        /// <param name="lat">The latitude of the current user's location</param>
        /// <param name="lon">The longtitude of the current user's location</param>
        /// <param name="stopsWithinXMetres">The number of metres within which to return bus stops</param>
        /// <param name="limitTo">The maximum number of bus stops to return in the update, in case the radius is too large</param>
        /// <returns>An array of bus locations and an optional array of bus stops</returns>
        [HttpGet]
        [Route("api/Values/{secondsAgo:int?}/{lat:decimal?}/{lon:decimal?}/{stopsWithinXMetres:int?}/{limitTo:int?}")]
        public HttpResponseMessage Get(int? secondsAgo = null, decimal? lat = null, decimal? lon = null, int? stopsWithinXMetres = 500, int? limitTo = 10)
        {
            List<AssetLocationUpdate> updates = GetUpdates(secondsAgo);

            List<BusStop> stops = GetStops(lat, lon, stopsWithinXMetres, limitTo);

            return Request.CreateResponse(HttpStatusCode.OK,
                new
                {
                    updates,
                    stops
                });
        }

        /// <summary>
        /// Get a minimum information version of bus locations and bus stops (if lat and lon provided) within x metres of the provided location
        /// </summary>
        /// <param name="secondsAgo">The number of seconds to look back for updates</param>
        /// <param name="lat">The latitude of the current user's location</param>
        /// <param name="lon">The longtitude of the current user's location</param>
        /// <param name="stopsWithinXMetres">The number of metres within which to return bus stops</param>
        /// <param name="limitTo">The maximum number of bus stops to return in the update, in case the radius is too large</param>
        /// <returns>An array of bus locations and an optional array of bus stops</returns>
        [HttpGet]
        [Route("api/Values/GetMin/{secondsAgo:int?}/{lat:decimal?}/{lon:decimal?}/{stopsWithinXMetres:int?}/{limitTo:int?}")]
        public HttpResponseMessage GetMin(int? secondsAgo = null, decimal? lat = null, decimal? lon = null, int? stopsWithinXMetres = 500, int? limitTo = 10)
        {
            List<AssetLocationUpdate> updates = GetUpdates(secondsAgo);

            List<mALU> minimumInfoUpdates = updates.ConvertAll(x => new mALU(x));
            List<BusStop> stops = GetStops(lat, lon, stopsWithinXMetres, limitTo);

            return Request.CreateResponse(HttpStatusCode.OK,
                new
                {
                    minimumInfoUpdates,
                    stops
                });
        }

        /// <summary>
        /// Get the list of routes available
        /// </summary>
        /// <returns>Route numebrs, colours and names</returns>
        [HttpGet]
        [Route("api/Values/GetRoutes")]
        public HttpResponseMessage GetRoutes()
        {
            var routes = GetRoutesOrCache();

            return Request.CreateResponse(HttpStatusCode.OK,
                new
                {
                    routes
                });
        }


        private List<BusStop> GetStops(decimal? lat, decimal? lon, int? stopsWithinXMetres, int? limitTo)
        {
            if (lat != null && lon != null && stopsWithinXMetres != null && limitTo != null)
            {
                MemoryCache mc = MemoryCache.Default;
                var stops = mc[StopCacheKey(lat, lon, stopsWithinXMetres, limitTo)] as List<BusStop>;
                if (stops == null)
                {
                    stops = getNearestStopsFromSqlAzure(lat.Value, lon.Value, stopsWithinXMetres.Value, limitTo.Value);
                    mc.Add(StopCacheKey(lat, lon, stopsWithinXMetres, limitTo), stops, DateTimeOffset.UtcNow.AddSeconds(STOP_CACHE_TIMESPAN_SECONDS));
                }

                return stops;
            }
            return new List<BusStop>();
        }

        private List<AssetLocationUpdate> getLiveLocationsFromSqlAzure(string assetType)
        {
            List<AssetLocationUpdate> results = new List<AssetLocationUpdate>();
            SqlConnection connection = new SqlConnection(CloudConfigurationManager.GetSetting("SQLAzure.ConnectionString"));
            SqlCommand command = new SqlCommand("getLiveLocations", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@AssetType", SqlDbType.VarChar, 50).Value = DBNull.Value;

            // Execute command.
            connection.Open();
            SqlDataReader rs = command.ExecuteReader();
            while (rs.Read())
            {
                AssetLocationUpdate update = new AssetLocationUpdate();
                update.DeviceId = (string)rs["DeviceId"];
                update.AssetType = (string)rs["AssetType"];
                update.AssetRegistrationNumber = (string)rs["AssetRegistrationNumber"];
                update.ServiceNumber = (string)rs["ServiceNumber"];
                update.ServiceName = (string)rs["ServiceName"];
                update.ServiceOperator = (string)rs["ServiceOperator"];
                update.OriginalStartTime = (DateTime)rs["OriginalStartTime"];
                update.TimeOfUpdate = (DateTime)rs["TimeOfUpdate"];
                update.Direction = (string)rs["Direction"];
                update.Latitude = (decimal)rs["Latitude"];
                update.Longitude = (decimal)rs["Longitude"];
                update.Bearing = (rs["Bearing"] == DBNull.Value ? null : (int?)int.Parse(rs["Bearing"].ToString()));
                results.Add(update);
            }
            connection.Close();

            return results;
        }

        private List<BusStop> getNearestStopsFromSqlAzure(decimal lat, decimal lon, int withinXMetres, int limitTo)
        {
            List<BusStop> results = new List<BusStop>();
            SqlConnection connection = new SqlConnection(CloudConfigurationManager.GetSetting("SQLAzure.ConnectionString"));
            SqlCommand command = new SqlCommand("[FindNearestStops]", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@Latitude", SqlDbType.Decimal).Value = lat;
            command.Parameters.Add("@Longitude", SqlDbType.Decimal).Value = lon;
            command.Parameters.Add("@withinXMetres", SqlDbType.Decimal).Value = withinXMetres;
            command.Parameters.Add("@limitTo", SqlDbType.Decimal).Value = limitTo;

            // Execute command.
            connection.Open();
            SqlDataReader rs = command.ExecuteReader();
            while (rs.Read())
            {
                BusStop stop = new BusStop();
                stop.StopName = (string)rs["StopName"];
                stop.StopNumber = (int)rs["StopNumber"];
                stop.Latitude = (decimal)rs["Latitude"];
                stop.Longitude = (decimal)rs["Longitude"];
                stop.DistanceAwayInMetres = (int)rs["DistanceMetres"];
                results.Add(stop);
            }
            connection.Close();

            return results;
        }

        private List<BusRoute> getRoutesFromSqlAzure()
        {
            var results = new List<BusRoute>();
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting("SQLAzure.ConnectionString")))
            {
                var command = new SqlCommand("[getRecentBuses]", connection) { CommandType = CommandType.StoredProcedure };
                connection.Open();
                var rs = command.ExecuteReader();
                while (rs.Read())
                {
                    results.Add(new BusRoute
                    {
                        Id = (int)rs["Id"],
                        Number = (string)rs["servicenumber"],
                        Name = (string)rs["serviceName"],
                        Colour = (string)rs["routeColour"],
                        ColourInverse = (string)rs["routeColourInverse"],
                        Active = (bool)rs["active"]
                    });
                }

                //Get the route coordinates
                foreach (var busRoute in results)
                {
                    var cmdCoordinates = new SqlCommand("[getRouteCoordinates]", connection) { CommandType = CommandType.StoredProcedure };
                    cmdCoordinates.Parameters.Add("@RouteId", SqlDbType.BigInt).Value = busRoute.Id;
                    using (var rs2 = cmdCoordinates.ExecuteReader())
                    {
                        if (rs2.Read())
                        {
                            busRoute.RouteCoordinates =
                                JsonConvert.DeserializeObject<List<RouteCoordinateDto>>((string)rs2["RouteCoordinates"]);
                        }
                        rs2.Close();
                    }
                }
                connection.Close();
            }
            return results;
        }

        private List<AssetLocationUpdate> GetUpdates(int? secondsAgo)
        {
            MemoryCache mc = MemoryCache.Default;

            try
            {
                var lastUpdate = mc[LastUpdatekey()] as string;

                IDatabase cache = RedisConnection.GetDatabase();
                var lastUpdateFromRedis = cache.StringGet("bus:lastUpdate");

                if (lastUpdateFromRedis != RedisValue.Null && lastUpdateFromRedis != lastUpdate)
                {
                    //we have a new bus update or the cache time has not been set at all
                    mc.Set(LastUpdatekey(), lastUpdateFromRedis.ToString(), DateTimeOffset.UtcNow.AddSeconds(BUS_CACHE_TIMESPAN_SECONDS));
                    mc.Remove(BusCachekey());
                }
            }
            catch (Exception ex)
            {
                //do nothing
            }

            var updates = mc[BusCachekey()] as List<AssetLocationUpdate>;
            if (updates == null)
            {
                updates = getLiveLocationsFromSqlAzure(null);
                mc.Add(BusCachekey(), updates, DateTimeOffset.UtcNow.AddSeconds(BUS_CACHE_TIMESPAN_SECONDS));
            }

            updates = updates.Where(t => (secondsAgo == null || t.SecondsAgo <= secondsAgo)).ToList();
            return updates;
        }

        private List<BusRoute> GetRoutesOrCache()
        {
            MemoryCache mc = MemoryCache.Default;
            var routes = mc[Routeskey()] as List<BusRoute>;
            if (routes == null)
            {
                routes = getRoutesFromSqlAzure();
                mc.Add(Routeskey(), routes, DateTimeOffset.UtcNow.AddSeconds(ROUTE_CACHE_TIMESPAN_SECONDS));
            }
            return routes;
        }


        private string BusCachekey()
        {
            return "buses";
        }

        private string Routeskey()
        {
            return "routes";
        }
        private string StopInformationkey(int id)
        {
            return string.Format("stopNumber-{0}", id);
        }
        private string LastUpdatekey()
        {
            return "lastUpdate";
        }

        private string StopCacheKey(decimal? lat = null, decimal? lon = null, int? stopsWithinXMetres = 500, int? limitTo = 10)
        {
            return string.Format("stops-lat={0},lon={1},stopsM={2},limit={3}", lat, lon, stopsWithinXMetres, limitTo);
        }
    }
}
