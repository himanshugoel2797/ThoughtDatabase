using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThoughtDatabase.StorageProviders
{
	public class FileSystemStorageProvider : IFileStorageProvider, IFileSystemStorageProviderFactory
	{
		public string DirectoryPath { get; init; }
		public FileSystemStorageProvider(string directoryPath)
		{
			DirectoryPath = directoryPath;
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
		}

		public void DeleteFile(string fileId)
		{
			string path = string.Join(Path.DirectorySeparatorChar, DirectoryPath, fileId);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public byte[] GetFile(string fileId)
		{
			string path = string.Join(Path.DirectorySeparatorChar, DirectoryPath, fileId);
			if (File.Exists(path))
			{
				return File.ReadAllBytes(path);
			}
			return Array.Empty<byte>();
		}

		public string StoreFile(byte[] fileData)
		{
			string fileId = Guid.NewGuid().ToString();
			string path = string.Join(Path.DirectorySeparatorChar, DirectoryPath, fileId);
			File.WriteAllBytes(path, fileData);
			return fileId;
		}

		public static IFileStorageProvider Create(string directoryPath)
		{
			return new FileSystemStorageProvider(directoryPath);
		}

		public void DeleteProvider()
		{
			Directory.Delete(DirectoryPath, true);
		}
	}
}
