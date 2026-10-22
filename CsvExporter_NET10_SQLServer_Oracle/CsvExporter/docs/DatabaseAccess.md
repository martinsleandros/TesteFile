# Database access: SQL Server and Oracle

- `IDbConnectionFactory` chooses the provider; `IStoredProcedureExecutor` executes approved stored procedures and returns a `DataSet`.
- `IListasRepository` and `IClientesRepository` demonstrate distinct database providers. Change the providers and procedure names to match your environment.
- SQL Server procedure `reporting.usp_ExportListas` receives `@ReferenceDate date` and returns one `SELECT` result set.
- Oracle procedure `PKG_EXPORT.EXPORT_CLIENTES` receives `p_reference_date IN DATE, p_result OUT SYS_REFCURSOR` (cursor parameter name must match). Oracle `BindByName=true`.
- Example Oracle signature:
```sql
PROCEDURE EXPORT_CLIENTES(p_reference_date IN DATE, p_result OUT SYS_REFCURSOR);
```
- The generic executor supports one tabular result set per call; `DataSet` is an in-memory representation. For large exports, use `DbDataReader` streaming (existing export pipeline) instead of DataSet.
- The repository examples are demonstration entry points and are **not yet wired to the existing metadata-driven export pipeline**. The existing pipeline still uses `SqlServerDataSourceReader`.
- Procedure names must come from approved configuration. Identifier regex is not a security boundary; grant execute-only rights to approved procedures and do not permit untrusted configuration editors.
- Connection strings should be injected from a secure secret store or environment variables. `ReportingOracle` in appsettings is a placeholder and must not contain real credentials in source control.
- `DataTable.Load` is synchronous, so cancellation is not guaranteed during the in-memory materialization stage.
- Verify the Oracle package version and compatibility during `dotnet restore` in your environment. The project has not been compiled in this environment.
