IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Schedule]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Schedule](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](120) NOT NULL,
	[CronExpression] [varchar](25) NOT NULL,
	[NextNotification] [datetime] NOT NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

ALTER TABLE [dbo].[Schedule] ADD  CONSTRAINT [DF_Schedule_Id]  DEFAULT (newid()) FOR [Id]
GO
