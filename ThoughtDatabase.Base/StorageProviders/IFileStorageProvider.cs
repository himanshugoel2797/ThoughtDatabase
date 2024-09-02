namespace ThoughtDatabase.StorageProviders
{
	//Interface for a file storage provider
	public interface IFileStorageProvider
	{
		//Stores a file in the storage provider, returns a unique ID for the file
		public string StoreFile(byte[] fileData);

		//Retrieves a file from the storage provider using the unique ID
		public byte[] GetFile(string fileId);

		//Deletes a file from the storage provider using the unique ID
		public void DeleteFile(string fileId);

		//Deletes the storage provider and all files stored in it
		public void DeleteProvider();
	}
}
