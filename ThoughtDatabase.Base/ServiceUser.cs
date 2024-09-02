using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtDatabase.StorageProviders;

namespace ThoughtDatabase
{

	public class ServiceUser
	{
		public string Username { get; set; } = "";
		public string Password { get; set; } = "";
		public bool IsAdmin { get; set; } = false;
		public DateTime DateOfRegistration { get; set; }
		public List<ThoughtDataset> Datasets { get; set; } = new();
		public List<string> AuthTokens { get; set; } = new();
		private List<string> _tmpTokens = new();

		public ServiceUser(string username, string password)
		{
			DateOfRegistration = DateTime.Now;
			Username = username;
			Password = password;
		}

		public void AddDataset(string name)
		{
			JsonDatasetStorageProvider<FileSystemStorageProvider> jsonDatasetStorageProvider = new JsonDatasetStorageProvider<FileSystemStorageProvider>(name);
			var dataset = new ThoughtDataset(jsonDatasetStorageProvider);
			Datasets.Add(dataset);
		}

		public void RemoveDataset(ThoughtDataset dataset)
		{
			Datasets.Remove(dataset);
			dataset.Destroy();
		}

		public string GenerateToken(bool tmp)
		{
			var token = Guid.NewGuid().ToString();
			if (tmp)
			{
				_tmpTokens.Add(token);
			}
			else
			{
				AuthTokens.Add(token);
			}
			return token;
		}

		public void RevokeToken(string token)
		{
			AuthTokens.Remove(token);
			_tmpTokens.Remove(token);
		}

		public bool ValidateToken(string token)
		{
			return AuthTokens.Contains(token) || _tmpTokens.Contains(token);
		}

		public void Destroy()
		{
			foreach (var dataset in Datasets)
			{
				dataset.Destroy();
			}
		}
	}
}
