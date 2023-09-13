

create table category
(
catID int primary key identity,
catName varchar(50)
)

--DBCC CHECKIDENT(category, reseed, 0)