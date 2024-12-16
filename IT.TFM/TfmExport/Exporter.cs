using Microsoft.Data.SqlClient;

namespace TfmExport
{
    public class Exporter
    {
        #region Private Members

        private StreamWriter file;
        
        private SqlConnection db;

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
            file.WriteLine("-- Clearing Tables");
            foreach (var table in tables)
            {
                Console.WriteLine($"Clearing {table.Name}...");
                Truncate(table);
            }

            file.WriteLine("GO");
            file.WriteLine();
            Console.WriteLine();
        }

        public void Run(List<TfmTable> tables)
        {
            file.WriteLine("-- Exporting Tables");

            foreach (var table in tables)
            {
                Delete(table);
            }

            file.WriteLine();

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

                file.WriteLine();
            }
            Console.WriteLine();
        }

        public void Close()
        {
            db.Close();

            file.Flush();
            file.Close();
        }

        #endregion

        #region Private Methods

        private void Truncate(TfmTable table)
        {
            file.WriteLine($"TRUNCATE TABLE {table.Name};");
        }

        private void Delete(TfmTable table)
        {
            file.WriteLine($"DELETE FROM {table.Name};\nGO");
        }

        private void IdentityInsert(TfmTable table, bool on)
        {
            file.WriteLine($"SET IDENTITY_INSERT {table.Name} {(on ? "ON" : "OFF")};\nGO");
        }

        private void GetColumns(TfmTable table)
        {
            var command = $"SELECT c.name AS COLUMN_NAME, t.name AS DATA_TYPE FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id WHERE c.object_id = OBJECT_ID('dbo.{table.Name}');";

            using SqlCommand sqlCommand = new(command, db);
            using SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                file.WriteLine($"-- Columns for {table.Name}");
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
            using SqlCommand sqlCommand = new(command, db);
            using SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                file.WriteLine($"-- Inserting data into {table.Name}");
                var currentRow = 0;
                while (reader.Read())
                {
                    if (++currentRow % blockSize == 0)
                    {
                        file.WriteLine("GO");
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
                    file.WriteLine($"INSERT INTO {table.Name} ({columns}) VALUES ({row});");
                }

                file.WriteLine("GO");
            }
        }

        private static bool IsNumeric(string dataType)
        {
            return dataType.Contains("int") || dataType.Contains("decimal") || dataType.Contains("float");
        }

        #endregion
    }
}
