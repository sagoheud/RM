

create table tblMain
(
MainID int Primary key identity,
aData date,
aTime Varchar(15),
TableName varchar(10),
WaiterName varchar(15),
status varchar(15),
orderType varchar(15),
total float,
received float,
change float
)

--DBCC CHECKIDENT(tblMain, reseed, 0)

create table tblDetails
(
DetailID int Primary key identity,
MainID int,
proID int,
qty int,
price float,
amount float
)

--DBCC CHECKIDENT(tblDetails, reseed, 0)