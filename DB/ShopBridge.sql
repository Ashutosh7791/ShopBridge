USE [ShopBridge]
GO
/****** Object:  Table [dbo].[ProductMaster]    Script Date: 05-07-2022 23:29:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductMaster](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NULL,
	[FileName] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[GetProduct]    Script Date: 05-07-2022 23:29:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetProduct]
	@Id int = 0
AS
BEGIN
	
	SET NOCOUNT ON; 

	Select Id,ProductName,Description,Price,FileName from ProductMaster where Id = @Id;

END
GO
/****** Object:  StoredProcedure [dbo].[GetProductList]    Script Date: 05-07-2022 23:29:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
CREATE PROCEDURE [dbo].[GetProductList] 
	@SortCol NVARCHAR(100) = 'Id',
	@SortDir NVARCHAR(100) = 'desc',
	@PageSize int = 20,
	@PageIndex int = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;		

		;WITH Table_A AS 
		(
			SELECT 
			( ROW_NUMBER() OVER ( ORDER BY Id DESC )) Row,
					CEILING(CAST(COUNT(*) OVER (PARTITION BY 1) AS DECIMAL(10, 2)) / CAST(CONVERT(NVARCHAR,@PageSize) AS DECIMAL(10, 2))) AS NoOfPages, 
					m.Id,
					m.ProductName,  
					m.Description,
					m.Price,
					m.FileName
			FROM [dbo].[ProductMaster] m where m.IsDelete = 0
		)

		SELECT (SELECT COUNT(1) AS CountOrders FROM Table_A) AS TotalRows,* 
		FROM Table_A
		WHERE ROW BETWEEN ((@PageIndex - 1) * @PageSize + 1) AND CASE WHEN @PageIndex > 0 THEN (@PageIndex * @PageSize) ELSE (SELECT COUNT(1) AS CountOrders FROM Table_A)  END 
	
END
GO
/****** Object:  StoredProcedure [dbo].[ProductOperation]    Script Date: 05-07-2022 23:29:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProductOperation]
	@Id int = 0,
	@ProductName nvarchar(max) = null,
	@Description nvarchar(max) = null,
	@Price decimal(18,2) = 0.0,
	@FileName nvarchar(max) = null,
	@OperationType int = 0
AS
BEGIN
	
	SET NOCOUNT ON; 

	if(@OperationType = 1)
	Begin
	if not exists (Select 1 from ProductMaster where ProductName = @ProductName and IsDelete = 0)
	Begin
	insert into ProductMaster(ProductName, Description, Price, FileName, IsDelete)
	values (@ProductName, @Description, @Price, @FileName, 0);
	select max(Id) as Id from ProductMaster
	End
	Else
	Begin
	Select 0 as Id
	End
	End

    else if(@OperationType = 2)
	Begin
	if not exists (Select 1 from ProductMaster where ProductName = @ProductName and Id!=@Id and IsDelete = 0)
	Begin
	update ProductMaster set
	ProductName = @ProductName,
	Description = @Description,
	Price = @Price,
	FileName = @FileName
	where Id = @Id
	Select @Id as Id
	End
	else
	Begin
	Select 0 as Id
	End
	End

	else if(@OperationType = 3)
	Begin
	update ProductMaster Set IsDelete = 1 where Id= @Id;
	Select @Id as Id
	End
END
GO
