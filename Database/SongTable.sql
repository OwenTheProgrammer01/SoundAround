USE [SoundAround]
GO

/****** Object:  Table [dbo].[Song]    Script Date: 2/10/2024 17:42:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Song]') AND type in (N'U'))
DROP TABLE [dbo].[Song]
GO

/****** Object:  Table [dbo].[Song]    Script Date: 2/10/2024 17:42:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Song](
	[Song_ID] [int] IDENTITY(1,1) NOT NULL,
	[FileType_ID] [int] NOT NULL,
	[Artist_ID] [int] NOT NULL,
	[Album_ID] [int] NOT NULL,
	[SongFile] [varbinary](max) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Duration] [time](3) NOT NULL,
 CONSTRAINT [PK_Song] PRIMARY KEY CLUSTERED 
(
	[Song_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

