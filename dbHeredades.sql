USE [master]
GO
/****** Object:  Database [dbHeredades]    Script Date: 10/12/2018 08:04:26 ******/
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
/****** Object:  UserDefinedFunction [dbo].[GetDeuda]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetDeuda](@codProveedor INT)
RETURNS DECIMAL(12,2)
BEGIN
	RETURN (SELECT SUM(tbProductoTransaccion.precioCompra * tbProductoTransaccion.cantidad)
			FROM tbTransaccion
				INNER JOIN tbProductoTransaccion ON tbTransaccion.codTransaccion = tbProductoTransaccion.codTransaccion
			WHERE tbTransaccion.codProveedor = @codProveedor);
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetEntradaProducto]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetEntradaProducto](@codProducto INT, @codPresentacion INT)
RETURNS DECIMAL(12,2)
BEGIN
	RETURN (SELECT ISNULL(SUM(cantidad), 0)
			FROM tbProductoTransaccion
				INNER JOIN tbTransaccion ON tbProductoTransaccion.codTransaccion = tbTransaccion.codTransaccion
			WHERE codProducto = @codProducto AND codPresentacion = @codPresentacion AND tbTransaccion.codTipoTransaccion = 1);
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetPagos]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- funcion que retorna total de los pagos a proveedores

CREATE FUNCTION [dbo].[GetPagos](@codProveedor INT)
RETURNS DECIMAL(12,2)
BEGIN
	RETURN (SELECT SUM(tbPagoProveedor.pago)
			FROM tbPagoProveedor
			WHERE tbPagoProveedor.codProveedor = @codProveedor AND tbPagoProveedor.estado = 1);
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetSalidaProducto]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetSalidaProducto](@codProducto INT, @codPresentacion INT)
RETURNS DECIMAL(12,2)
BEGIN
	RETURN (SELECT ISNULL(SUM(cantidad), 0)
			FROM tbProductoTransaccion
				INNER JOIN tbTransaccion ON tbProductoTransaccion.codTransaccion = tbTransaccion.codTransaccion
			WHERE codProducto = @codProducto AND codPresentacion = @codPresentacion AND tbTransaccion.codTipoTransaccion = 2);
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetVentasProducto]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetVentasProducto](@codProducto INT, @codPresentacion INT)
RETURNS DECIMAL(12,2)
BEGIN
	RETURN (SELECT ISNULL(SUM(cantidad), 0)
			FROM tbVentaProducto
			WHERE codProducto = @codProducto AND codPresentacion = @codPresentacion);
END
GO
/****** Object:  Table [dbo].[tbProductoPresentacion]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbProductoPresentacion](
	[codProducto] [int] NOT NULL,
	[codPresentacion] [int] NOT NULL,
	[precioVenta] [decimal](8, 2) NULL,
	[unidades] [smallint] NULL,
	[correlativo] [smallint] NULL,
	[existencia] [decimal](8, 2) NULL,
 CONSTRAINT [PK_tbProductoPresentacion] PRIMARY KEY CLUSTERED 
(
	[codProducto] ASC,
	[codPresentacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbPresentacion]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbPresentacion](
	[codPresentacion] [int] IDENTITY(1,1) NOT NULL,
	[presentacion] [varchar](50) NULL,
	[estado] [bit] NULL,
 CONSTRAINT [PK_tbPresentacion] PRIMARY KEY CLUSTERED 
(
	[codPresentacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbProducto]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbProducto](
	[codProducto] [int] IDENTITY(1,1) NOT NULL,
	[codCategoria] [int] NULL,
	[producto] [varchar](50) NULL,
	[estado] [bit] NULL,
 CONSTRAINT [PK_tbProducto] PRIMARY KEY CLUSTERED 
(
	[codProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vExistencias]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vExistencias]
AS
	SELECT tbProducto.codProducto, tbProducto.producto,
		tbPresentacion.codPresentacion, tbPresentacion.presentacion,
		(
			dbo.GetEntradaProducto(tbProducto.codProducto, tbPresentacion.codPresentacion) -
			(dbo.GetSalidaProducto(tbProducto.codProducto, tbPresentacion.codPresentacion) +
			dbo.GetVentasProducto(tbProducto.codProducto, tbPresentacion.codPresentacion))
		) AS existencia
	FROM tbProductoPresentacion 
		INNER JOIN tbProducto ON tbProductoPresentacion.codProducto = tbProducto.codProducto
		INNER JOIN tbPresentacion ON tbProductoPresentacion.codPresentacion = tbPresentacion.codPresentacion
GO
/****** Object:  Table [dbo].[tbProveedor]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbProveedor](
	[codProveedor] [int] IDENTITY(1,1) NOT NULL,
	[proveedor] [varchar](50) NULL,
	[telefono] [varchar](10) NULL,
	[direccion] [varchar](50) NULL,
	[estado] [bit] NULL,
	[deuda] [decimal](8, 2) NULL,
 CONSTRAINT [PK_tbProveedor] PRIMARY KEY CLUSTERED 
(
	[codProveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vDeudaProveedor]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vDeudaProveedor]
AS
	SELECT codProveedor, proveedor, telefono, (dbo.GetDeuda(codProveedor) - dbo.GetPagos(codProveedor)) AS deuda
	FROM tbProveedor
GO
/****** Object:  Table [dbo].[tbCaja]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbCaja](
	[codCaja] [smallint] NOT NULL,
	[cantidad] [decimal](8, 2) NULL,
 CONSTRAINT [PK_tbCaja] PRIMARY KEY CLUSTERED 
(
	[codCaja] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbCategoria]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbCategoria](
	[codCategoria] [int] IDENTITY(1,1) NOT NULL,
	[categoria] [varchar](50) NULL,
	[estado] [bit] NULL,
 CONSTRAINT [PK_tbCategoria] PRIMARY KEY CLUSTERED 
(
	[codCategoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbDeudor]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbDeudor](
	[codDeudor] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](50) NULL,
	[telefono] [varchar](15) NULL,
	[residencia] [varchar](50) NULL,
	[refUno] [varchar](50) NULL,
	[refDos] [varchar](50) NULL,
	[deuda] [decimal](8, 2) NULL,
 CONSTRAINT [PK_tbDeudor] PRIMARY KEY CLUSTERED 
(
	[codDeudor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbPagoDeudor]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbPagoDeudor](
	[codPagoDeudor] [int] IDENTITY(1,1) NOT NULL,
	[codDeudor] [int] NULL,
	[codUsuario] [int] NULL,
	[pago] [decimal](8, 2) NULL,
	[fecha] [datetime] NULL,
 CONSTRAINT [PK_tbPagoDeudor] PRIMARY KEY CLUSTERED 
(
	[codPagoDeudor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbPagoProveedor]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbPagoProveedor](
	[codPagoProveedor] [int] IDENTITY(1,1) NOT NULL,
	[codProveedor] [int] NULL,
	[codUsuario] [int] NULL,
	[fecha] [datetime] NULL,
	[pago] [decimal](8, 2) NULL,
	[descripcion] [varchar](150) NULL,
	[estado] [bit] NULL,
 CONSTRAINT [PK_tbPagoProveedor] PRIMARY KEY CLUSTERED 
(
	[codPagoProveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbPermiso]    Script Date: 10/12/2018 08:04:27 ******/
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
/****** Object:  Table [dbo].[tbProductoProveedor]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbProductoProveedor](
	[codProveedor] [int] NOT NULL,
	[codProducto] [int] NOT NULL,
	[codPresentacion] [int] NOT NULL,
	[precioCompra] [decimal](6, 2) NULL,
	[estado] [bit] NULL,
 CONSTRAINT [PK_tbProductoProveedor] PRIMARY KEY CLUSTERED 
(
	[codProveedor] ASC,
	[codProducto] ASC,
	[codPresentacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbProductoTransaccion]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbProductoTransaccion](
	[codProductoTransaccion] [int] IDENTITY(1,1) NOT NULL,
	[codTransaccion] [int] NOT NULL,
	[codProducto] [int] NOT NULL,
	[codPresentacion] [int] NOT NULL,
	[cantidad] [int] NULL,
	[precioCompra] [decimal](6, 2) NULL,
 CONSTRAINT [PK_tbProductoTransaccion_1] PRIMARY KEY CLUSTERED 
(
	[codProductoTransaccion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbRol]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbRol](
	[codRol] [int] IDENTITY(1,1) NOT NULL,
	[rol] [varchar](25) NULL,
 CONSTRAINT [PK_tbRol] PRIMARY KEY CLUSTERED 
(
	[codRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbRolPermiso]    Script Date: 10/12/2018 08:04:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbRolPermiso](
	[codRol] [int] NOT NULL,
	[codPermiso] [int] NOT NULL,
	[estado] [bit] NULL,
 CONSTRAINT [PK_tbPermisoDenegado] PRIMARY KEY CLUSTERED 
(
	[codRol] ASC,
	[codPermiso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbTransaccion]    Script Date: 10/12/2018 08:04:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbTransaccion](
	[codTransaccion] [int] IDENTITY(1,1) NOT NULL,
	[codProveedor] [int] NULL,
	[codTipoTransaccion] [smallint] NULL,
	[descripcion] [varchar](500) NULL,
	[fecha] [datetime] NOT NULL,
	[estado] [bit] NULL,
	[codUsuario] [int] NULL,
 CONSTRAINT [PK_tbTransaccion] PRIMARY KEY CLUSTERED 
(
	[codTransaccion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbTransaccionCaja]    Script Date: 10/12/2018 08:04:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbTransaccionCaja](
	[codTransaccionCaja] [int] IDENTITY(1,1) NOT NULL,
	[codUsuario] [int] NULL,
	[tipoTransaccion] [smallint] NULL,
	[cantidad] [decimal](8, 2) NULL,
	[descripcion] [varchar](150) NULL,
	[fecha] [datetime] NULL,
 CONSTRAINT [PK_tbTransaccionCaja] PRIMARY KEY CLUSTERED 
(
	[codTransaccionCaja] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbUsuario]    Script Date: 10/12/2018 08:04:28 ******/
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
	[estado] [bit] NULL,
 CONSTRAINT [PK_tbUsuario] PRIMARY KEY CLUSTERED 
(
	[codUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbVenta]    Script Date: 10/12/2018 08:04:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbVenta](
	[codVenta] [int] IDENTITY(1,1) NOT NULL,
	[codDeudor] [int] NULL,
	[codUsuario] [int] NULL,
	[fecha] [datetime] NULL,
	[numFactura] [varchar](15) NULL,
	[estado] [bit] NULL,
 CONSTRAINT [PK_tbVenta] PRIMARY KEY CLUSTERED 
(
	[codVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbVentaProducto]    Script Date: 10/12/2018 08:04:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbVentaProducto](
	[codVenta] [int] NOT NULL,
	[codProducto] [int] NOT NULL,
	[codPresentacion] [int] NOT NULL,
	[cantidad] [decimal](6, 2) NULL,
	[precioVenta] [decimal](8, 2) NULL,
 CONSTRAINT [PK_tbVentaProducto] PRIMARY KEY CLUSTERED 
(
	[codVenta] ASC,
	[codProducto] ASC,
	[codPresentacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[tbCaja] ([codCaja], [cantidad]) VALUES (1, CAST(372.50 AS Decimal(8, 2)))
SET IDENTITY_INSERT [dbo].[tbCategoria] ON 

INSERT [dbo].[tbCategoria] ([codCategoria], [categoria], [estado]) VALUES (5, N'Pollo', 1)
INSERT [dbo].[tbCategoria] ([codCategoria], [categoria], [estado]) VALUES (6, N'Carne de res', 1)
INSERT [dbo].[tbCategoria] ([codCategoria], [categoria], [estado]) VALUES (7, N'Especialidades', 1)
SET IDENTITY_INSERT [dbo].[tbCategoria] OFF
SET IDENTITY_INSERT [dbo].[tbDeudor] ON 

INSERT [dbo].[tbDeudor] ([codDeudor], [nombre], [telefono], [residencia], [refUno], [refDos], [deuda]) VALUES (6, N'Nancy', N'4864 8732', NULL, NULL, NULL, CAST(0.00 AS Decimal(8, 2)))
INSERT [dbo].[tbDeudor] ([codDeudor], [nombre], [telefono], [residencia], [refUno], [refDos], [deuda]) VALUES (7, N'Olma tumax', N'30545565', NULL, NULL, NULL, CAST(0.00 AS Decimal(8, 2)))
INSERT [dbo].[tbDeudor] ([codDeudor], [nombre], [telefono], [residencia], [refUno], [refDos], [deuda]) VALUES (8, N'Olga Tumax', N'54657554', NULL, NULL, NULL, CAST(0.00 AS Decimal(8, 2)))
SET IDENTITY_INSERT [dbo].[tbDeudor] OFF
SET IDENTITY_INSERT [dbo].[tbPagoDeudor] ON 

INSERT [dbo].[tbPagoDeudor] ([codPagoDeudor], [codDeudor], [codUsuario], [pago], [fecha]) VALUES (3, 6, 1, CAST(30.00 AS Decimal(8, 2)), CAST(N'2018-11-13T10:12:41.777' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbPagoDeudor] OFF
SET IDENTITY_INSERT [dbo].[tbPagoProveedor] ON 

INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (3, 8, 1, CAST(N'2018-09-16T09:54:20.237' AS DateTime), CAST(24.50 AS Decimal(8, 2)), N'primer pago sistema', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (4, 8, 1, CAST(N'2018-09-16T10:30:42.243' AS DateTime), CAST(0.50 AS Decimal(8, 2)), N'', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (5, 8, 1, CAST(N'2018-09-16T10:30:48.993' AS DateTime), CAST(50.25 AS Decimal(8, 2)), N'', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (6, 8, 1, CAST(N'2018-09-16T10:31:03.553' AS DateTime), CAST(900.75 AS Decimal(8, 2)), N'', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (7, 8, 1, CAST(N'2018-09-20T14:38:54.670' AS DateTime), CAST(400.00 AS Decimal(8, 2)), N'', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (8, 8, 1, CAST(N'2018-09-20T14:48:02.373' AS DateTime), CAST(250.00 AS Decimal(8, 2)), N'pague por ...', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (9, 9, 1, CAST(N'2018-09-20T17:05:55.103' AS DateTime), CAST(100.00 AS Decimal(8, 2)), N'pague por anticipado', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (10, 9, 1, CAST(N'2018-09-29T14:00:50.763' AS DateTime), CAST(72.50 AS Decimal(8, 2)), N'', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (11, 9, 1, CAST(N'2018-09-29T15:14:24.647' AS DateTime), CAST(500.00 AS Decimal(8, 2)), N'-', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (12, 8, 1, CAST(N'2018-10-17T14:57:02.293' AS DateTime), CAST(24.00 AS Decimal(8, 2)), N'', 1)
INSERT [dbo].[tbPagoProveedor] ([codPagoProveedor], [codProveedor], [codUsuario], [fecha], [pago], [descripcion], [estado]) VALUES (13, 10, 1, CAST(N'2018-11-13T10:49:06.933' AS DateTime), CAST(2000.00 AS Decimal(8, 2)), N'Cajas de pollo ', 1)
SET IDENTITY_INSERT [dbo].[tbPagoProveedor] OFF
INSERT [dbo].[tbPermiso] ([codPermiso], [permiso]) VALUES (1, N'Catalogos')
INSERT [dbo].[tbPermiso] ([codPermiso], [permiso]) VALUES (2, N'Usuarios')
INSERT [dbo].[tbPermiso] ([codPermiso], [permiso]) VALUES (3, N'Inventario')
INSERT [dbo].[tbPermiso] ([codPermiso], [permiso]) VALUES (4, N'Roles')
INSERT [dbo].[tbPermiso] ([codPermiso], [permiso]) VALUES (5, N'Proveedores')
INSERT [dbo].[tbPermiso] ([codPermiso], [permiso]) VALUES (6, N'Compras')
INSERT [dbo].[tbPermiso] ([codPermiso], [permiso]) VALUES (7, N'Caidas')
SET IDENTITY_INSERT [dbo].[tbPresentacion] ON 

INSERT [dbo].[tbPresentacion] ([codPresentacion], [presentacion], [estado]) VALUES (7, N'Caja', 1)
INSERT [dbo].[tbPresentacion] ([codPresentacion], [presentacion], [estado]) VALUES (8, N'Bolsa', 1)
INSERT [dbo].[tbPresentacion] ([codPresentacion], [presentacion], [estado]) VALUES (9, N'Libra', 1)
INSERT [dbo].[tbPresentacion] ([codPresentacion], [presentacion], [estado]) VALUES (10, N'Unidad', 1)
SET IDENTITY_INSERT [dbo].[tbPresentacion] OFF
SET IDENTITY_INSERT [dbo].[tbProducto] ON 

INSERT [dbo].[tbProducto] ([codProducto], [codCategoria], [producto], [estado]) VALUES (7, 6, N'Rochoy', 1)
INSERT [dbo].[tbProducto] ([codProducto], [codCategoria], [producto], [estado]) VALUES (8, 5, N'Pollo amarillo', 1)
INSERT [dbo].[tbProducto] ([codProducto], [codCategoria], [producto], [estado]) VALUES (9, 5, N'Pollo blanco', 1)
INSERT [dbo].[tbProducto] ([codProducto], [codCategoria], [producto], [estado]) VALUES (10, 5, N'Pollo amarillo importado', 1)
INSERT [dbo].[tbProducto] ([codProducto], [codCategoria], [producto], [estado]) VALUES (11, 7, N'Tortitas', 1)
SET IDENTITY_INSERT [dbo].[tbProducto] OFF
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (7, 9, CAST(25.00 AS Decimal(8, 2)), 1, 1, CAST(1.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (8, 7, CAST(190.00 AS Decimal(8, 2)), 4, 3, CAST(6.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (8, 8, CAST(35.00 AS Decimal(8, 2)), 10, 2, CAST(2.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (8, 9, CAST(10.00 AS Decimal(8, 2)), 1, 1, CAST(0.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (9, 7, CAST(190.00 AS Decimal(8, 2)), 4, 3, CAST(18.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (9, 8, CAST(50.00 AS Decimal(8, 2)), 10, 2, CAST(2.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (9, 9, CAST(6.00 AS Decimal(8, 2)), 1, 1, CAST(-35.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (10, 7, CAST(180.00 AS Decimal(8, 2)), 40, 2, CAST(1.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (10, 9, CAST(8.50 AS Decimal(8, 2)), 1, 1, CAST(39.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (11, 8, CAST(15.00 AS Decimal(8, 2)), 18, 2, CAST(0.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion], [precioVenta], [unidades], [correlativo], [existencia]) VALUES (11, 10, CAST(1.00 AS Decimal(8, 2)), 1, 1, CAST(0.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProductoProveedor] ([codProveedor], [codProducto], [codPresentacion], [precioCompra], [estado]) VALUES (8, 7, 9, CAST(5.00 AS Decimal(6, 2)), 1)
INSERT [dbo].[tbProductoProveedor] ([codProveedor], [codProducto], [codPresentacion], [precioCompra], [estado]) VALUES (8, 8, 7, CAST(180.00 AS Decimal(6, 2)), 1)
INSERT [dbo].[tbProductoProveedor] ([codProveedor], [codProducto], [codPresentacion], [precioCompra], [estado]) VALUES (8, 9, 7, CAST(175.00 AS Decimal(6, 2)), 1)
INSERT [dbo].[tbProductoProveedor] ([codProveedor], [codProducto], [codPresentacion], [precioCompra], [estado]) VALUES (9, 8, 7, CAST(174.50 AS Decimal(6, 2)), 1)
INSERT [dbo].[tbProductoProveedor] ([codProveedor], [codProducto], [codPresentacion], [precioCompra], [estado]) VALUES (9, 10, 7, CAST(185.00 AS Decimal(6, 2)), 1)
INSERT [dbo].[tbProductoProveedor] ([codProveedor], [codProducto], [codPresentacion], [precioCompra], [estado]) VALUES (10, 9, 7, CAST(160.00 AS Decimal(6, 2)), 1)
SET IDENTITY_INSERT [dbo].[tbProductoTransaccion] ON 

INSERT [dbo].[tbProductoTransaccion] ([codProductoTransaccion], [codTransaccion], [codProducto], [codPresentacion], [cantidad], [precioCompra]) VALUES (33, 1024, 10, 7, 2, CAST(185.00 AS Decimal(6, 2)))
INSERT [dbo].[tbProductoTransaccion] ([codProductoTransaccion], [codTransaccion], [codProducto], [codPresentacion], [cantidad], [precioCompra]) VALUES (34, 1024, 8, 7, 2, CAST(174.50 AS Decimal(6, 2)))
INSERT [dbo].[tbProductoTransaccion] ([codProductoTransaccion], [codTransaccion], [codProducto], [codPresentacion], [cantidad], [precioCompra]) VALUES (35, 1025, 8, 9, 6, NULL)
INSERT [dbo].[tbProductoTransaccion] ([codProductoTransaccion], [codTransaccion], [codProducto], [codPresentacion], [cantidad], [precioCompra]) VALUES (36, 1026, 9, 7, 15, CAST(160.00 AS Decimal(6, 2)))
SET IDENTITY_INSERT [dbo].[tbProductoTransaccion] OFF
SET IDENTITY_INSERT [dbo].[tbProveedor] ON 

INSERT [dbo].[tbProveedor] ([codProveedor], [proveedor], [telefono], [direccion], [estado], [deuda]) VALUES (8, N'Camilo', N'654987654', N'cobán', 1, CAST(1800.00 AS Decimal(8, 2)))
INSERT [dbo].[tbProveedor] ([codProveedor], [proveedor], [telefono], [direccion], [estado], [deuda]) VALUES (9, N'Pollo Rey', N'9879879', N'Cobán', 1, CAST(1591.50 AS Decimal(8, 2)))
INSERT [dbo].[tbProveedor] ([codProveedor], [proveedor], [telefono], [direccion], [estado], [deuda]) VALUES (10, N'Dany Mayen ', N'545', NULL, 1, CAST(400.00 AS Decimal(8, 2)))
SET IDENTITY_INSERT [dbo].[tbProveedor] OFF
SET IDENTITY_INSERT [dbo].[tbRol] ON 

INSERT [dbo].[tbRol] ([codRol], [rol]) VALUES (1, N'Desarrollador')
INSERT [dbo].[tbRol] ([codRol], [rol]) VALUES (2, N'Administrador')
INSERT [dbo].[tbRol] ([codRol], [rol]) VALUES (3, N'Vendedor')
INSERT [dbo].[tbRol] ([codRol], [rol]) VALUES (9, N'Contador')
SET IDENTITY_INSERT [dbo].[tbRol] OFF
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (1, 1, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (1, 2, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (1, 3, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (1, 4, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (1, 5, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (1, 6, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (1, 7, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (2, 1, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (2, 2, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (2, 3, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (2, 4, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (2, 5, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (2, 6, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (2, 7, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (3, 1, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (3, 2, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (3, 3, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (3, 4, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (3, 5, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (3, 6, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (3, 7, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (9, 1, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (9, 2, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (9, 3, 1)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (9, 4, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (9, 5, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (9, 6, 0)
INSERT [dbo].[tbRolPermiso] ([codRol], [codPermiso], [estado]) VALUES (9, 7, 0)
SET IDENTITY_INSERT [dbo].[tbTransaccion] ON 

INSERT [dbo].[tbTransaccion] ([codTransaccion], [codProveedor], [codTipoTransaccion], [descripcion], [fecha], [estado], [codUsuario]) VALUES (1024, 9, 1, N'-', CAST(N'2018-11-13T09:49:12.970' AS DateTime), 1, 1)
INSERT [dbo].[tbTransaccion] ([codTransaccion], [codProveedor], [codTipoTransaccion], [descripcion], [fecha], [estado], [codUsuario]) VALUES (1025, NULL, 2, N'Despensa ', CAST(N'2018-11-13T10:40:07.310' AS DateTime), 1, 1)
INSERT [dbo].[tbTransaccion] ([codTransaccion], [codProveedor], [codTipoTransaccion], [descripcion], [fecha], [estado], [codUsuario]) VALUES (1026, 10, 1, N'Pollo ', CAST(N'2018-11-13T10:48:21.593' AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[tbTransaccion] OFF
SET IDENTITY_INSERT [dbo].[tbTransaccionCaja] ON 

INSERT [dbo].[tbTransaccionCaja] ([codTransaccionCaja], [codUsuario], [tipoTransaccion], [cantidad], [descripcion], [fecha]) VALUES (8, NULL, 1, CAST(50.00 AS Decimal(8, 2)), NULL, CAST(N'2018-11-13T09:54:06.520' AS DateTime))
INSERT [dbo].[tbTransaccionCaja] ([codTransaccionCaja], [codUsuario], [tipoTransaccion], [cantidad], [descripcion], [fecha]) VALUES (9, NULL, 1, CAST(68.50 AS Decimal(8, 2)), NULL, CAST(N'2018-11-13T09:58:12.723' AS DateTime))
INSERT [dbo].[tbTransaccionCaja] ([codTransaccionCaja], [codUsuario], [tipoTransaccion], [cantidad], [descripcion], [fecha]) VALUES (10, 1, 2, CAST(100.00 AS Decimal(8, 2)), N'Retiro semanal', CAST(N'2018-11-13T10:08:51.687' AS DateTime))
INSERT [dbo].[tbTransaccionCaja] ([codTransaccionCaja], [codUsuario], [tipoTransaccion], [cantidad], [descripcion], [fecha]) VALUES (11, NULL, 1, CAST(324.00 AS Decimal(8, 2)), NULL, CAST(N'2018-11-13T10:37:07.497' AS DateTime))
INSERT [dbo].[tbTransaccionCaja] ([codTransaccionCaja], [codUsuario], [tipoTransaccion], [cantidad], [descripcion], [fecha]) VALUES (12, NULL, 1, CAST(0.00 AS Decimal(8, 2)), NULL, CAST(N'2018-11-13T10:37:44.143' AS DateTime))
INSERT [dbo].[tbTransaccionCaja] ([codTransaccionCaja], [codUsuario], [tipoTransaccion], [cantidad], [descripcion], [fecha]) VALUES (13, NULL, 1, CAST(0.00 AS Decimal(8, 2)), NULL, CAST(N'2018-11-13T11:09:28.647' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbTransaccionCaja] OFF
SET IDENTITY_INSERT [dbo].[tbUsuario] ON 

INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (1, 1, N'pedro', N'Pedro Andrés Vega Stalling', N'pedlito', 1)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (2, 3, N'nichamus', N'Nelson Ivan Cucul', N'suzete', 1)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (3, 2, N'yami', N'Georgina Yamlieth Gutiérrez', N'bewe', 1)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (4, 2, N'juancho', N'Juan Luis Vega Stalling', N'juancho', 1)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (5, 3, N'carlos', N'Carlos Albarado Coy', N'carlos', 0)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (6, 3, N'gerardo', N'Gerardo Cu Yat', N'gerardo', 1)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (7, 3, N'yovani', N'Yovani de Leon', N'12345', 0)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (8, 3, N'maria', N'Maria Jose Hernandez', N'maria', 1)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (10, 2, N'AlejandraKlug', N'Alejandra Klug', N'alejandra', 1)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (11, 9, N'manuelklug', N'Manuel klug', N'manu', 0)
INSERT [dbo].[tbUsuario] ([codUsuario], [codRol], [usuario], [nombre], [password], [estado]) VALUES (12, 2, N'PabloKR10', N'Pablo Klug', N'Klugramirez18', 1)
SET IDENTITY_INSERT [dbo].[tbUsuario] OFF
SET IDENTITY_INSERT [dbo].[tbVenta] ON 

INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (19, NULL, 1, CAST(N'2018-11-13T09:54:06.500' AS DateTime), NULL, 1)
INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (20, NULL, 1, CAST(N'2018-11-13T09:58:12.660' AS DateTime), NULL, 1)
INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (21, 6, 1, CAST(N'2018-11-13T10:00:41.023' AS DateTime), NULL, 1)
INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (22, NULL, 1, CAST(N'2018-11-13T10:37:07.457' AS DateTime), NULL, 1)
INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (23, NULL, 1, CAST(N'2018-11-13T10:37:44.130' AS DateTime), NULL, 1)
INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (24, 8, 1, CAST(N'2018-11-13T10:44:53.503' AS DateTime), NULL, 1)
INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (25, NULL, 12, CAST(N'2018-11-13T11:09:28.570' AS DateTime), NULL, 1)
INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (26, 6, 10, CAST(N'2018-11-13T11:11:42.833' AS DateTime), NULL, 1)
INSERT [dbo].[tbVenta] ([codVenta], [codDeudor], [codUsuario], [fecha], [numFactura], [estado]) VALUES (27, 6, 10, CAST(N'2018-11-13T11:15:17.737' AS DateTime), NULL, 1)
SET IDENTITY_INSERT [dbo].[tbVenta] OFF
INSERT [dbo].[tbVentaProducto] ([codVenta], [codProducto], [codPresentacion], [cantidad], [precioVenta]) VALUES (19, 7, 9, CAST(2.00 AS Decimal(6, 2)), CAST(25.00 AS Decimal(8, 2)))
INSERT [dbo].[tbVentaProducto] ([codVenta], [codProducto], [codPresentacion], [cantidad], [precioVenta]) VALUES (20, 8, 9, CAST(6.00 AS Decimal(6, 2)), CAST(10.00 AS Decimal(8, 2)))
INSERT [dbo].[tbVentaProducto] ([codVenta], [codProducto], [codPresentacion], [cantidad], [precioVenta]) VALUES (20, 10, 9, CAST(1.00 AS Decimal(6, 2)), CAST(8.50 AS Decimal(8, 2)))
INSERT [dbo].[tbVentaProducto] ([codVenta], [codProducto], [codPresentacion], [cantidad], [precioVenta]) VALUES (21, 8, 9, CAST(3.00 AS Decimal(6, 2)), CAST(10.00 AS Decimal(8, 2)))
INSERT [dbo].[tbVentaProducto] ([codVenta], [codProducto], [codPresentacion], [cantidad], [precioVenta]) VALUES (22, 9, 9, CAST(54.00 AS Decimal(6, 2)), CAST(6.00 AS Decimal(8, 2)))
ALTER TABLE [dbo].[tbUsuario] ADD  CONSTRAINT [DF_tbUsuario_estado]  DEFAULT ((1)) FOR [estado]
GO
ALTER TABLE [dbo].[tbPagoDeudor]  WITH CHECK ADD  CONSTRAINT [FK_tbPagoDeudor_tbDeudor] FOREIGN KEY([codDeudor])
REFERENCES [dbo].[tbDeudor] ([codDeudor])
GO
ALTER TABLE [dbo].[tbPagoDeudor] CHECK CONSTRAINT [FK_tbPagoDeudor_tbDeudor]
GO
ALTER TABLE [dbo].[tbPagoDeudor]  WITH CHECK ADD  CONSTRAINT [FK_tbPagoDeudor_tbUsuario] FOREIGN KEY([codUsuario])
REFERENCES [dbo].[tbUsuario] ([codUsuario])
GO
ALTER TABLE [dbo].[tbPagoDeudor] CHECK CONSTRAINT [FK_tbPagoDeudor_tbUsuario]
GO
ALTER TABLE [dbo].[tbPagoProveedor]  WITH CHECK ADD  CONSTRAINT [FK_tbPagoProveedor_tbProveedor] FOREIGN KEY([codProveedor])
REFERENCES [dbo].[tbProveedor] ([codProveedor])
GO
ALTER TABLE [dbo].[tbPagoProveedor] CHECK CONSTRAINT [FK_tbPagoProveedor_tbProveedor]
GO
ALTER TABLE [dbo].[tbPagoProveedor]  WITH CHECK ADD  CONSTRAINT [FK_tbPagoProveedor_tbUsuario] FOREIGN KEY([codUsuario])
REFERENCES [dbo].[tbUsuario] ([codUsuario])
GO
ALTER TABLE [dbo].[tbPagoProveedor] CHECK CONSTRAINT [FK_tbPagoProveedor_tbUsuario]
GO
ALTER TABLE [dbo].[tbProducto]  WITH CHECK ADD  CONSTRAINT [FK_tbProducto_tbCategoria] FOREIGN KEY([codCategoria])
REFERENCES [dbo].[tbCategoria] ([codCategoria])
GO
ALTER TABLE [dbo].[tbProducto] CHECK CONSTRAINT [FK_tbProducto_tbCategoria]
GO
ALTER TABLE [dbo].[tbProductoPresentacion]  WITH CHECK ADD  CONSTRAINT [FK_tbProductoPresentacion_tbPresentacion] FOREIGN KEY([codPresentacion])
REFERENCES [dbo].[tbPresentacion] ([codPresentacion])
GO
ALTER TABLE [dbo].[tbProductoPresentacion] CHECK CONSTRAINT [FK_tbProductoPresentacion_tbPresentacion]
GO
ALTER TABLE [dbo].[tbProductoPresentacion]  WITH CHECK ADD  CONSTRAINT [FK_tbProductoPresentacion_tbProducto] FOREIGN KEY([codProducto])
REFERENCES [dbo].[tbProducto] ([codProducto])
GO
ALTER TABLE [dbo].[tbProductoPresentacion] CHECK CONSTRAINT [FK_tbProductoPresentacion_tbProducto]
GO
ALTER TABLE [dbo].[tbProductoProveedor]  WITH CHECK ADD  CONSTRAINT [FK_tbProductoProveedor_tbProductoPresentacion] FOREIGN KEY([codProducto], [codPresentacion])
REFERENCES [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion])
GO
ALTER TABLE [dbo].[tbProductoProveedor] CHECK CONSTRAINT [FK_tbProductoProveedor_tbProductoPresentacion]
GO
ALTER TABLE [dbo].[tbProductoProveedor]  WITH CHECK ADD  CONSTRAINT [FK_tbProductoProveedor_tbProveedor] FOREIGN KEY([codProveedor])
REFERENCES [dbo].[tbProveedor] ([codProveedor])
GO
ALTER TABLE [dbo].[tbProductoProveedor] CHECK CONSTRAINT [FK_tbProductoProveedor_tbProveedor]
GO
ALTER TABLE [dbo].[tbProductoTransaccion]  WITH CHECK ADD  CONSTRAINT [FK_tbProductoTransaccion_tbProductoPresentacion1] FOREIGN KEY([codProducto], [codPresentacion])
REFERENCES [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion])
GO
ALTER TABLE [dbo].[tbProductoTransaccion] CHECK CONSTRAINT [FK_tbProductoTransaccion_tbProductoPresentacion1]
GO
ALTER TABLE [dbo].[tbProductoTransaccion]  WITH CHECK ADD  CONSTRAINT [FK_tbProductoTransaccion_tbTransaccion1] FOREIGN KEY([codTransaccion])
REFERENCES [dbo].[tbTransaccion] ([codTransaccion])
GO
ALTER TABLE [dbo].[tbProductoTransaccion] CHECK CONSTRAINT [FK_tbProductoTransaccion_tbTransaccion1]
GO
ALTER TABLE [dbo].[tbRolPermiso]  WITH CHECK ADD  CONSTRAINT [FK_tbPermisoDenegado_tbPermiso] FOREIGN KEY([codPermiso])
REFERENCES [dbo].[tbPermiso] ([codPermiso])
GO
ALTER TABLE [dbo].[tbRolPermiso] CHECK CONSTRAINT [FK_tbPermisoDenegado_tbPermiso]
GO
ALTER TABLE [dbo].[tbRolPermiso]  WITH CHECK ADD  CONSTRAINT [FK_tbPermisoDenegado_tbRol] FOREIGN KEY([codRol])
REFERENCES [dbo].[tbRol] ([codRol])
GO
ALTER TABLE [dbo].[tbRolPermiso] CHECK CONSTRAINT [FK_tbPermisoDenegado_tbRol]
GO
ALTER TABLE [dbo].[tbTransaccion]  WITH CHECK ADD  CONSTRAINT [FK_tbTransaccion_tbProveedor] FOREIGN KEY([codProveedor])
REFERENCES [dbo].[tbProveedor] ([codProveedor])
GO
ALTER TABLE [dbo].[tbTransaccion] CHECK CONSTRAINT [FK_tbTransaccion_tbProveedor]
GO
ALTER TABLE [dbo].[tbTransaccion]  WITH CHECK ADD  CONSTRAINT [FK_tbTransaccion_tbUsuario] FOREIGN KEY([codUsuario])
REFERENCES [dbo].[tbUsuario] ([codUsuario])
GO
ALTER TABLE [dbo].[tbTransaccion] CHECK CONSTRAINT [FK_tbTransaccion_tbUsuario]
GO
ALTER TABLE [dbo].[tbTransaccionCaja]  WITH CHECK ADD  CONSTRAINT [FK_tbTransaccionCaja_tbUsuario] FOREIGN KEY([codUsuario])
REFERENCES [dbo].[tbUsuario] ([codUsuario])
GO
ALTER TABLE [dbo].[tbTransaccionCaja] CHECK CONSTRAINT [FK_tbTransaccionCaja_tbUsuario]
GO
ALTER TABLE [dbo].[tbUsuario]  WITH CHECK ADD  CONSTRAINT [FK_tbUsuario_tbRol] FOREIGN KEY([codRol])
REFERENCES [dbo].[tbRol] ([codRol])
GO
ALTER TABLE [dbo].[tbUsuario] CHECK CONSTRAINT [FK_tbUsuario_tbRol]
GO
ALTER TABLE [dbo].[tbVenta]  WITH CHECK ADD  CONSTRAINT [FK_tbVenta_tbDeudor] FOREIGN KEY([codDeudor])
REFERENCES [dbo].[tbDeudor] ([codDeudor])
GO
ALTER TABLE [dbo].[tbVenta] CHECK CONSTRAINT [FK_tbVenta_tbDeudor]
GO
ALTER TABLE [dbo].[tbVenta]  WITH CHECK ADD  CONSTRAINT [FK_tbVenta_tbUsuario] FOREIGN KEY([codUsuario])
REFERENCES [dbo].[tbUsuario] ([codUsuario])
GO
ALTER TABLE [dbo].[tbVenta] CHECK CONSTRAINT [FK_tbVenta_tbUsuario]
GO
ALTER TABLE [dbo].[tbVentaProducto]  WITH CHECK ADD  CONSTRAINT [FK_tbVentaProducto_tbProductoPresentacion] FOREIGN KEY([codProducto], [codPresentacion])
REFERENCES [dbo].[tbProductoPresentacion] ([codProducto], [codPresentacion])
GO
ALTER TABLE [dbo].[tbVentaProducto] CHECK CONSTRAINT [FK_tbVentaProducto_tbProductoPresentacion]
GO
ALTER TABLE [dbo].[tbVentaProducto]  WITH CHECK ADD  CONSTRAINT [FK_tbVentaProducto_tbVenta] FOREIGN KEY([codVenta])
REFERENCES [dbo].[tbVenta] ([codVenta])
GO
ALTER TABLE [dbo].[tbVentaProducto] CHECK CONSTRAINT [FK_tbVentaProducto_tbVenta]
GO
/****** Object:  StoredProcedure [dbo].[DeshabilitarProductos]    Script Date: 10/12/2018 08:04:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeshabilitarProductos] @codProd int, @codPres int
AS
	BEGIN
		UPDATE tbProductoProveedor 
		SET estado = 0
		WHERE codProducto = @codProd AND codPresentacion = @codPres
	END
GO
USE [master]
GO
ALTER DATABASE [dbHeredades] SET  READ_WRITE 
GO
