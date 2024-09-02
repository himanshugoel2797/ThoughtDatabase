namespace ThoughtDatabase.StorageProviders
{
	public interface IThoughtDatasetStorageProvider
	{
		void SaveDataset(ThoughtDataset dataset);
		ThoughtDataset LoadDataset();
		void DeleteDataset(ThoughtDataset dataset);
		IFileStorageProvider CreateFileStorageProvider(string name);
	}
}
