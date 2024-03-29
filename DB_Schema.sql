USE [MembershipQR]
GO
/****** Object:  User [memQR]    Script Date: 05/31/2019 21:06:18 ******/
CREATE USER [memQR] FOR LOGIN [memQR] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[QR_Use_History]    Script Date: 05/31/2019 21:06:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QR_Use_History](
	[UserID] [nvarchar](50) NULL,
	[QRCode] [nvarchar](50) NULL,
	[MachineID] [nvarchar](50) NULL,
	[TransID] [nvarchar](50) NULL,
	[TransTime] [datetime] NULL,
	[Currency] [nvarchar](10) NULL,
	[UnitPrice] [decimal](10, 2) NULL,
	[UpdateTime] [datetime] NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_QR_Transaction] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QR_Transaction]    Script Date: 05/31/2019 21:06:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QR_Transaction](
	[UserID] [nvarchar](50) NOT NULL,
	[TransId] [nvarchar](50) NOT NULL,
	[TransTime] [datetime] NULL,
	[ActionType] [nvarchar](50) NULL,
	[QRCode] [nvarchar](50) NULL,
	[Qty] [int] NULL,
	[SourceQR] [nvarchar](50) NULL,
	[Amt] [decimal](10, 2) NULL,
	[PromotionCode] [nvarchar](50) NULL,
	[UpdateTime] [datetime] NULL,
	[ifSuccess] [bit] NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_QR_Transaction_1] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[TransId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QR_Status]    Script Date: 05/31/2019 21:06:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QR_Status](
	[UserID] [nvarchar](50) NOT NULL,
	[QRCode] [nvarchar](50) NOT NULL,
	[Company] [nvarchar](50) NOT NULL,
	[Qty] [int] NULL,
	[Product] [nvarchar](50) NOT NULL,
	[Currency] [nvarchar](10) NULL,
	[UnitPrice] [decimal](10, 2) NULL,
	[ExpiryDate] [datetime] NULL,
	[Remark] [nvarchar](50) NULL,
	[ifValid] [bit] NULL,
	[LastUpdateTime] [datetime] NULL,
 CONSTRAINT [PK_QR_Status] PRIMARY KEY CLUSTERED 
(
	[QRCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductList]    Script Date: 05/31/2019 21:06:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductList](
	[Company] [nvarchar](50) NOT NULL,
	[Product] [nvarchar](50) NOT NULL,
	[Currency] [nvarchar](10) NULL,
	[UnitPrice] [decimal](10, 2) NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UpdateBy] [nvarchar](50) NULL,
	[UpdateTime] [datetime] NOT NULL,
	[ifValid] [bit] NULL,
 CONSTRAINT [PK_ProductList] PRIMARY KEY CLUSTERED 
(
	[Company] ASC,
	[Product] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Default [DF_ProductList_UpdateTime]    Script Date: 05/31/2019 21:06:18 ******/
ALTER TABLE [dbo].[ProductList] ADD  CONSTRAINT [DF_ProductList_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO
/****** Object:  Default [DF_ProductList_ifValid]    Script Date: 05/31/2019 21:06:18 ******/
ALTER TABLE [dbo].[ProductList] ADD  CONSTRAINT [DF_ProductList_ifValid]  DEFAULT ((1)) FOR [ifValid]
GO
/****** Object:  Default [DF_QR_Status_Qty]    Script Date: 05/31/2019 21:06:18 ******/
ALTER TABLE [dbo].[QR_Status] ADD  CONSTRAINT [DF_QR_Status_Qty]  DEFAULT ((0)) FOR [Qty]
GO
/****** Object:  Default [DF_QR_Status_ifValid]    Script Date: 05/31/2019 21:06:18 ******/
ALTER TABLE [dbo].[QR_Status] ADD  CONSTRAINT [DF_QR_Status_ifValid]  DEFAULT ((0)) FOR [ifValid]
GO
/****** Object:  Default [DF_Table_1_UpdateTime]    Script Date: 05/31/2019 21:06:18 ******/
ALTER TABLE [dbo].[QR_Status] ADD  CONSTRAINT [DF_Table_1_UpdateTime]  DEFAULT (getdate()) FOR [LastUpdateTime]
GO
/****** Object:  Default [DF_QR_Transaction_UpdateTime_1]    Script Date: 05/31/2019 21:06:18 ******/
ALTER TABLE [dbo].[QR_Transaction] ADD  CONSTRAINT [DF_QR_Transaction_UpdateTime_1]  DEFAULT (getdate()) FOR [UpdateTime]
GO
/****** Object:  Default [DF_QR_Transaction_ifSuccess]    Script Date: 05/31/2019 21:06:18 ******/
ALTER TABLE [dbo].[QR_Transaction] ADD  CONSTRAINT [DF_QR_Transaction_ifSuccess]  DEFAULT ((0)) FOR [ifSuccess]
GO
/****** Object:  Default [DF_QR_Transaction_UpdateTime]    Script Date: 05/31/2019 21:06:18 ******/
ALTER TABLE [dbo].[QR_Use_History] ADD  CONSTRAINT [DF_QR_Transaction_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO
