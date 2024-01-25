USE [WarikakeWebContext]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CsvMigration]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CsvMigration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NOT NULL,
	[inputDate] [nvarchar](max) NOT NULL,
	[buyDate] [nvarchar](max) NOT NULL,
	[kindName] [nvarchar](max) NOT NULL,
	[buyAmount] [nvarchar](max) NOT NULL,
	[pf1] [nvarchar](max) NOT NULL,
	[pf2] [nvarchar](max) NOT NULL,
	[pf3] [nvarchar](max) NOT NULL,
	[pa1] [nvarchar](max) NOT NULL,
	[pa2] [nvarchar](max) NOT NULL,
	[pa3] [nvarchar](max) NOT NULL,
	[pr1] [nvarchar](max) NOT NULL,
	[pr2] [nvarchar](max) NOT NULL,
	[pr3] [nvarchar](max) NOT NULL,
	[buyStatus] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_CsvMigration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MGenre]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MGenre](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[GenreName] [nvarchar](max) NOT NULL,
	[GenreId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_MGenre] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MGroup]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[GroupId] [int] NOT NULL,
	[GroupName] [nvarchar](max) NOT NULL,
	[UserId] [int] NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_MGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MMember]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MMember](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[GroupId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_MMember] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MUser]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[UserId] [int] NOT NULL,
	[UserName] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_MUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TCost]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TCost](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CostId] [int] NOT NULL,
	[status] [int] NULL,
	[CostTitle] [nvarchar](max) NULL,
	[GroupId] [int] NOT NULL,
	[GenreId] [int] NOT NULL,
	[GenreName] [nvarchar](max) NULL,
	[ProvisionFlg] [int] NOT NULL,
	[CostAmount] [int] NOT NULL,
	[CostDate] [datetime2](7) NOT NULL,
	[CostStatus] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TCost] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TCostSubscribe]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TCostSubscribe](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubscribeId] [int] NOT NULL,
	[status] [int] NULL,
	[CostTitle] [nvarchar](max) NULL,
	[GroupId] [int] NOT NULL,
	[GenreId] [int] NOT NULL,
	[GenreName] [nvarchar](max) NULL,
	[ProvisionFlg] [int] NOT NULL,
	[CostAmount] [int] NOT NULL,
	[CostStatus] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TCostSubscribe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TDateSubscribe]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TDateSubscribe](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[SubscribeId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[m1] [bit] NOT NULL,
	[m2] [bit] NOT NULL,
	[m3] [bit] NOT NULL,
	[m4] [bit] NOT NULL,
	[m5] [bit] NOT NULL,
	[m6] [bit] NOT NULL,
	[m7] [bit] NOT NULL,
	[m8] [bit] NOT NULL,
	[m9] [bit] NOT NULL,
	[m10] [bit] NOT NULL,
	[m11] [bit] NOT NULL,
	[m12] [bit] NOT NULL,
	[r1] [bit] NOT NULL,
	[r2] [bit] NOT NULL,
	[r3] [bit] NOT NULL,
	[r4] [bit] NOT NULL,
	[r5] [bit] NOT NULL,
	[w1] [bit] NOT NULL,
	[w2] [bit] NOT NULL,
	[w3] [bit] NOT NULL,
	[w4] [bit] NOT NULL,
	[w5] [bit] NOT NULL,
	[w6] [bit] NOT NULL,
	[w7] [bit] NOT NULL,
	[d1] [bit] NOT NULL,
	[d2] [bit] NOT NULL,
	[d3] [bit] NOT NULL,
	[d4] [bit] NOT NULL,
	[d5] [bit] NOT NULL,
	[d6] [bit] NOT NULL,
	[d7] [bit] NOT NULL,
	[d8] [bit] NOT NULL,
	[d9] [bit] NOT NULL,
	[d10] [bit] NOT NULL,
	[d11] [bit] NOT NULL,
	[d12] [bit] NOT NULL,
	[d13] [bit] NOT NULL,
	[d14] [bit] NOT NULL,
	[d15] [bit] NOT NULL,
	[d16] [bit] NOT NULL,
	[d17] [bit] NOT NULL,
	[d18] [bit] NOT NULL,
	[d19] [bit] NOT NULL,
	[d20] [bit] NOT NULL,
	[d21] [bit] NOT NULL,
	[d22] [bit] NOT NULL,
	[d23] [bit] NOT NULL,
	[d24] [bit] NOT NULL,
	[d25] [bit] NOT NULL,
	[d26] [bit] NOT NULL,
	[d27] [bit] NOT NULL,
	[d28] [bit] NOT NULL,
	[d29] [bit] NOT NULL,
	[d30] [bit] NOT NULL,
	[d31] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TDateSubscribe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TPay]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TPay](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[CostId] [int] NOT NULL,
	[PayId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[PayAmount] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TPay] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TPaySubscribe]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TPaySubscribe](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[SubscribeId] [int] NOT NULL,
	[PayId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[PayAmount] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TPaySubscribe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TRepay]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TRepay](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[CostId] [int] NOT NULL,
	[RepayId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[RepayAmount] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TRepay] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TRepaySubscribe]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TRepaySubscribe](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[SubscribeId] [int] NOT NULL,
	[RepayId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[RepayAmount] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TRepaySubscribe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TSubscribe]    Script Date: 2024/01/23 0:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSubscribe](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[SubscribeId] [int] NOT NULL,
	[CostId] [int] NOT NULL,
	[SubscribedDate] [datetime2](7) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreateUser] [nvarchar](max) NULL,
	[CreatePg] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdateUser] [nvarchar](max) NULL,
	[UpdatePg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TSubscribe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[TCost] ADD  DEFAULT (NEXT VALUE FOR [CostIdSeq]) FOR [CostId]
GO
ALTER TABLE [dbo].[TCostSubscribe] ADD  DEFAULT (NEXT VALUE FOR [SubscribeIdSeq]) FOR [SubscribeId]
GO
