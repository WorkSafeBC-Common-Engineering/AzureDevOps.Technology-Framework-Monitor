namespace TfmExport
{
    public class TableColumn
    {
        public string Name { get; set; } = string.Empty;

        public bool UseQuotes { get; set; } = false;

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    public class TfmTable
    {
        public string Name { get; set; } = string.Empty;

        public bool HasIdentity { get; set; } = false;

        public List<TableColumn> Columns { get; set; } = [];

        public override string ToString()
        {
            return $"{Name}: HasIdentity = {HasIdentity}, Columns = {Columns}";
        }
    }
}
