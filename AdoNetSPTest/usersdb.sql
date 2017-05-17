USE [usersdb]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 17.05.2017 14:31:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Age] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAgeRange]    Script Date: 17.05.2017 14:31:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAgeRange]
    @name nvarchar(50),
    @minAge int out,
    @maxAge int out
AS
    SELECT @minAge = MIN(Age), @maxAge = MAX(Age) FROM Users WHERE Name LIKE '%' + @name + '%'

GO
/****** Object:  StoredProcedure [dbo].[sp_GetUsers]    Script Date: 17.05.2017 14:31:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetUsers]
AS
    SELECT * FROM Users 

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUser]    Script Date: 17.05.2017 14:31:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUser] 
	@name nvarchar(50),
	@age int
AS
	insert into users (name, age)
	values (@name, @age)

	SELECT SCOPE_IDENTITY()

GO
