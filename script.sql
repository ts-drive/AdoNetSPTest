USE [master]
GO
/****** Object:  Database [usersdb]    Script Date: 19.05.2017 9:46:50 ******/
CREATE DATABASE [usersdb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'usersdb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLSERV2014\MSSQL\DATA\usersdb.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'usersdb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLSERV2014\MSSQL\DATA\usersdb_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [usersdb] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [usersdb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [usersdb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [usersdb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [usersdb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [usersdb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [usersdb] SET ARITHABORT OFF 
GO
ALTER DATABASE [usersdb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [usersdb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [usersdb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [usersdb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [usersdb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [usersdb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [usersdb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [usersdb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [usersdb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [usersdb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [usersdb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [usersdb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [usersdb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [usersdb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [usersdb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [usersdb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [usersdb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [usersdb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [usersdb] SET  MULTI_USER 
GO
ALTER DATABASE [usersdb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [usersdb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [usersdb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [usersdb] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [usersdb] SET DELAYED_DURABILITY = DISABLED 
GO
USE [usersdb]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 19.05.2017 9:46:50 ******/
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
/****** Object:  StoredProcedure [dbo].[sp_CreateUser]    Script Date: 19.05.2017 9:46:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CreateUser]
    @name nvarchar(50),
    @age int,
	@Id int out
AS
    INSERT INTO Users (Name, Age)
    VALUES (@name, @age)
 
    SET @Id=SCOPE_IDENTITY()

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAgeRange]    Script Date: 19.05.2017 9:46:50 ******/
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
/****** Object:  StoredProcedure [dbo].[sp_GetUsers]    Script Date: 19.05.2017 9:46:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetUsers]
AS
    SELECT * FROM Users 

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUser]    Script Date: 19.05.2017 9:46:50 ******/
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
USE [master]
GO
ALTER DATABASE [usersdb] SET  READ_WRITE 
GO
