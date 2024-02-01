using System.Data;
using System.Data.SqlClient;

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
            throw new ArgumentException("For cleanup, you need to specify a second int parameter indicating number of days to maintain. Ex: c 30");
        }
        Cleanup(connectionString, value);
        return;

    default: throw new ArgumentException("Invalid argument given, must be either u for update, or c for cleanup");
}

void Update(string connectionString)
{
    using SqlConnection connection = new(connectionString);
    var command = new SqlCommand("dbo.UpdateReports", connection)
    {
        CommandType = CommandType.StoredProcedure,
        CommandTimeout = 60
    };

    command.Connection.Open();
    command.ExecuteNonQuery();
}

void Cleanup(string connectionString, int days)
{
    using SqlConnection connection = new(connectionString);
    var command = new SqlCommand("dbo.CleanupReports", connection)
    {
        CommandType = CommandType.StoredProcedure,
        CommandTimeout = 60
    };
    var daysParameter = command.Parameters.Add("@days", SqlDbType.Int);
    daysParameter.Value = days;

    command.Connection.Open();
    command.ExecuteNonQuery();
}