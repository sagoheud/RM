

create table tables
(
tID int primary key identity,
tName varchar(15)
)

--DBCC CHECKIDENT(tables, reseed, 0)