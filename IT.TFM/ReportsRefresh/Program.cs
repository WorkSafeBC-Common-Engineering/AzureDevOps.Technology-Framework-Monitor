using System.Data;
using System.Data.SqlClient;

const int sqlExecuteTimeout = 3600;  // 3600 seconds = 30 minutes
const string sqlUpdate = "dbo.UpdateReports";
const string sqlCleanup = "dbo.CleanupReports";
const string sqlEmptyAITable = "TRUNCATE TABLE [dbo].[AIQueryTable]";
const string sqlFillAITable = "EXECUTE [AI].[Refresh]";
const string sqlDaysParameter = "@days";

if (args.Length == 0)
{
    throw new ArgumentException("Need to specify u for update, or c for cleanup as a parameter");
}

var connectionString = ConfidentialSettings.Values.DbConnection;

switch (args[0])
{
    case "u":
        Update(connectionString);
        return;

    case "c":
        if (args.Length < 2 || !int.TryParse(args[1], out var value))
        {
            throw new ArgumentException("For cleanup, you need to specify a second integer parameter indicating number of days to maintain. Ex: c 30");
        }
        Cleanup(connectionString, value);
        return;

    case "ax":
        EmptyAIQuery(connectionString);
        return;

    case "af":
        FillAIQuery(connectionString);
        return;

    default: throw new ArgumentException("Invalid argument given, must be either u for update, c for cleanup, ax for cleanup AI query table, or af to fill the AI query table");
}

void Update(string connectionString)
{
    using SqlConnection connection = new(connectionString);
    var command = new SqlCommand(sqlUpdate, connection)
    {
        CommandType = CommandType.StoredProcedure,
        CommandTimeout = sqlExecuteTimeout
    };

    command.Connection.Open();
    command.ExecuteNonQuery();
}

void Cleanup(string connectionString, int days)
{
    using SqlConnection connection = new(connectionString);
    var command = new SqlCommand(sqlCleanup, connection)
    {
        CommandType = CommandType.StoredProcedure,
        CommandTimeout = sqlExecuteTimeout
    };
    var daysParameter = command.Parameters.Add(sqlDaysParameter, SqlDbType.Int);
    daysParameter.Value = days;

    command.Connection.Open();
    command.ExecuteNonQuery();
}

void EmptyAIQuery(string connectionString)
{
    using SqlConnection connection = new(connectionString);
    var command = new SqlCommand(sqlEmptyAITable, connection)
    {
        CommandType = CommandType.Text,
        CommandTimeout = sqlExecuteTimeout
    };

    command.Connection.Open();
    command.ExecuteNonQuery();
}

void FillAIQuery(string connectionString)
{
    using SqlConnection connection = new(connectionString);
    var command = new SqlCommand(sqlFillAITable, connection)
    {
        CommandType = CommandType.Text,
        CommandTimeout = sqlExecuteTimeout
    };

    command.Connection.Open();
    command.ExecuteNonQuery();
}