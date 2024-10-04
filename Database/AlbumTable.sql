USE [SoundAround]
GO

/****** Object:  Table [dbo].[Album]    Script Date: 2/10/2024 17:43:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Album]') AND type in (N'U'))
DROP TABLE [dbo].[Album]
GO

/****** Object:  Table [dbo].[Album]    Script Date: 2/10/2024 17:43:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Album](
	[Album_ID] [int] IDENTITY(1,1) NOT NULL,
	[Album] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Album] PRIMARY KEY CLUSTERED 
(
	[Album_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

