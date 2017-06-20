
  alter procedure getStopsForDepartureCode
  (@groupName nvarchar(50))
  as 
  begin
	select sgs.stopid from stopGroupStops sgs
	join stopGroups sg on sg.id = sgs.stopGroupId
	where sg.groupName = @groupName
	order by sgs.stopId
  end
  go

  exec getStopsForDepartureCode 'LIBERATIONSTATION'

