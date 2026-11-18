using Microsoft.Data.SqlClient;

namespace TfmExport
{
    public class Exporter
    {
        #region Private Members

        private StreamWriter? file;
        private SqlConnection? db;

        private StreamWriter Output => file ?? throw new InvalidOperationException("Exporter has not been initialized.");
        private SqlConnection Connection => db ?? throw new InvalidOperationException("Exporter has not been initialized.");

        private const int blockSize = 50;

        #endregion

        #region Public Methods

        public void Initialize(string outputFile, string dbConnection)
        {
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            file = File.CreateText(outputFile);

            db = new SqlConnection(dbConnection);
            db.Open();
        }

        public void Clear(List<TfmTable> tables)
        {
            Output.WriteLine("-- Clearing Tables");
            foreach (var table in tables)
            {
                Console.WriteLine($"Clearing {table.Name}...");
                Truncate(table);
            }

            Output.WriteLine("GO");
            Output.WriteLine();
            Console.WriteLine();
        }

        public void Run(List<TfmTable> tables)
        {
            Output.WriteLine("-- Exporting Tables");

            foreach (var table in tables)
            {
                Delete(table);
            }

            Output.WriteLine();

            var reverseTables = tables.ToArray().Reverse();
            foreach (var table in reverseTables)
            {
                Console.WriteLine($"Exporting {table.Name}...");

                if (table.HasIdentity)
                {
                    IdentityInsert(table, true);
                }

                GetColumns(table);

                Insert(table);

                if (table.HasIdentity)
                {
                    IdentityInsert(table, false);
                }

                Output.WriteLine();
            }
            Console.WriteLine();
        }

        public void Close()
        {
            Connection.Close();

            Output.Flush();
            Output.Close();
        }

        #endregion

        #region Private Methods

        private void Truncate(TfmTable table)
        {
            Output.WriteLine($"TRUNCATE TABLE {table.Name};");
        }

        private void Delete(TfmTable table)
        {
            Output.WriteLine($"DELETE FROM {table.Name};\nGO");
        }

        private void IdentityInsert(TfmTable table, bool on)
        {
            Output.WriteLine($"SET IDENTITY_INSERT {table.Name} {(on ? "ON" : "OFF")};\nGO");
        }

        private void GetColumns(TfmTable table)
        {
            var command = $"SELECT c.name AS COLUMN_NAME, t.name AS DATA_TYPE FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id WHERE c.object_id = OBJECT_ID('dbo.{table.Name}');";

            using SqlCommand sqlCommand = new(command, Connection);
            using SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                Output.WriteLine($"-- Columns for {table.Name}");
                while (reader.Read())
                {
                    var columnName = reader.GetString(0);
                    var dataType = reader.GetString(1);
                    table.Columns.Add(new TableColumn { Name = columnName, UseQuotes = !IsNumeric(dataType) });
                }
            }
        }

        private void Insert(TfmTable table)
        {
            var columns = string.Join(", ", table.Columns.Select(c => c.Name));
            var command = $"SELECT {columns} FROM {table.Name}";
            using SqlCommand sqlCommand = new(command, Connection);
            using SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                Output.WriteLine($"-- Inserting data into {table.Name}");
                var currentRow = 0;
                while (reader.Read())
                {
                    if (++currentRow % blockSize == 0)
                    {
                        Output.WriteLine("GO");
                    }

                    var values = new List<string>();
                    string value;
                    foreach (var column in table.Columns)
                    {
                        var index = reader.GetOrdinal(column.Name);

                        if (reader.IsDBNull(index))
                        {
                            value = "null";
                        }
                        else
                        {
                            value = reader[index].ToString() ?? string.Empty;
                            if (column.UseQuotes)
                            {
                                value = $"'{value.Replace("'", "''" )}'";
                            }
                        }

                        values.Add(value);
                    }
                    var row = string.Join(", ", values);
                    Output.WriteLine($"INSERT INTO {table.Name} ({columns}) VALUES ({row});");
                }

                Output.WriteLine("GO");
            }
        }

        private static bool IsNumeric(string dataType)
        {
            return dataType.Contains("int") || dataType.Contains("decimal") || dataType.Contains("float");
        }

        #endregion
    }
}
