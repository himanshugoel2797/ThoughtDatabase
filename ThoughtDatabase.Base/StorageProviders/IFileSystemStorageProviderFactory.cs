namespace ThoughtDatabase.StorageProviders
{
	public interface IFileSystemStorageProviderFactory
	{
		static abstract IFileStorageProvider Create(string directoryPath);
	}
}
