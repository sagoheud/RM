

create table product
(
pID int primary key identity,
pName varchar(50),
pPrice float,
categoryID int,
pImage image
)

select pID,pName,pPrice,categoryID,c.catName from product p inner join category c on c.catID = p.categoryID

--DBCC CHECKIDENT(product, reseed, 0)

