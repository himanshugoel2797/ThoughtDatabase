using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtDatabase.StorageProviders;

namespace ThoughtDatabase
{
	//Stores files in a storage provider associated with this collection, assign a unique ID to each file
	[Serializable]
	public class FileCollection
	{
		private readonly IFileStorageProvider _fileStorageProvider;
		private readonly Dictionary<string, string> _fileIdMap = new();
		private readonly Dictionary<string, string> _fileHashes = new();

		public FileCollection(IFileStorageProvider fileStorageProvider)
		{
			_fileStorageProvider = fileStorageProvider;
		}

		//Stores a file in the storage provider and returns a unique ID for the file
		public string AddFile(byte[] fileData)
		{
			//Generate an MD5 hash of the file data
			byte[] hash;
			using (var md5 = System.Security.Cryptography.MD5.Create())
			{
				hash = md5.ComputeHash(fileData);
			}
			string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
			if (_fileHashes.ContainsKey(hashString))
			{
				return _fileHashes[hashString];
			}

			string fileId = _fileStorageProvider.StoreFile(fileData);

			Guid fileIdGuid = Guid.NewGuid();
			string fileId_local = fileIdGuid.ToString();
			_fileHashes[hashString] = fileId_local;
			_fileIdMap[fileId_local] = fileId;
			return fileId_local;
		}

		//Retrieves a file from the storage provider using the unique ID
		public byte[] GetFile(string fileId)
		{
			if (_fileIdMap.ContainsKey(fileId))
			{
				return _fileStorageProvider.GetFile(_fileIdMap[fileId]);
			}
			return Array.Empty<byte>();
		}

		//Deletes a file from the storage provider using the unique ID
		public void DeleteFile(string fileId)
		{
			if (_fileIdMap.ContainsKey(fileId))
			{
				_fileStorageProvider.DeleteFile(_fileIdMap[fileId]);
				_fileIdMap.Remove(fileId);

				//Remove the hash entry
				string hash = _fileHashes.FirstOrDefault(x => x.Value == fileId).Key;
				if (hash != null)
				{
					_fileHashes.Remove(hash);
				}
			}
		}

		//Checks if a file exists in the storage provider using the unique ID
		public bool FileExists(string fileId)
		{
			return _fileIdMap.ContainsKey(fileId);
		}

		public void Destroy()
		{
			_fileStorageProvider.DeleteProvider();
			_fileHashes.Clear();
			_fileIdMap.Clear();
		}
	}
}
