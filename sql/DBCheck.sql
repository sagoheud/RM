Select * from tblMain;
Select * from tblDetails;

DBCC CHECKIDENT(tblMain, reseed, 0)
DBCC CHECKIDENT(tblDetails, reseed, 0)