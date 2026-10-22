CREATE TABLE dbo.ExportDefinition (
 ExportId int NOT NULL PRIMARY KEY,
 Enabled bit NOT NULL DEFAULT 1,
 DefinitionJson nvarchar(max) NOT NULL,
 CONSTRAINT CK_ExportDefinition_Json CHECK (ISJSON(DefinitionJson)=1)
);
GO
CREATE TABLE dbo.ExportExecution (
 ExecutionId uniqueidentifier NOT NULL PRIMARY KEY,
 ExportId int NOT NULL REFERENCES dbo.ExportDefinition(ExportId),
 ReferenceDate date NOT NULL,
 DefinitionVersion int NOT NULL,
 Status varchar(20) NOT NULL,
 StartedAt datetime2(3) NOT NULL,
 EndedAt datetime2(3) NULL,
 RecordCount bigint NULL,
 FilePath nvarchar(1024) NULL,
 Sha256 char(64) NULL,
 ErrorCode nvarchar(200) NULL,
 CONSTRAINT UQ_ExportExecution UNIQUE (ExportId,ReferenceDate,DefinitionVersion),
 CONSTRAINT CK_ExportExecution_Status CHECK (Status IN ('Running','Succeeded','Failed'))
);
GO
