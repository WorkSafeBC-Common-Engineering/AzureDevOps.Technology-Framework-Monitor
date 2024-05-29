namespace ProjectData.Interfaces
{
    public interface IFilter
    {
        void Initialize(string data);

        string ColumnName { get; }

        bool IsMatch(string data);
    }
}
