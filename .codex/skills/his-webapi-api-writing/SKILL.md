---
name: his-webapi-api-writing
description: HIS_WebApi project API authoring guidance for adding or modifying ASP.NET Core controllers in PharSystem. Use when Codex works on HIS_WebApi APIs, including returnData request/response handling, SQLControl/HIS_DB_Lib data access, ServerSetting connection lookup, init table creation endpoints, Swagger XML comments, internal helper methods, file upload/download endpoints, or project-specific API naming and validation conventions.
---

# HIS_WebApi API Writing

## Read First

- Inspect nearby controllers under `HIS_WebApi/(API)*` before editing. Match the domain folder, controller naming, return format, table enum, and existing helper style.
- Treat `OldControllers` as legacy reference only. Prefer newer patterns from files such as `medUnit.cs`, `materialRequisition.cs`, `Logger.cs`, and `ServerSetting.cs`.
- Keep APIs compatible with existing clients: most endpoints return serialized `returnData` strings, not `ActionResult`, except file download or status-code-specific endpoints.

## Project Shape

- Target framework is ASP.NET Core `net5.0`; controllers use `[Route("api/[controller]")]` and `[ApiController]`.
- Functional APIs live in Chinese domain folders such as `(API)藥品資料`, `(API)申領`, `(API)盤點`, `(API)系統`.
- Shared types come mainly from `Basic`, `HIS_DB_Lib`, `SQLUI`, `MyUI`, and `MySql.Data.MySqlClient`.
- Swagger is enabled through XML comments in `Startup.cs`; public API methods should include XML docs and examples when behavior is non-trivial.

## Controller Pattern

Use this shape unless the surrounding file clearly uses another established style:

```csharp
[Route("api/[controller]")]
[ApiController]
public class featureName : ControllerBase
{
    static private MySqlSslMode SSLMode = MySqlSslMode.None;
    private static string tableName = "table_name";

    [HttpPost("init")]
    public string init()
    {
        returnData returnData = new returnData();
        try
        {
            return CheckCreatTable();
        }
        catch (Exception ex)
        {
            returnData.Code = -200;
            returnData.Result = ex.Message;
            return returnData.JsonSerializationt(true);
        }
    }

    [HttpPost("get_all")]
    public async Task<string> get_all([FromBody] returnData returnData)
    {
        MyTimerBasic timer = new MyTimerBasic();
        returnData.Method = "get_all";
        try
        {
            var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
            SQLControl sql = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
            List<object[]> rows = await sql.GetAllRowsAsync(null);
            List<MyClass> data = rows.SQLToClass<MyClass>();

            returnData.Code = 200;
            returnData.Result = $"取得資料共<{data.Count}>筆";
            returnData.Data = data;
            returnData.TimeTaken = $"{timer}";
            return await returnData.JsonSerializationtAsync(true);
        }
        catch (Exception ex)
        {
            returnData.Code = -200;
            returnData.Result = ex.Message;
            return returnData.JsonSerializationt(true);
        }
    }
}
```

## Request And Response Rules

- Accept common JSON APIs as `[FromBody] returnData returnData`.
- Use `returnData.ValueAry` for simple scalar inputs. Validate exact count and return a clear message such as `ValueAry錯誤，應為["GUID"]`.
- Use `returnData.Data` for object or list payloads. Parse with `ObjToClass<T>()`, `ObjToListClass<T>()`, or a single-object fallback when nearby APIs support both.
- Set `returnData.Method` to the route/action name for public endpoints.
- Set `returnData.Code = 200` on success and `returnData.Code = -200` on business or exception failure.
- Set `returnData.Result` with a human-readable Chinese message, usually including affected row count.
- Set `returnData.Data` to the typed object/list returned to callers; set it to `null` when validation fails and surrounding code does so.
- Set `returnData.TimeTaken = $"{myTimerBasic}"` before returning when a timer is used.
- Serialize with `returnData.JsonSerializationt(true)` or `await returnData.JsonSerializationtAsync(true)` for JSON APIs.

## Data Access

- Get DB connection info from `Method.GetServerInfo(...)` or `Method.GetServerInfoAsync(...)` where possible. Common VM lookup is `("Main", "網頁", "VM端")`.
- Use `ServerSettingController.GetAllServerSetting()` and `.MyFind(...)` when matching an existing controller that manually resolves server settings.
- Use `SQLControl` for reads/writes and the matching enum class from `HIS_DB_Lib`.
- Convert DB rows with `rows.SQLToClass<T>()` or `rows.SQLToClass<T, TEnum>()`.
- Convert objects to DB rows with `ClassToSQL<T>()` or `ClassToSQL<T, TEnum>()`.
- Prefer `GetRowsByDefultAsync`, `GetRowsByBetween`, `AddRowsAsync`, `UpdateRowsAsync`, `UpdateByDefulteExtra`, and `DeleteExtra` over raw SQL when they fit.
- If raw SQL is unavoidable, avoid interpolating user-provided values directly. Existing code has some direct string interpolation; do not copy it into new code when a structured `SQLControl` method can express the query.

## Init And Table Creation

- Provide an `init` endpoint when the API owns a table.
- Implement private `CheckCreatTable()` close to the controller.
- Prefer `MethodClass.CheckCreatTable<T>(sys_serverSettingClass)` for class-backed tables and `MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_xxx())` for enum-backed tables.
- Return table creation result as serialized JSON, following existing spelling `CheckCreatTable`.
- If a query may run before a table exists, either call `init()` at the start like older controllers or handle missing-table exceptions consistently with neighboring code.

## Naming And Routes

- Keep controller class names in the established style, often lower camel case or existing domain terms, because `[controller]` affects the URL.
- Prefer POST routes with verb-like snake_case names: `add`, `update_by_guid`, `delete_by_guid`, `get_by_code`, `get_all`, `download_excel_by_requestTime`.
- Preserve existing misspelled public route names when changing existing APIs; clients may depend on them.
- Use `[HttpPost("route_name")]` for new APIs. `[Route("route_name")]` plus `[HttpPost]` is also common, but avoid mixing styles within the same small edit.
- Add `[ApiExplorerSettings(IgnoreApi = true)]` to internal helper overloads that are not meant to appear in Swagger.

## Validation Checklist

- Check `returnData` itself, `Data`, and `ValueAry` before use.
- Validate required GUIDs, 藥碼, Med_GUID, date ranges, file presence, file extensions, and list counts.
- For list payloads, skip incomplete rows only when the existing feature does so; otherwise fail early with `Code = -200`.
- For add endpoints, assign `GUID = Guid.NewGuid().ToString()` when creating new rows.
- For audit/time fields, use project helpers such as `DateTime.Now.ToDateTimeString()`, `ToDateTimeString_6()`, or `ToDateString("-")` to match existing data formats.
- For state workflows, update only intended fields and keep Chinese status values consistent, for example `等待過帳` and `已過帳`.

## Swagger Docs

- Add XML `<summary>`, `<remarks>`, `<param>`, and `<returns>` for new public endpoints.
- Include a compact JSON example inside `<code>` for request shape, especially `ValueAry` and `Data`.
- Document whether `Data` accepts one object, a list, or both.
- Use `Swashbuckle.AspNetCore.Annotations.SwaggerResponse` when nearby endpoints in the same controller use it.

## File Endpoints

- Return `Task<ActionResult>` or `Task<IActionResult>` for downloads/uploads that need file streams or HTTP status codes.
- For Excel downloads, use content type `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` and filenames using `DateTime.Now.ToDateString("-")`.
- For uploads, read from `Request.Form.Files.FirstOrDefault()`, validate extension, ensure the target directory exists, and use `Path.Combine`.
- Never trust client-provided filenames for final storage paths; use fixed names or sanitized generated names.

## Internal API Calls

- Some controllers forward to another configured API through `Method.GetServerAPI(...)`, `Net.WEBApiPostJson(...)`, or `HttpClient`.
- Preserve this behavior when editing existing endpoints. If the configured external API returns data, return it in the same `returnData` shape rather than wrapping it again unless the surrounding endpoint does so.
- Add internal helper overloads for other controllers to call directly, and hide them from Swagger with `[ApiExplorerSettings(IgnoreApi = true)]`.

## Build And Verification

- Run `dotnet build HIS_WebApi/HIS_WebApi.csproj` after API changes when feasible.
- If build fails because `temp_version.txt`, generated `AssemblyInfo`, local DLLs, or environment files are missing, report that explicitly and still check syntax around changed files.
- For behavioral changes, verify route names, request payload shape, `returnData.Method`, success/failure `Code`, and DB table enum mapping.

## Avoid

- Do not introduce a new response envelope for normal JSON APIs.
- Do not move controllers into `OldControllers`.
- Do not rename existing public routes casually.
- Do not expose internal helper methods in Swagger.
- Do not add broad refactors, dependency injection rewrites, or ORM replacements while implementing a narrow API request.
- Do not log or echo DB passwords or full connection strings.
