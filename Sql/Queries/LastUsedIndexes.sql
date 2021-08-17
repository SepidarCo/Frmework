select db_name(database_id) as [Database],
	quotename(object_schema_name([object_id])) + '.' + quotename(object_name([object_id])) as ObjectName,
	max(case when last_user_update < last_system_update then last_system_update else last_user_update end) as LastUpdate
from sys.dm_db_index_usage_stats 
where db_name(database_id) not in 
(
	'master',
	'tempdb',
	'model',
	'msdb'
)
group by database_id, [object_id]
order by db_name(database_id), object_name([object_id], database_id)