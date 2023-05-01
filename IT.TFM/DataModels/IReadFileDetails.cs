using System.Messaging;

namespace RepoScan.DataModels
{
    public interface IReadFileDetails
    {
        FileDetails Read();

        MessageEnumerator GetEnumerator();

        FileDetails Read(Message message);

        void Clear();
    }
}
