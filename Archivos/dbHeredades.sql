USE [master]
GO
/****** Object:  Database [dbHeredades]    Script Date: 05/08/2018 11:53:56 ******/
CREATE DATABASE [dbHeredades]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'dbHeredades', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\dbHeredades.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'dbHeredades_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\dbHeredades_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [dbHeredades] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [dbHeredades].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [dbHeredades] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [dbHeredades] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [dbHeredades] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [dbHeredades] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [dbHeredades] SET ARITHABORT OFF 
GO
ALTER DATABASE [dbHeredades] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [dbHeredades] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [dbHeredades] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [dbHeredades] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [dbHeredades] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [dbHeredades] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [dbHeredades] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [dbHeredades] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [dbHeredades] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [dbHeredades] SET  DISABLE_BROKER 
GO
ALTER DATABASE [dbHeredades] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [dbHeredades] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [dbHeredades] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [dbHeredades] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [dbHeredades] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [dbHeredades] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [dbHeredades] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [dbHeredades] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [dbHeredades] SET  MULTI_USER 
GO
ALTER DATABASE [dbHeredades] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [dbHeredades] SET DB_CHAINING OFF 
GO
ALTER DATABASE [dbHeredades] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [dbHeredades] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [dbHeredades] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [dbHeredades] SET QUERY_STORE = OFF
GO
USE [dbHeredades]
GO
ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [dbHeredades]
GO
/****** Object:  User [uPedro]    Script Date: 05/08/2018 11:53:56 ******/
CREATE USER [uPedro] FOR LOGIN [uPedro] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[tbPermiso]    Script Date: 05/08/2018 11:53:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbPermiso](
	[codPermiso] [int] NOT NULL,
	[permiso] [varchar](75) NULL,
 CONSTRAINT [PK_tbPermiso] PRIMARY KEY CLUSTERED 
(
	[codPermiso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbPermisoDenegado]    Script Date: 05/08/2018 11:53:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbPermisoDenegado](
	[codRol] [int] NOT NULL,
	[codPermiso] [int] NOT NULL,
 CONSTRAINT [PK_tbPermisoDenegado] PRIMARY KEY CLUSTERED 
(
	[codRol] ASC,
	[codPermiso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbRol]    Script Date: 05/08/2018 11:53:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbRol](
	[codRol] [int] NOT NULL,
	[rol] [varchar](25) NULL,
 CONSTRAINT [PK_tbRol] PRIMARY KEY CLUSTERED 
(
	[codRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbUsuario]    Script Date: 05/08/2018 11:53:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbUsuario](
	[codUsuario] [int] IDENTITY(1,1) NOT NULL,
	[codRol] [int] NOT NULL,
	[usuario] [varchar](20) NOT NULL,
	[nombre] [varchar](50) NOT NULL,
	[password] [varchar](30) NOT NULL,
 CONSTRAINT [PK_tbUsuario] PRIMARY KEY CLUSTERED 
(
	[codUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbPermisoDenegado]  WITH CHECK ADD  CONSTRAINT [FK_tbPermisoDenegado_tbPermiso] FOREIGN KEY([codPermiso])
REFERENCES [dbo].[tbPermiso] ([codPermiso])
GO
ALTER TABLE [dbo].[tbPermisoDenegado] CHECK CONSTRAINT [FK_tbPermisoDenegado_tbPermiso]
GO
ALTER TABLE [dbo].[tbPermisoDenegado]  WITH CHECK ADD  CONSTRAINT [FK_tbPermisoDenegado_tbRol] FOREIGN KEY([codRol])
REFERENCES [dbo].[tbRol] ([codRol])
GO
ALTER TABLE [dbo].[tbPermisoDenegado] CHECK CONSTRAINT [FK_tbPermisoDenegado_tbRol]
GO
ALTER TABLE [dbo].[tbUsuario]  WITH CHECK ADD  CONSTRAINT [FK_tbUsuario_tbRol] FOREIGN KEY([codRol])
REFERENCES [dbo].[tbRol] ([codRol])
GO
ALTER TABLE [dbo].[tbUsuario] CHECK CONSTRAINT [FK_tbUsuario_tbRol]
GO
USE [master]
GO
ALTER DATABASE [dbHeredades] SET  READ_WRITE 
GO
