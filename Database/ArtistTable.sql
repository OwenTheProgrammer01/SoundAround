USE [SoundAround]
GO

/****** Object:  Table [dbo].[Artist]    Script Date: 2/10/2024 17:43:20 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Artist]') AND type in (N'U'))
DROP TABLE [dbo].[Artist]
GO

/****** Object:  Table [dbo].[Artist]    Script Date: 2/10/2024 17:43:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Artist](
	[Artist_ID] [int] IDENTITY(1,1) NOT NULL,
	[Artist] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_Artiest] PRIMARY KEY CLUSTERED 
(
	[Artist_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

