--drop DATABASE QuanLyKhoHang
CREATE DATABASE QuanLyKhoHang
 go
 use QuanLyKhoHang
CREATE TABLE Category(
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	DisplayName nvarchar(max) NULL,
) 

GO
CREATE TABLE Customer(
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	DisplayName nvarchar(max) NULL,
	BirthDay datetime NULL,
	Sex nvarchar(10) NULL,
	Address nvarchar(max) NULL,
	Phone nvarchar(20) NULL,
	Email nvarchar(200) NULL,
	MoreInfo nvarchar(max) NULL,
	ContractDate datetime NULL,
	Status nvarchar(128) NULL,
	IsVisible int NULL DEFAULT 0,
	LinkImage nvarchar(MAX)NULL

)

GO

CREATE TABLE Position(
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	DisplayName nvarchar(max) NULL,
 )
GO
CREATE TABLE Supplier(
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	DisplayName nvarchar(max) NULL,
	Phone nvarchar(20) NULL,
	Address nvarchar(max) NULL,
	Email nvarchar(200) NULL,
	MoreInfo nvarchar(max) NULL,
	ContractDate datetime NULL,
	Status nvarchar(128) NULL,
	IsVisible int NULL DEFAULT 0,
 )
 GO
CREATE TABLE Unit(
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	DisplayName nvarchar(max) NULL,
)
GO
CREATE TABLE Object(
	Id nvarchar(128) NOT NULL PRIMARY KEY,
	DisplayName nvarchar(max) NULL,
	IdUnit int NULL,
	IdCategory int NOT NULL,
	InputPrice float NULL DEFAULT 0,
	OutputPrice float NULL DEFAULT 0,
	Count int NULL DEFAULT 0,
	IdPosition int NULL,
	LinkImage nvarchar(128) NULL,
	Status nvarchar(128) NULL,
	IsVisible int NULL DEFAULT 0,
	foreign key(IdUnit) references Unit(Id),
	foreign key(IdCategory) references Category(Id),
	foreign key(IdPosition) references Position(Id)
)
GO

CREATE TABLE UserRole(
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	DisplayName nvarchar(max) NOT NULL,
	RolePermision nvarchar(max) NOT NULL,
)
GO
CREATE TABLE Users(
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	DisplayName nvarchar(max) NULL,
	BirthDay datetime NULL,
	Sex nvarchar(10) NULL,
	Address nvarchar(max) NULL,
	Phone nvarchar(20) NULL,
	Email nvarchar(200) NULL,
	MoreInfo nvarchar(max) NULL,
	UserName nvarchar(100) NULL,
	Password nvarchar(max) NULL,
	IdRole int NOT NULL,
	Status nvarchar(128) NULL,
	IsVisible int NULL DEFAULT 0,
	foreign key (IdRole) references UserRole(Id)
)
GO

CREATE TABLE Input(
	Id nvarchar(128) NOT NULL PRIMARY KEY,
	IdUser int NOT NULL,
	IdSupplier int  NULL,
	DateInput datetime NULL,
	Discount float NULL DEFAULT 0,
	Payment nvarchar(128) NULL,
	TotalPrice float NULL DEFAULT 0,
	Note nvarchar(max) NULL,
	Status nvarchar(128) NULL,
	TotalObject int NULL DEFAULT 0,
	TotalQuantity int NULL DEFAULT 0,
	 foreign key (IdUser) references Users(Id),
	 foreign key (IdSupplier) references Supplier(Id)
)

GO
CREATE TABLE InputInfo(
	IdInput nvarchar(128) NOT NULL,
	IdObject nvarchar(128) NOT NULL,
	Price float NULL DEFAULT 0,
	Quantity int NULL DEFAULT 0,
	Discount float NULl DEFAULT 0,
	Stock int NULL DEFAULT 0
	
	primary key(IdInput,IdObject),
	foreign key (IdInput) references Input(Id),
	foreign key (IdObject) references Object(Id)
)
GO

CREATE TABLE Output(
	Id nvarchar(128) NOT NULL PRIMARY KEY,
	IdUser int NOT NULL,
	IdCustomer int  NULL,
	DateOutput datetime NULL,
	Discount float NULL DEFAULT 0,
	Payment nvarchar(128) NULL,
	TotalPrice float NULL DEFAULT 0,
	Note nvarchar(max) NULL,
	Status nvarchar(128) NULL,
	foreign key (IdUser) references Users(Id),
	foreign key (IdCustomer) references Customer(Id)
		
)
GO
CREATE TABLE OutputInfo(
	IdOutput nvarchar(128) NOT NULL,
	IdObject nvarchar(128) NOT NULL,
	Price float NULL DEFAULT 0,
	Quantity int NULL DEFAULT 0,
	Discount float NULL DEFAULT 0,
	Stock int NULL DEFAULT 0
	primary key(IdOutput,IdObject),
	foreign key (IdOutput) references Output(Id),
	foreign key (IdObject) references Object(Id)
) 
GO
INSERT Category ( DisplayName) VALUES ( N'Đồ ăn nhanh')
INSERT Category ( DisplayName) VALUES ( N'Nước ngọt')
INSERT Category ( DisplayName) VALUES ( N'Kẹo bánh')
INSERT Category ( DisplayName) VALUES ( N'Mỹ phẩm')
INSERT Category ( DisplayName) VALUES ( N'Văn phòng phẩm')


INSERT Position ( DisplayName) VALUES ( N'Kệ số 1')


INSERT Supplier (DisplayName, Phone, Address, Email, MoreInfo, ContractDate, Status, IsVisible) VALUES ( N'tuan khai', N'0123456789', N'q12', N'tuankhai123456@gmail.com', N'', CAST(N'2019-05-24T09:12:13.833' AS DateTime), N'Đang hoạt động', 0)
INSERT Supplier (DisplayName, Phone, Address, Email, MoreInfo, ContractDate, Status, IsVisible) VALUES ( N'to xuan', N'0123456666', N'q11', N'toxuan@gmail.com', N'', CAST(N'2019-05-24T10:07:24.823' AS DateTime), N'Đang hoạt động', 0)


INSERT Unit (DisplayName) VALUES ( N'Thùng')
INSERT Unit (DisplayName) VALUES ( N'Bao')
INSERT Unit (DisplayName) VALUES ( N'Chai')
INSERT Unit (DisplayName) VALUES ( N'Gói')


INSERT UserRole (DisplayName, RolePermision) VALUES ( N'Admin', N'12345678')
INSERT UserRole (DisplayName, RolePermision) VALUES ( N'Nhân viên', N'123')

INSERT Users ( DisplayName, BirthDay, Sex, Address, Phone, Email, MoreInfo, UserName, Password, IdRole, Status, IsVisible) VALUES ( N'Lê Tuấn Khải', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'Nam', N'', N'0375022255', N'', N'', N'admin', N'db69fc039dcbd2962cb4d28f5891aae1', 1, N'Đang hoạt động', 0)
INSERT Users ( DisplayName, BirthDay, Sex, Address, Phone, Email, MoreInfo, UserName, Password, IdRole, Status, IsVisible) VALUES ( N'Lê Tuấn Khôi', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'Nam', N'', N'0375022256', N'', N'', N'staff', N'978aae9bb6bee8fb75de3e4830a1be46', 2, N'Đang hoạt động', 0)

INSERT Object (Id, DisplayName, IdUnit, IdCategory, InputPrice, OutputPrice, Count, IdPosition, LinkImage, Status, IsVisible) VALUES (N'SP00001', N'Kẹo dynamite sôcola', 4, 3, 10000, 18000, 0, NULL, N'IMG_SP00001.png', N'Cho phép kinh doanh', 0)
INSERT Object (Id, DisplayName, IdUnit, IdCategory, InputPrice, OutputPrice, Count, IdPosition, LinkImage, Status, IsVisible) VALUES (N'SP00003', N'Lăn nách boss gold to 50ml', 3, 4, 30000, 58000, 0, NULL, N'IMG_SP00003.png', N'Cho phép kinh doanh', 0)
INSERT Object (Id, DisplayName, IdUnit, IdCategory, InputPrice, OutputPrice, Count, IdPosition, LinkImage, Status, IsVisible) VALUES (N'SP00004', N'Kẹo extar xylitol bac hà hũ 56g', NULL, 3, 20000, 22000, 0, NULL, N'IMG_SP00004.png', N'Ngừng kinh doanh', 0)
INSERT Object (Id, DisplayName, IdUnit, IdCategory, InputPrice, OutputPrice, Count, IdPosition, LinkImage, Status, IsVisible) VALUES (N'SP00005', N'Coca chai 390ml', NULL, 2, 6500, 7000, 0, NULL, N'IMG_SP00005.png', N'Cho phép kinh doanh', 0)

INSERT Customer ( DisplayName, BirthDay, Sex, Address, Phone, Email, MoreInfo, ContractDate, Status, IsVisible) VALUES ( N'Lê Tuấn Khải', CAST(N'1998-09-03T00:00:00.000' AS DateTime), N'Nam', N'q12', N'0123456777', N'tuankhai12345@gmail.com', NULL, CAST(N'2019-02-02T00:00:00.000' AS DateTime), N'Đang hoạt động', 0)

--INSERT Output (Id, IdUser, IdCustomer, DateOutput, Discount, Payment, TotalPrice, Note, Status) VALUES (N'HD00001', 2, 1, CAST(N'2019-05-25T00:00:00.000' AS DateTime), 0, N'Tiền mặt', 21000, NULL, N'Hoàn thành')


--INSERT Input (Id, IdUser, IdSupplier, DateInput, Discount, Payment, TotalPrice, Note, Status, TotalObject, TotalQuantity) VALUES (N'PN00001', 1, 1, CAST(N'2019-05-24T09:13:56.450' AS DateTime), 0, N'Tiền mặt', 10000, N'', N'Đã nhập hàng', 1, 1)
--INSERT Input (Id, IdUser, IdSupplier, DateInput, Discount, Payment, TotalPrice, Note, Status, TotalObject, TotalQuantity) VALUES (N'PN00002', 1, 2, CAST(N'2019-05-24T09:16:56.977' AS DateTime), 0, N'Tiền mặt', 100000, N'', N'Đã nhập hàng', 2, 4)
--INSERT Input (Id, IdUser, IdSupplier, DateInput, Discount, Payment, TotalPrice, Note, Status, TotalObject, TotalQuantity) VALUES (N'PN00003', 1, 2, CAST(N'2019-05-24T15:38:25.540' AS DateTime), 0, N'Chuyển khoản', 60000, N'', N'Đã nhập hàng', 1, 10)
--INSERT Input (Id, IdUser, IdSupplier, DateInput, Discount, Payment, TotalPrice, Note, Status, TotalObject, TotalQuantity) VALUES (N'PN00004', 1, 2, CAST(N'2019-05-24T15:41:22.827' AS DateTime), 0, N'Thẻ', 45000, N'', N'Đã nhập hàng', 1, 5)
--INSERT Input (Id, IdUser, IdSupplier, DateInput, Discount, Payment, TotalPrice, Note, Status, TotalObject, TotalQuantity) VALUES (N'PN00005', 1, 2, CAST(N'2019-05-24T15:45:46.423' AS DateTime), 0, N'Tiền mặt', 44000, N'', N'Đã nhập hàng', 1, 5)
--INSERT Input (Id, IdUser, IdSupplier, DateInput, Discount, Payment, TotalPrice, Note, Status, TotalObject, TotalQuantity) VALUES (N'PN00006', 1, 2, CAST(N'2019-05-24T15:56:19.600' AS DateTime), 0, N'Tiền mặt', 25000, N'', N'Đã nhập hàng', 1, 5)
--INSERT Input (Id, IdUser, IdSupplier, DateInput, Discount, Payment, TotalPrice, Note, Status, TotalObject, TotalQuantity) VALUES (N'PN00007', 1, 1, CAST(N'2019-05-24T16:03:24.103' AS DateTime), 0, N'Thẻ', 190000, N'', N'Đã nhập hàng', 1, 19)
--INSERT Input (Id, IdUser, IdSupplier, DateInput, Discount, Payment, TotalPrice, Note, Status, TotalObject, TotalQuantity) VALUES (N'PN00008', 1, 1, CAST(N'2019-05-24T21:17:06.713' AS DateTime), 1000, N'Tiền mặt', 80000, N'1234', N'Phiếu tạm', 2, 3)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00001', N'SP00001', 10000, 1, 0, 0)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00002', N'SP00003', 30000, 2, 0, 0)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00002', N'SP00004', 20000, 2, 0, 0)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00003', N'SP00005', 6000, 10, 0, 0)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00005', N'SP00005', 9000, 5, 1000, 0)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00006', N'SP00005', 5000, 5, 0, 0)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00007', N'SP00001', 10000, 19, 0, 0)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00008', N'SP00003', 30000, 2, 0, 0)
--INSERT InputInfo (IdInput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'PN00008', N'SP00004', 20000, 1, 0, 0)
--INSERT OutputInfo (IdOutput, IdObject, Price, Quantity, Discount, Stock) VALUES (N'HD00001', N'SP00005', 7000, 3, 0, 0)

go
create view uv_View_UserRole
as
select * from UserRole
go
create view uv_View_Supplier
as
select * from Supplier 
go
create view uv_View_Object
as
select * from Object
go
create view uv_View_Input
as
select * from Input
go
create view uv_View_Output
as
select * from Output
go
create view uv_View_Unit
as
select * from Unit
go
create view uv_View_Position
as
select * from Position
go

create view uv_View_InputInfo
as
select * from InputInfo
go
create view uv_View_OutputInfo
as
select * from OutputInfo
go
create view uv_View_Category
as
select * from Category
go
create view uv_View_Customer
as
select * from Customer
go
create view uv_View_User
as
select * from Users
go

--proceduce view các bảng
create proc usp_View_User
as
begin
select * from Users
end
go

create proc usp_View_CurrentUserRole
@Id int
as
begin
select * from UserRole where Id =@Id
end
go

create proc usp_View_CurrentUser
@Id int
as
begin
select * from Users where Id =@Id
end
go

create proc usp_View_Supplier
as
begin
select * from Supplier 
end
go

create proc usp_View_Customer
as
begin
select * from Customer 
end
go

create proc usp_View_Object
as
begin
select * from Object
end
go

create proc usp_View_Input
as
begin
select * from Input
end
go
create proc usp_View_Output
as
begin
select * from Output
end
go

create proc usp_View_Unit
as
begin
select * from Unit
end
go

create proc usp_View_Position
as
begin
select * from Position
end
go

create proc usp_View_InputInfo
as
begin
select * from InputInfo
end
go

create proc usp_View_OutputInfo
as
begin
select * from OutputInfo
end
go

create proc usp_View_Category
as
begin
select * from Category
end
go

create proc usp_Insert_Update_Supplier
@Id int,
@DisplayName nvarchar(MAX),
@Phone nvarchar(20),
@Address nvarchar(MAX),
@Email nvarchar(200),
@MoreInfo nvarchar(MAX),
@ContractDate datetime,
@Status nvarchar(128),
@IsVisible int
as 
begin
  if exists(select * from Supplier with (updlock, serializable)  where id = @Id )
   begin 
update Supplier set DisplayName=@DisplayName,Address=@Address,Phone=@Phone,Email=@Email,MoreInfo=@MoreInfo,ContractDate=@ContractDate,Status=@Status,IsVisible=@IsVisible where Id = @Id  
   end 
  else 
   begin 
insert into Supplier(DisplayName,Address,Phone,Email,MoreInfo,ContractDate,Status,IsVisible) values(@DisplayName,@Address,@Phone,@Email,@MoreInfo,@ContractDate,@Status,@IsVisible)
   end 
end
go

create proc usp_Select_Count_Supplier
as
  begin
declare @Total int
declare @Business int
select @Business = count(*)from Supplier where Status = N'Đang hoạt động' and IsVisible<>1
--waitfor delay '00:00:05'
select @Total = count(*) from Supplier where IsVisible<>1
select @Total  as Total, @Business as Business
  end
go
--create proc usp_Insert_Update_Supplier
--@Id int,
--@DisplayName nvarchar(MAX),
--@Phone nvarchar(20),
--@Address nvarchar(MAX),
--@Email nvarchar(200),
--@MoreInfo nvarchar(MAX),
--@ContractDate datetime,
--@Status nvarchar(128),
--@IsVisible int
--as 
--begin
--	--declare @countPhone int = 0
--	--declare @countEmail int = 0
--	--select @countPhone = count(*) from Supplier c where c.Phone = @Phone  and c.Id <> @Id and trim(c.Phone) <> '' and c.IsVisible <>1
--	--select @countEmail= count(*) from Supplier c where c.Email= @Email and c.Id <> @Id and trim(c.Email) <> '' and c.IsVisible <>1
--  if exists(select * from Supplier with (updlock, serializable)  where id = @Id )
--   begin 
--    --begin tran
--update Supplier set DisplayName=@DisplayName,Address=@Address,Phone=@Phone,Email=@Email,MoreInfo=@MoreInfo,ContractDate=@ContractDate,Status=@Status,IsVisible=@IsVisible where Id = @Id  
----     if (@countPhone <> 0)
----      begin
------waitfor delay '00:00:10'
----RAISERROR (N'Số điện thoại này đã tồn tại!',0,1)
----rollback tran
----      end
----     else if (@countEmail <> 0 )
----      begin
------waitfor delay '00:00:10'
----RAISERROR (N'Email này đã tồn tại!',0,1)
----rollback tran
----      end
----	 else commit tran
--   end 
--  else 
--   begin 
--    --begin tran 
--insert into Supplier(DisplayName,Address,Phone,Email,MoreInfo,ContractDate,Status,IsVisible) values(@DisplayName,@Address,@Phone,@Email,@MoreInfo,@ContractDate,@Status,@IsVisible)
----	 if (@countPhone <> 0)
----      begin
------waitfor delay '00:00:10'
----RAISERROR (N'Số điện thoại này đã tồn tại!',0,1)
----rollback tran
----      end
----	 else if (@countEmail <> 0 )
----      begin
------waitfor delay '00:00:10'
----RAISERROR (N'Email này đã tồn tại!',0,1)
----rollback tran
----      end
----	else commit tran
--   end 
--end
--go

--Insert Update Customer
--create proc usp_Insert_Update_Customer
--@Id int,
--@DisplayName nvarchar(MAX),
--@BirthDay datetime,
--@Sex nvarchar(10),
--@Address nvarchar(MAX),
--@Phone nvarchar(20),
--@Email nvarchar(200),
--@MoreInfo nvarchar(MAX),
--@ContractDate datetime,
--@Status nvarchar(128),
--@IsVisible int,
--@LinkImage nvarchar(MAX)
--as 
--begin
--	--declare @countPhone int = 0
--	--declare @countEmail int = 0
--	--select @countPhone = count(*) from Customer c where c.Phone = @Phone  and c.Id <> @Id
--	--select @countEmail= count(*) from Customer c where c.Email= @Email and c.Id <> @Id
--  if exists(select * from Customer with (updlock, serializable)  where id = @Id )
--   begin 
--    --begin tran
--update Customer set DisplayName=@DisplayName,BirthDay=@BirthDay,Sex=@Sex,Address=@Address,Phone=@Phone,Email=@Email,MoreInfo=@MoreInfo,ContractDate=@ContractDate,Status=@Status,IsVisible=@IsVisible,LinkImage=@LinkImage  where Id = @Id  
----     if (@countPhone <> 0)
----      begin
------waitfor delay '00:00:10'
----RAISERROR (N'Số điện thoại này đã tồn tại!',0,1)
----rollback tran
----      end
----     else if (@countEmail <> 0 )
----      begin
------waitfor delay '00:00:10'
----RAISERROR (N'Email này đã tồn tại!',0,1)
----rollback tran
----      end
----	 else commit tran
--   end 
--  else 
--   begin 
--    --begin tran 
--insert into Customer(DisplayName,BirthDay,Sex,Address,Phone,Email,MoreInfo,ContractDate,Status,IsVisible,LinkImage )  values(@DisplayName,@BirthDay,@Sex,@Address,@Phone,@Email,@MoreInfo,@ContractDate,@Status,@IsVisible,@LinkImage)
----	 if (@countPhone <> 0)
----      begin
------waitfor delay '00:00:10'
----RAISERROR (N'Số điện thoại này đã tồn tại!',0,1)
----rollback tran
----      end
----	 else if (@countEmail <> 0 )
----      begin
------waitfor delay '00:00:10'
----RAISERROR (N'Email này đã tồn tại!',0,1)
----rollback tran
----      end
----	else commit tran
--   end 
--end
--go       


--Insert Update Object
create proc usp_Insert_Update_Object
@Id nvarchar(128),
@DisplayName varchar(MAX),
@IdUnit int,
@IdCategory int,        
@InputPrice float,
@OutputPrice float,
@Count int,
@IdPosition int,
@LinkImage nvarchar(128),
@Status nvarchar(128),
@IsVisible int
as 
begin
 begin tran 
  if exists(select * from Object with (updlock, serializable)  where id = @Id )
   begin 
Update Object set DisplayName = @DisplayName , IdUnit = @IdUnit, IdCategory = @IdCategory ,InputPrice = @InputPrice, OutputPrice = @OutputPrice, Count = @Count, IdPosition = @IdPosition,LinkImage = @LinkImage,Status = @Status,IsVisible = @IsVisible  where Id = @Id
   end 
  else 
  begin
  declare @IdInsert nvarchar(128) = ''
if((select count(*) from Object)=0)
  begin
set @IdInsert = 'SP00001'
  end
else
  begin
  declare @loop int = 0
  declare @index int = 0
  declare @idTemp nvarchar(128)
SELECT TOP 1 @idTemp = Id FROM Object ORDER BY Id DESC
SET @idTemp = SUBSTRING(@idTemp, 3, 5) 
set @loop = 5 - len(CAST(@idTemp AS int))
set @index = CAST(@idTemp AS int)+1;
set @IdInsert = 'SP'
if CAST(SUBSTRING(@idTemp, 4, 5)  AS int) = 9
  begin
set @loop -= 1
  end
  while @loop > 0
  begin
set @IdInsert += '0'
set @loop -=1
  end
set @IdInsert += CAST(@index AS nvarchar)
  end
insert into Object(Id,DisplayName,IdUnit,IdCategory,InputPrice,OutputPrice, Count,IdPosition,LinkImage,Status,IsVisible) values(@IdInsert,@DisplayName,@IdUnit,@IdCategory,@InputPrice,@OutputPrice,@Count,@IdPosition,@LinkImage,@Status,@IsVisible)
select @IdInsert
   end 
 commit tran
end
go

create proc usp_Check_DisplayName_Exist_Object
@DisplayName nvarchar(MAX),
@Id nvarchar(128)
 as 
 begin
select count(*) from Object where DisplayName = @DisplayName and Id <> @Id
 end
go

--Delete Object
create proc usp_Delete_Object
@Id nvarchar(128)
as 
begin
 begin tran 
  if( exists(select * from OutputInfo with (updlock, serializable) inner join Object  on OutputInfo.IdObject = Object.Id where Object.Id = @Id ) 
  or exists(select * from InputInfo with (updlock, serializable) inner join Object  on InputInfo.IdObject = Object.Id where Object.Id = @Id ) )
   begin 
Update Object set IsVisible = 1 where Id = @Id
   end 
  else 
   begin 
delete from Object where Id = @Id
   end 
 commit tran
end
go

--Delete User
create proc usp_Delete_User
@Id nvarchar(128)
as 
begin
 begin tran 
  if( exists(select * from Output with (updlock, serializable) inner join Users  on Output.IdUser = Users.Id where Users.Id = @Id ) 
  or exists(select * from Input with (updlock, serializable) inner join Users  on Input.IdUser = Users.Id where Users.Id = @Id ) )
   begin 
Update Users set IsVisible = 1 where Id = @Id
   end 
  else 
   begin 
delete from Users where Id = @Id
   end 
 commit tran
end
go
--Delete Supplier
create proc usp_Delete_Supplier
@Id nvarchar(128)
as 
begin
 begin tran 
  if exists(select * from Input  inner join Supplier  on Input.IdSupplier = Supplier.Id where Supplier.Id = @Id) 
   begin 
Update Supplier set IsVisible = 1 where Id = @Id
Update Input set IdSupplier = null where IdSupplier = @Id and Status = N'Phiếu tạm'
   end 
  else 
   begin 
delete from Supplier where Id = @Id
   end 
 commit tran
end
go

--Delete Customer
create proc usp_Delete_Customer
@Id nvarchar(128)
as 
begin
 begin tran 
  if exists(select * from Output with (updlock, serializable) inner join Customer  on Output.IdCustomer = Customer.Id where Customer.Id = @Id) 
   begin 
Update Customer set IsVisible = 1 where Id = @Id
   end 
  else 
   begin 
delete from Customer where Id = @Id
   end 
 commit tran
end
go

---Delete Catgory
create proc usp_Delete_Category
@Id int
as 
begin
 begin tran 
  if exists(select * from Object with (updlock, serializable) inner join Category  on Object.IdCategory= Category.Id where Category.Id = @Id and Object.IsVisible = 0) 
   begin 
RAISERROR (N'Loại hàng này đã được sử dụng trong các hàng hóa!',0,1)
rollback tran
   end 
  else 
   begin 
Update Object set IdCategory = null where IdCategory = @Id and IsVisible = 1
delete from Category where Id = @Id
   end 
 commit tran
end
go

---Delete Position
create proc usp_Delete_Position
@Id int
as 
begin
 begin tran 
  if exists(select * from Object with (updlock, serializable) inner join Position on Object.IdCategory= Position.Id where Position.Id = @Id and IsVisible = 0) 
   begin 
RAISERROR (N'Vị trí này đã được sử dụng trong các hàng hóa!',0,1)
rollback tran
   end 
  else 
   begin 
Update Object set IdPosition= null where IdPosition = @Id and IsVisible = 1
delete from Position where Id = @Id
   end 
 commit tran
end
go


---Delete Catgory
create proc usp_Delete_Unit
@Id int
as 
begin
 begin tran 
  if exists(select * from Object with (updlock, serializable) inner join Unit  on Object.IdCategory= Unit.Id where Unit.Id = @Id and IsVisible = 0) 
   begin 
RAISERROR (N'Đơn vị tính này đã được sử dụng trong các hàng hóa!',0,1)
rollback tran
   end 
  else 
   begin 
Update Object set IdUnit= null where IdUnit= @Id and IsVisible = 1
delete from Unit where Id = @Id
   end 
 commit tran
end
go

--Update Status Object
create proc usp_Update_Status_Object
@Id nvarchar(128),
@Status nvarchar(128)
as 
begin
Update Object set Status  = @Status where Id = @Id
end
go

--Update RolePermision 
create proc usp_Update_RolePermision_UserRole
@Id nvarchar(128),
@RolePermision nvarchar(128)
as 
begin
Update UserRole set RolePermision = @RolePermision where Id = @Id
end
go

--Insert Update Input
create proc usp_Insert_Update_Input
@Id nvarchar(128),
@IdUser int,
@IdSupplier int,        
@DateInput datetime,
@Discount float,
@Payment nvarchar(128),
@TotalPrice float,
@Note nvarchar(MAX),
@Status nvarchar(128),
@TotalObject int,
@TotalQuantity int   
as 
begin
 begin tran 
  if exists(select * from Input with (updlock, serializable)  where id = @Id )
   begin 
update Input set IdUser = @IdUser, IdSupplier = @IdSupplier,DateInput = @DateInput,Discount = @Discount,Payment = @Payment,TotalPrice = @TotalPrice,Note = @Note,Status = @Status,TotalObject = @TotalObject,TotalQuantity = @TotalQuantity where Id = @Id
   end 
  else 
  begin
  declare @IdInsert nvarchar(128) = ''
if((select count(*) from Input)=0)
  begin
set @IdInsert = 'PN00001'
  end
else
  begin
  declare @loop int = 0
  declare @index int = 0
  declare @idTemp nvarchar(128)
SELECT TOP 1 @idTemp = Id FROM Input ORDER BY Id DESC
SET @idTemp = SUBSTRING(@idTemp, 3, 5) 
set @loop = 5 - len(CAST(@idTemp AS int))
set @index = CAST(@idTemp AS int)+1;
set @IdInsert = 'PN'
if CAST(SUBSTRING(@idTemp, 4, 5)  AS int) = 9
  begin
set @loop -= 1
  end
  while @loop > 0
  begin
set @IdInsert += '0'
set @loop -=1
  end
set @IdInsert += CAST(@index AS nvarchar)
  end
insert into Input(Id,IdUser,IdSupplier,DateInput,Discount,Payment,TotalPrice,Note,Status,TotalObject,TotalQuantity) values(@IdInsert,@IdUser,@IdSupplier,@DateInput,@Discount,@Payment,@TotalPrice,@Note,@Status,@TotalObject,@TotalQuantity)
select @IdInsert
   end 
 commit tran
end
go

--Insert Update InputInfo
create proc usp_Insert_Update_InputInfo
@IdInput nvarchar(128),
@IdObject nvarchar(128),
@Price float, 
@Quantity  int,
@Discount float,
@IdUnit int
as 
begin
 begin tran 
  if exists(select * from InputInfo with (updlock, serializable)  where IdInput = @IdInput and IdObject = @IdObject)
   begin 
update InputInfo set Price = @Price,Quantity= @Quantity,Discount = @Discount  where IdInput = @IdInput and IdObject = @IdObject
   end 
  else 
   begin
declare @Stock int = 0
select @Stock = Count from Object where Id = @IdObject
insert into InputInfo(IdInput,IdObject,Price,Quantity,Discount,Stock)  values(@IdInput,@IdObject,@Price,@Quantity,@Discount,@Stock)
   end 
   declare @Status nvarchar(128) = ''
   select @Status = Status  from Input where Id = @IdInput
   if(@Status = N'Đã nhập hàng')
   begin
   declare @Count int = 0
   select @Count = @Quantity + Count from Object where Id = @IdObject
   declare @InputPrice float = 0
   select @InputPrice =( @Quantity*@Price + Count*InputPrice)/(@Quantity+Count) from Object where Id = @IdObject
   update Object set Count = @Count,InputPrice = @InputPrice,IdUnit = @IdUnit where Id = @IdObject
   end
 commit tran
end
go 

--Insert Update Output
create proc usp_Insert_Output
@IdUser int,
@IdCustomer int,        
@DateOutput datetime,
@Discount float,
@Payment nvarchar(128),
@TotalPrice float,
@Note nvarchar(MAX),
@Status nvarchar(128)   
as 
begin 
  declare @IdInsert nvarchar(128) = ''
if((select count(*) from Output)=0)
  begin
set @IdInsert = 'HD00001'
  end
else
  begin
  declare @loop int = 0
  declare @index int = 0
  declare @idTemp nvarchar(128)
SELECT TOP 1 @idTemp = Id FROM Output ORDER BY Id DESC
SET @idTemp = SUBSTRING(@idTemp, 3, 5) 
set @loop = 5 - len(CAST(@idTemp AS int))
set @index = CAST(@idTemp AS int)+1;
set @IdInsert = 'HD'
if CAST(SUBSTRING(@idTemp, 4, 5)  AS int) = 9
  begin
set @loop -= 1
  end
  while @loop > 0
  begin
set @IdInsert += '0'
set @loop -=1
  end
set @IdInsert += CAST(@index AS nvarchar)
  end
insert into Output(Id,IdUser,IdCustomer,DateOutput,Discount,Payment,TotalPrice,Note,Status)  values(@IdInsert,@IdUser,@IdCustomer,@DateOutput,@Discount,@Payment,@TotalPrice,@Note,@Status)
select @IdInsert
end
go 

--Insert Update OutputInfo
create proc usp_Insert_OutputInfo
@IdOutput nvarchar(128),
@IdObject nvarchar(128),
@Price float, 
@Quantity  int,
@Discount float
as 
begin
 begin tran 
declare @Stock int = 0
select @Stock = Count from Object where Id = @IdObject
insert into OutputInfo(IdOutput,IdObject,Price,Quantity,Discount,Stock) values(@IdOutput,@IdObject,@Price,@Quantity,@Discount,@Stock)
declare @Count int = 0
select @Count = -@Quantity + Count from Object where Id = @IdObject
   if(@Count<0)
   begin
   --declare @string nvarchar(MAX) = ''
   --set @string = N'Số lượng '+  DisplayName from Object where Id = @IdObject +' không quá số lượng tồn!'
RAISERROR (N'Số lượng không quá số lượng tồn!',0,1)
rollback tran
	end
	else
update Object set Count = @Count where Id = @IdObject
 commit tran
end
go 

--Insert Update Category
create proc usp_Insert_Update_Category
@Id int,
@DisplayName nvarchar(MAX) 
as 
begin
--if exists(select * from Category with (updlock, serializable)  where id <> @Id and UPPER(DisplayName) = UPPER(@DisplayName) )
--  begin
--RAISERROR (N'Tên đã tồn tại!',0,1)
--  end
--else 
  begin
 if exists(select * from Category with (updlock, serializable)  where id = @Id )
  begin 
  --select * from Category
  --waitfor delay '00:00:5'
update Category set DisplayName=@DisplayName where Id = @Id  
   end 
 else 
   begin 
  --   select * from Category
  --waitfor delay '00:00:5'
insert into Category(DisplayName)  values(@DisplayName)
   end
   end 
end
go

--Insert Update Position
create proc usp_Insert_Update_Position
@Id int,
@DisplayName nvarchar(MAX) 
as 
begin
--if exists(select * from Position with (updlock, serializable)  where id <> @Id and UPPER(DisplayName) = UPPER(@DisplayName) )
--  begin
--RAISERROR (N'Tên đã tồn tại!',0,1)
--  end
--else 
  begin
 if exists(select * from Position with (updlock, serializable)  where id = @Id )
  begin 
update Position set DisplayName=@DisplayName where Id = @Id  
   end 
 else 
   begin 
insert into Position(DisplayName)  values(@DisplayName)
   end
   end 
end
go

--Insert Update Unit
create proc usp_Insert_Update_Unit
@Id int,
@DisplayName nvarchar(MAX) 
as 
begin
--if exists(select * from Unit with (updlock, serializable)  where id <> @Id and UPPER(DisplayName) = UPPER(@DisplayName) )
--  begin
--RAISERROR (N'Tên đã tồn tại!',0,1)
--  end
--else 
  begin
 if exists(select * from Unit with (updlock, serializable)  where id = @Id )
  begin 
update Unit set DisplayName=@DisplayName where Id = @Id  
   end 
 else 
   begin 
insert into Unit(DisplayName)  values(@DisplayName)
   end
   end 
end
go

--Update Status Output
create proc usp_Update_Status_Output
@Id nvarchar(128),
@Status nvarchar(128)
as 
begin
Update Output set Status = @Status where Id = @Id
end
go 

--Update Note Output
create proc usp_Update_Note_Output
@Id nvarchar(128),
@Note nvarchar(MAX)
as 
begin
Update Output set Note = @Note where Id = @Id
end
go 


--Update Status Intput
create proc usp_Update_Status_Input
@Id nvarchar(128),
@Status nvarchar(128)
as 
begin
Update Input set Status = @Status where Id = @Id
end
go 

--Update Note Input
create proc usp_Update_Note_Input
@Id nvarchar(128),
@Note nvarchar(MAX)
as 
begin
Update Input set Note = @Note where Id = @Id
end
go 

--/*trigger*/
--trigger Insert Update Category
create trigger trg_Insert_Update_Category
on Category
for insert,update
as
if update(DisplayName)
if exists(select * from inserted i,Category c
where UPPER(i.DisplayName) = UPPER(c.DisplayName) and i.Id <> c.Id)
begin
RAISERROR (N'Tên này đã tồn tại!',0,1)
rollback tran
end
go

--trigger Insert Update Position
create trigger trg_Insert_Update_Position
on Position
for insert,update
as
if update(DisplayName)
if exists(select * from inserted i,Position c
where UPPER(i.DisplayName) = UPPER(c.DisplayName) and i.Id <> c.Id)
begin
RAISERROR (N'Tên này đã tồn tại!',0,1)
rollback tran
end
go

--trigger Insert Update User
create trigger trg_Insert_Update_User
on Users
for insert,update
as
if update(Phone) or update(Email) or update(UserName)
if exists(select * from inserted i,Users c
where i.Phone = c.Phone  and i.Id <> c.Id and trim(c.Phone) <> '' and c.IsVisible <>1)
begin
--waitfor delay '00:00:10'
RAISERROR (N'Số điện thoại này đã tồn tại!',0,1)
rollback tran
end
else if exists(select * from inserted i,Users c
where i.Email = c.Email  and i.Id <> c.Id and trim(c.Email) <> '' and c.IsVisible <>1)
begin
--waitfor delay '00:00:10'
RAISERROR (N'Email này đã tồn tại!',0,1)
rollback tran
end
else if exists(select * from inserted i,Users c
where i.UserName = c.UserName  and i.Id <> c.Id and trim(c.UserName) <> '' and c.IsVisible <>1)
begin
--waitfor delay '00:00:10'
RAISERROR (N'Tên đăng nhập này đã tồn tại!',0,1)
rollback tran
end
go

--trigger Insert Update Unit
create trigger trg_Insert_Update_Unit
on Unit
for insert,update
as
if update(DisplayName)
if exists(select * from inserted i,Unit c
where UPPER(i.DisplayName) = UPPER(c.DisplayName) and i.Id <> c.Id)
begin
RAISERROR (N'Tên này đã tồn tại!',0,1)
rollback tran
end
go

--trigger Insert Update Supplier
create trigger trg_Insert_Update_Supplier
on Supplier
for insert,update
as
if update(Phone) or update(Email)
if exists(select * from inserted i,Supplier c
where i.Phone = c.Phone  and i.Id <> c.Id and trim(c.Phone) <> '' and c.IsVisible <>1)
begin
waitfor delay '00:00:2'
RAISERROR (N'Số điện thoại này đã tồn tại!',0,1)
rollback tran
end
else if exists(select * from inserted i,Supplier c
where i.Email = c.Email  and i.Id <> c.Id and trim(c.Email) <> '' and c.IsVisible <>1)
begin
waitfor delay '00:00:2'
RAISERROR (N'Email này đã tồn tại!',0,1)
rollback tran
end
go

create proc usp_Update_Customer
@Id int,
@DisplayName nvarchar(MAX),
@BirthDay datetime,
@Sex nvarchar(10),
@Address nvarchar(MAX),
@Phone nvarchar(20),
@Email nvarchar(200),
@MoreInfo nvarchar(MAX),
@ContractDate datetime,
@Status nvarchar(128),
@IsVisible int,
@LinkImage nvarchar(MAX)
as
begin
update Customer set DisplayName=@DisplayName,BirthDay=@BirthDay,Sex=@Sex,Address=@Address,Phone=@Phone,Email=@Email,MoreInfo=@MoreInfo,ContractDate=@ContractDate,Status=@Status,IsVisible=@IsVisible,LinkImage=@LinkImage where Id = @Id
end
go
create proc usp_Update_Status_Customer
@Id int,
@Status nvarchar(128)
as
begin
update Customer set Status=@Status where Id = @Id
end
go

create proc usp_Update_Status_Supplier
@Id int,
@Status nvarchar(128)
as
begin
update Supplier set Status=@Status where Id = @Id
end
go

create proc usp_Update_Status_User
@Id int,
@Status nvarchar(128)
as
begin
update Users set Status=@Status where Id = @Id
end

go
create proc usp_Insert_Customer
@DisplayName nvarchar(MAX),
@BirthDay datetime,
@Sex nvarchar(10),
@Address nvarchar(MAX),
@Phone nvarchar(20),
@Email nvarchar(200),
@MoreInfo nvarchar(MAX),
@ContractDate datetime,
@Status nvarchar(128),
@IsVisible int,
@LinkImage nvarchar(MAX)
as
begin
insert into Customer(DisplayName,BirthDay,Sex,Address,Phone,Email,MoreInfo,ContractDate,Status,IsVisible,LinkImage )
 values(@DisplayName,@BirthDay,@Sex,@Address,@Phone,@Email,@MoreInfo,@ContractDate,@Status,@IsVisible,@LinkImage)
end
go

--Hàm người dùng Function
--Check Login
create function uf_Check_Login
(@UserName nvarchar(100),@Password nvarchar(MAX)) returns Table
As
Return
(Select *
From Users
Where Upper(UserName) = Upper(@UserName)
and Upper(Users.Password) = Upper(@Password) and IsVisible = 0 and Status = N'Đang hoạt động')
go

--Select * from uf_Check_Login('admin','db69fc039dcbd2962cb4d28f5891aae1')


--Lấy ds Thẻ kho hàng hóa
create function uf_Select_Deal (@Id nvarchar(128))
returns @Deal table( id nvarchar(128),method nvarchar(128),date Datetime, price float,count int,stock int) 
as begin
insert into @Deal select Id,N'Nhập hàng',DateInput,Price,Quantity,Stock  from InputInfo  join Input on IdInput = Id
where IdObject = @Id
insert into @Deal select Id,N'Xuất hàng',DateOutput,Price,Quantity,Stock  from OutputInfo  join Output on IdOutput = Id
where IdObject = @Id
return
end
go
--select * from uf_Select_Deal('SP00001') order by date


--hàm người dùng
--Tính số phiếu nhập,tổng tiền từ ngày A-B
create function uf_Select_Revenue_Input (@dateIn date,@dateOut date) returns @Revenue table
( CoutnIn int, TotalIn float)
as begin
insert into @Revenue select count(*), sum(A.TotalPrice-A.Discount) from Input A 
where CONVERT(date, A.DateInput) between @dateIn and @dateOut
and A.Status <> N'Phiếu tạm' and A.Status <> N'Đã hủy'
return
end
go

--select * from uf_Select_Revenue_Input('2019-05-29 00:00:00','2019-05-29 23:59:59')


--Tính số hóa đơn,tổng tiền từ ngày A-B
create function uf_Select_Revenue_Output (@dateIn datetime,@dateOut datetime) returns @Revenue table
( CoutnIn int, TotalIn float)
as begin

insert into @Revenue select count(*), sum(A.TotalPrice) from Output A where A.DateOutput between @dateIn and @dateOut
and A.Status <>N'Đã hủy'
return
end
go
--select * from uf_Select_Revenue_Output('2019-05-28 00:00:00','2019-05-28 23:59:59')

--Tính Tổng Doanh thu theo ngày
create function [dbo].[uf_Select_Revenue] (@dateIn date,@dateOut date) returns @Revenue table
(day Date,Sales float)
as begin
insert into @Revenue 
select CONVERT(date, DateOutput),sum(TotalPrice)  
from  Output 
where CONVERT(date, DateOutput) between @dateIn and @dateOut
and Status  <> N'Đã hủy'
group by CONVERT(date, DateOutput)
return
end
go

--Insert Update UserRole
create proc usp_Insert_UserRole
@Id int,
@DisplayName nvarchar(MAX),
@RolePermision nvarchar(MAX)
as 
begin
  if exists(select * from UserRole with (updlock, serializable)  where id = @Id )
   begin 
update UserRole set DisplayName=@DisplayName,RolePermision=@RolePermision  where Id = @Id  
   end 
  else 
   begin 
insert into UserRole(DisplayName,RolePermision)
  values(@DisplayName,@RolePermision)
   end 
end
go 


--Insert Update User
create proc usp_Insert_Update_User
@Id int,
@DisplayName nvarchar(MAX),
@BirthDay datetime,
@Sex nvarchar(10),
@Address nvarchar(MAX),
@Phone nvarchar(20),
@Email nvarchar(200),
@MoreInfo nvarchar(MAX),
@UserName nvarchar(100),
@Password nvarchar(MAX),
@IdRole int,
@Status nvarchar(128),
@IsVisible int
as 
begin
  if exists(select * from Users with (updlock, serializable)  where id = @Id )
   begin 
update Users set DisplayName=@DisplayName,BirthDay=@BirthDay,Sex=@Sex,Address=@Address
,Phone=@Phone,Email=@Email,MoreInfo=@MoreInfo,UserName = @UserName,Password= @Password, IdRole=@IdRole,Status=@Status
,IsVisible=@IsVisible  where Id = @Id  
   end 
  else 
   begin 
insert into Users(DisplayName,BirthDay,Sex,Address,Phone,Email,MoreInfo,UserName,Password,IdRole,Status,IsVisible)
  values(@DisplayName,@BirthDay,@Sex,@Address,@Phone,@Email,@MoreInfo,@UserName,@Password,@IdRole,@Status,@IsVisible)
   end 
end
go 

--Insert Update Customer
create proc usp_Insert_Update_Customer
@Id int,
@DisplayName nvarchar(MAX),
@BirthDay datetime,
@Sex nvarchar(10),
@Address nvarchar(MAX),
@Phone nvarchar(20),
@Email nvarchar(200),
@MoreInfo nvarchar(MAX),
@ContractDate datetime,
@Status nvarchar(128),
@IsVisible int,
@LinkImage nvarchar(MAX)
as 
begin
  if exists(select * from Customer with (updlock, serializable)  where id = @Id )
   begin 
update Customer set DisplayName=@DisplayName,BirthDay=@BirthDay,Sex=@Sex,Address=@Address
,Phone=@Phone,Email=@Email,MoreInfo=@MoreInfo,ContractDate=@ContractDate,Status=@Status
,IsVisible=@IsVisible,LinkImage=@LinkImage  where Id = @Id  
   end 
  else 
   begin 
insert into Customer(DisplayName,BirthDay,Sex,Address,Phone,Email,MoreInfo,ContractDate,Status,IsVisible,LinkImage )
  values(@DisplayName,@BirthDay,@Sex,@Address,@Phone,@Email,@MoreInfo,@ContractDate,@Status,@IsVisible,@LinkImage)
   end 
end
go       