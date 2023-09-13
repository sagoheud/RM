

create table staff
(
stfID int primary key identity,
stfName varchar(50),
stfPhone varchar(50),
stfRole varchar(50)
)

--DBCC CHECKIDENT(staff, reseed, 0)