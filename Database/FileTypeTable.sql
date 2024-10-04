USE [SoundAround]
GO

/****** Object:  Table [dbo].[FileType]    Script Date: 2/10/2024 17:43:10 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FileType]') AND type in (N'U'))
DROP TABLE [dbo].[FileType]
GO

/****** Object:  Table [dbo].[FileType]    Script Date: 2/10/2024 17:43:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FileType](
	[FileType_ID] [int] IDENTITY(1,1) NOT NULL,
	[FileType] [nvarchar](5) NOT NULL,
 CONSTRAINT [PK_Bestandtype] PRIMARY KEY CLUSTERED 
(
	[FileType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

