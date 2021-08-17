begin try
	drop table #DeadLocks
end try
begin catch
end catch

create table #DeadLocks
(
	SpId bigint,
	DatabaseId bigint,
	ObjectId bigint,
	IndId bigint,
	[Type] varchar(100),
	[Resource] varchar(100),
	Mode varchar(100),
	[Status] varchar(100)
)

insert into #DeadLocks
execute sp_lock

select SpId, DatabaseId, db_name(DatabaseId), object_name(ObjectId), [Type]
from #DeadLocks