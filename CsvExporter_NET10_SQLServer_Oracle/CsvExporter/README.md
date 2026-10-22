# CsvExporter — .NET 10

Solution starter: Clean Architecture, SOLID, Strategy, Registry/Factory, validation pipeline, SQL Server metadata, streaming CSV and Windows Task Scheduler.

## Prerequisites
- .NET 10 SDK and Windows Server with compatible .NET runtime
- SQL Server for metadata and SQL Server reporting database
- A service account with read/execute permission on approved reporting stored procedures and write permission on destination directory

## Setup
1. Execute `sql/001_schema.sql` on the metadata database.
2. Review `sql/002_seed_example.sql`; create `reporting.usp_ExportFxOperations @ReferenceDate date` returning TradeId, TradeDate, Amount; enable definition after approval.
3. Configure connection strings securely (environment variables `ConnectionStrings__Metadata` and `ConnectionStrings__Reporting` or managed deployment secrets). Do not commit credentials.
4. Run `dotnet restore`, `dotnet build`, `dotnet test`.
5. Run `dotnet run --project src/CsvExporter.Host -- --date 2026-10-19 --export 101`.
6. Publish: `dotnet publish src/CsvExporter.Host -c Release -r win-x64 --self-contained false -o publish`.
7. Register task under a service identity; configure alerts and task history.

## Design notes
- The default batch date is yesterday, local server time.
- Definitions are stored as JSON in SQL Server. This is a pragmatic initial implementation, not a normalized metadata schema.
- Query source accepts ONLY a schema-qualified approved SQL Server stored procedure with `@ReferenceDate`.
- Export execution has a UNIQUE key on export/date/version; duplicates are skipped. Failed runs require explicit operator intervention; a retry policy and lease recovery must be designed before production.
- The local publisher uses a partial file and rename, and refuses overwrite. Publishing across filesystems/shares may not be atomic.
- CSV formula injection protection is a conservative prefix policy; confirm requirements for numeric signs and downstream consumers.
- This starter does not include Oracle, SFTP, an admin UI, metadata approval/version history, structured persistent logging, or automatic stale-run recovery. Implement and test these before claiming production readiness.
- Validate access controls and use approved destination roots; do not allow arbitrary paths or arbitrary SQL from untrusted editors.
- The app was generated without a .NET SDK in the authoring environment; compilation and tests have not been executed here.

## SQL Server + Oracle DataSet patterns
See `docs/DatabaseAccess.md` for connection factory, stored procedure executor, and Listas/Clientes repository examples. These repositories are illustrative and not yet integrated with the metadata export engine.
