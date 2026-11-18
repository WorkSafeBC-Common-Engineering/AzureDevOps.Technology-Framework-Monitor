using ProjectData;
using ProjectData.Interfaces;

namespace Storage
{
    public static class StorageFactory
    {
        public static IStorageWriter GetStorageWriter()
        {
            var instance = ServiceProviderAccessor.GetRequiredKeyedService<IStorageWriter>(Settings.Storage);
            instance.Initialize(Settings.Configuration);

            return instance;
        }

        public static IStorageReader GetStorageReader()
        {
            var instance = ServiceProviderAccessor.GetRequiredKeyedService<IStorageReader>(Settings.Storage);
            instance.Initialize(Settings.Configuration);

            return instance;
        }
    }
}
