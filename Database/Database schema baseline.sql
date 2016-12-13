USE [busdata]
GO
/****** Object:  Table [dbo].[BusStops]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BusStops](
	[StopNumber] [int] NOT NULL,
	[StopName] [nvarchar](50) NULL,
	[Latitude] [decimal](9, 7) NOT NULL,
	[Longitude] [decimal](9, 7) NOT NULL,
	[DBTimeOfUpdate] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Bearing] [nvarchar](50) NULL,
	[p]  AS ([geography]::Point([Latitude],[Longitude],(4326))) PERSISTED,
 CONSTRAINT [pk_StopNumber] PRIMARY KEY CLUSTERED 
(
	[StopNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[busSubscription]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[busSubscription](
	[subscriptionid] [int] IDENTITY(1,1) NOT NULL,
	[subscriptionStatus] [bit] NOT NULL,
	[DBTimeOfUpdate] [datetime] NOT NULL DEFAULT (getutcdate()),
	[subscriptionXml] [nvarchar](max) NOT NULL,
 CONSTRAINT [pk_subscriptionid] PRIMARY KEY CLUSTERED 
(
	[subscriptionid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[liveLocations]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[liveLocations](
	[DeviceId] [nvarchar](50) NOT NULL,
	[AssetType] [nvarchar](50) NOT NULL,
	[AssetRegistrationNumber] [nvarchar](50) NULL,
	[ServiceNumber] [nvarchar](50) NULL,
	[ServiceName] [nvarchar](50) NULL,
	[ServiceOperator] [nvarchar](50) NOT NULL,
	[TimeOfUpdate] [datetime] NOT NULL,
	[DBTimeOfUpdate] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Direction] [nvarchar](50) NULL,
	[Latitude] [decimal](9, 7) NOT NULL,
	[Longitude] [decimal](9, 7) NOT NULL,
	[Bearing] [nvarchar](50) NULL,
	[originalStartTime] [datetime] NULL,
	[p]  AS ([geography]::Point([Latitude],[Longitude],(4326))) PERSISTED,
 CONSTRAINT [pk_deviceId] PRIMARY KEY CLUSTERED 
(
	[DeviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[liveLocationsArchive]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[liveLocationsArchive](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceId] [nvarchar](50) NOT NULL,
	[AssetType] [nvarchar](50) NOT NULL,
	[AssetRegistrationNumber] [nvarchar](50) NULL,
	[ServiceNumber] [nvarchar](50) NULL,
	[ServiceName] [nvarchar](50) NULL,
	[ServiceOperator] [nvarchar](50) NOT NULL,
	[TimeOfUpdate] [datetime] NOT NULL,
	[DBTimeOfUpdate] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Direction] [nvarchar](50) NULL,
	[Latitude] [decimal](9, 7) NOT NULL,
	[Longitude] [decimal](9, 7) NOT NULL,
	[Bearing] [nvarchar](50) NULL,
	[originalStartTime] [datetime] NULL,
	[p]  AS ([geography]::Point([Latitude],[Longitude],(4326))) PERSISTED,
 CONSTRAINT [pk_deviceIdArchive] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[recentBearing]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[recentBearing](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[AssetRegistrationNumber] [nvarchar](50) NULL,
	[DBTimeOfUpdate] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Bearing] [nvarchar](50) NULL,
 CONSTRAINT [pk_Id] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[routeCoordinates]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[routeCoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RouteId] [int] NOT NULL,
	[Latitude] [decimal](9, 7) NOT NULL,
	[Longitude] [decimal](9, 7) NOT NULL,
	[IsVital] [bit] NOT NULL,
	[p]  AS ([geography]::Point([Latitude],[Longitude],(4326))) PERSISTED,
	[Occasional] [bit] NOT NULL DEFAULT ((0)),
	[Direction] [char](1) NOT NULL DEFAULT ('O'),
	[SplitSection] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RouteDirections]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RouteDirections](
	[Direction] [char](1) NOT NULL,
	[DirectionType] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Direction] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[routes]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[routes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServiceNumber] [nvarchar](50) NULL,
	[ServiceName] [nvarchar](255) NULL,
	[RouteColour] [nvarchar](50) NOT NULL,
	[RouteColourInverse] [nvarchar](50) NOT NULL DEFAULT ('#FFFFFF'),
	[Active] [bit] NOT NULL DEFAULT ((1)),
 CONSTRAINT [pk_routeId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Index [ix_timeOfUpdate]    Script Date: 13/12/2016 12:00:00 ******/
CREATE NONCLUSTERED INDEX [ix_timeOfUpdate] ON [dbo].[liveLocationsArchive]
(
	[TimeOfUpdate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [dbo].[routeCoordinates]  WITH CHECK ADD FOREIGN KEY([Direction])
REFERENCES [dbo].[RouteDirections] ([Direction])
GO
ALTER TABLE [dbo].[routeCoordinates]  WITH CHECK ADD  CONSTRAINT [fk_RouteId] FOREIGN KEY([RouteId])
REFERENCES [dbo].[routes] ([Id])
GO
ALTER TABLE [dbo].[routeCoordinates] CHECK CONSTRAINT [fk_RouteId]
GO
/****** Object:  StoredProcedure [dbo].[addLiveLocaton]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[addLiveLocaton] (
	@DeviceId nvarchar(50) ,
	@AssetType nvarchar(50) ,
	@AssetRegistrationNumber nvarchar(50) null,
	@ServiceNumber nvarchar(50) null,
	@ServiceName nvarchar(50) null,
	@ServiceOperator nvarchar(50) ,
	@OriginalStartTime nvarchar(50) null,
	@TimeOfUpdate datetime ,
	@Direction nvarchar(50) null,
	@Latitude decimal(9, 7) ,
	@Longitude decimal(9, 7) ,
	@Bearing nvarchar(50) null) as
begin

	if exists(select 1 from [dbo].[liveLocations] where assetRegistrationNumber = @assetRegistrationNumber   and 
					(serviceNumber != @serviceNumber or direction != @direction) )
	begin
		-- this bus has turned around and we need to clear the recent bearings
		delete [recentBearing] where assetRegistrationNumber = @assetRegistrationNumber
	end
	
	INSERT INTO [dbo].[liveLocationsArchive]
		([DeviceId]
		,[AssetType]
		,[AssetRegistrationNumber]
		,[ServiceNumber]
		,[ServiceName]
		,[ServiceOperator]
		,[OriginalStartTime]
		,[TimeOfUpdate]
		,[Direction]
		,[Latitude]
		,[Longitude]
		,[Bearing])
	VALUES
		(@DeviceId
		,@AssetType
		,@AssetRegistrationNumber
		,@ServiceNumber
		,@ServiceName
		,@ServiceOperator
		,@OriginalStartTime
		,@TimeOfUpdate
		,@Direction
		,@Latitude
		,@Longitude
		,@Bearing)

	delete from liveLocationsArchive where timeOfUpdate < dateadd(day, -7, getdate())
		
	if exists(select 1 from [dbo].[liveLocations] where 
					assetRegistrationNumber = @assetRegistrationNumber )

	begin
		-- if we've already seen this bus before then update the location
		update [dbo].[liveLocations]
		   set [AssetType] = @AssetType
			  ,[AssetRegistrationNumber] = @AssetRegistrationNumber
			  ,[ServiceNumber] = @ServiceNumber
			  ,[ServiceName] = @ServiceName
			  ,[ServiceOperator] = @ServiceOperator
			  ,[OriginalStartTime] = @OriginalStartTime
			  ,[TimeOfUpdate] = @TimeOfUpdate
			  ,[DBTimeOfUpdate] = getutcdate()
			  ,[Direction] = @Direction
			  ,[Latitude] = @Latitude
			  ,[Longitude] = @Longitude
			  ,[Bearing] = IIF(@Bearing = -1, bearing, @bearing)
		 WHERE assetRegistrationNumber = @assetRegistrationNumber
		 and @TimeOfUpdate > TimeOfUpdate

	end else
	begin
		-- it's a new bus so let's add the location
		INSERT INTO [dbo].[liveLocations]
			   ([DeviceId]
			   ,[AssetType]
			   ,[AssetRegistrationNumber]
			   ,[ServiceNumber]
			   ,[ServiceName]
			   ,[ServiceOperator]
			   ,[OriginalStartTime]
			   ,[TimeOfUpdate]
			   ,[Direction]
			   ,[Latitude]
			   ,[Longitude]
			   ,[Bearing])
		 VALUES
			   (@DeviceId
			   ,@AssetType
			   ,@AssetRegistrationNumber
			   ,@ServiceNumber
			   ,@ServiceName
			   ,@ServiceOperator
			   ,@OriginalStartTime
			   ,@TimeOfUpdate
			   ,@Direction
			   ,@Latitude
			   ,@Longitude
			   ,@Bearing)
	end

	if @bearing >= 0 
	begin
		insert into recentBearing (assetRegistrationNumber, bearing) values (@AssetRegistrationNumber, @Bearing)
	end 

	--finally let's get rid of any old updates, old being > 40 minutes
	--(the thinking behind this is that these should definitely be yesterday's buses or very much out of date today)
	delete [dbo].[liveLocations] 
	where ([DBTimeOfUpdate] < dateadd(minute, -40, getutcdate()))
	or (assetRegistrationNumber = @AssetRegistrationNumber and 
			(serviceNumber != @serviceNumber or direction != @direction) 
	   )
end

GO
/****** Object:  StoredProcedure [dbo].[addSubscriptionInformation]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[addSubscriptionInformation]
(@subscriptionStatus bit,
 @subscriptionXml nvarchar(max) )
 as
 begin
	insert into [busSubscription] (subscriptionStatus, subscriptionXml) values (@subscriptionStatus, @subscriptionXml)
 end

 
GO
/****** Object:  StoredProcedure [dbo].[FindNearestBus]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[FindNearestBus]
@Latitude DECIMAL(9,7),
@Longitude DECIMAL(9,7)
AS
BEGIN
SET NOCOUNT ON;

DECLARE @GeogTemp NVARCHAR(200)
DECLARE @Geog GEOGRAPHY

-- Create a geography type variable from the passed in latitude and longitude. This will be used to find the closest point in the database
SET @GeogTemp = 'POINT(' + cast(  @Longitude as NVARCHAR(100)) + ' ' + convert(NVARCHAR(100), @Latitude) + ')'

SET @Geog = geography::STGeomFromText(@GeogTemp, 4326)

-- Run the main query
SELECT
    *,
    [p].STDistance(@Geog) AS DistanceMetres
FROM 
    [dbo].[livelocations] WITH(INDEX([livelocations_SpatialIndex]))
WHERE 
    [p].STDistance(@Geog) < 100000     -- 100 KM 100000 
ORDER BY
    [p].STDistance(@Geog) ASC
END


GO
/****** Object:  StoredProcedure [dbo].[FindNearestStops]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FindNearestStops]
@Latitude DECIMAL(9,7),
@Longitude DECIMAL(9,7),
@withinXMetres int,
@limitTo int
AS
BEGIN
SET NOCOUNT ON;

DECLARE @GeogTemp NVARCHAR(200)
DECLARE @Geog GEOGRAPHY

-- Create a geography type variable from the passed in latitude and longitude. This will be used to find the closest point in the database
SET @GeogTemp = 'POINT(' + cast(  @Longitude as NVARCHAR(100)) + ' ' + convert(NVARCHAR(100), @Latitude) + ')'

SET @Geog = geography::STGeomFromText(@GeogTemp, 4326)

-- Run the main query
SELECT top (@limitTo)
    [StopNumber],
	[StopName],
    [Latitude],
    [Longitude],
    [p],
    cast ([p].STDistance(@Geog) as int) AS DistanceMetres
FROM 
    [dbo].[busstops] WITH(INDEX([busstop_SpatialIndex]))
WHERE 
    [p].STDistance(@Geog) <= @withinXMetres    
ORDER BY
    [p].STDistance(@Geog) ASC
END

GO
/****** Object:  StoredProcedure [dbo].[getLiveLocations]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[getLiveLocations] (
	@AssetType nvarchar(50) null ) as
begin
	select
	   [DeviceId]
      ,[AssetType]
      ,[AssetRegistrationNumber]
      ,[ServiceNumber]
      ,[ServiceName]
      ,[ServiceOperator]
      ,[TimeOfUpdate]
      ,[DBTimeOfUpdate]
      ,[Direction]
      ,[Latitude]
      ,[Longitude]
	  ,IIF([dbo].[liveLocations].[bearing] = -1, 0, Bearing) as Bearing
      ,[originalStartTime]
      ,[p]
	from [dbo].[liveLocations]
	where (@AssetType is null or AssetType = @AssetType)
end

GO
/****** Object:  StoredProcedure [dbo].[getRecentBuses]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



  CREATE procedure [dbo].[getRecentBuses]
  as
  begin
	Select Id, 
		ServiceNumber as servicenumber, 
		ServiceName as serviceName, 
		RouteColour as routeColour, 
		RouteColourInverse as routeColourInverse,
		Active as active
	from [dbo].[routes]
	order by ServiceNumber
  end





GO
/****** Object:  StoredProcedure [dbo].[getRouteCoordinates]    Script Date: 13/12/2016 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[getRouteCoordinates]
	@RouteId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Select @RouteId RouteId, (Select Latitude as lat, Longitude as lon, IsVital as isVital, Occasional as occasional, Direction as direction, SplitSection as splitSection
	FROM routeCoordinates ST1
	Where ST1.RouteId = @RouteId
	ORDER BY RouteId, Id
	For JSON AUTO) RouteCoordinates

END

GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
/****** Object:  Index [busstop_SpatialIndex]    Script Date: 13/12/2016 12:00:00 ******/
CREATE SPATIAL INDEX [busstop_SpatialIndex] ON [dbo].[BusStops]
(
	[p]
)USING  GEOGRAPHY_AUTO_GRID 
WITH (
CELLS_PER_OBJECT = 12, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
/****** Object:  Index [livelocations_SpatialIndex]    Script Date: 13/12/2016 12:00:00 ******/
CREATE SPATIAL INDEX [livelocations_SpatialIndex] ON [dbo].[liveLocations]
(
	[p]
)USING  GEOGRAPHY_AUTO_GRID 
WITH (
CELLS_PER_OBJECT = 12, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
