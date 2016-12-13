begin

	declare @groupId int
	insert into stopGroups (groupName) values ('LIBERTYSTATION')
	select @groupId = SCOPE_IDENTITY()
	
	insert into stopGroupStops (stopGroupId, stopId) 
		(select @groupId, stopnumber from busstops where stopname like '%stand%')

	insert into stopGroups (groupName) values ('AIRPORT')
	select @groupId = SCOPE_IDENTITY()
	
	insert into stopGroupStops (stopGroupId, stopId) 
		(select @groupId, stopnumber from busstops where stopname like '%Jersey Airport%')

end