
select COUNT(*) 
from master.dbo.sysprocesses p
join master.dbo.sysdatabases d on p.dbID = d.dbID
where d.name = 'AdventureWorks'

SELECT spid, uid=rtrim(loginame), Program_name=rtrim(Program_name),
dbname=db_name(dbid), status=rtrim(status) FROM master.dbo.sysprocesses 
WHERE loginame = 'test';
