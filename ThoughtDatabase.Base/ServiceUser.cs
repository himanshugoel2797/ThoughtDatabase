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
		private ReaderWriterLockSlim _lock = new();

		public ServiceUser(string username, string password)
		{
			DateOfRegistration = DateTime.Now;
			Username = username;
			Password = password;
		}

		public void AddDataset(string name, string description = "")
		{
			_lock.EnterWriteLock();
			try
			{
				JsonDatasetStorageProvider jsonDatasetStorageProvider = new JsonDatasetStorageProvider(name);
				var dataset = new ThoughtDataset(jsonDatasetStorageProvider)
				{
					Name = name,
					Description = description
				};
				Datasets.Add(dataset);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void RemoveDataset(ThoughtDataset dataset)
		{
			_lock.EnterWriteLock();
			try
			{
				Datasets.Remove(dataset);
				dataset.Destroy();
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public string GenerateToken(bool tmp)
		{
			_lock.EnterWriteLock();
			try
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
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void RevokeToken(string token)
		{
			_lock.EnterWriteLock();
			try
			{
				AuthTokens.Remove(token);
				_tmpTokens.Remove(token);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public bool ValidateToken(string token)
		{
			_lock.EnterReadLock();
			try
			{
				return AuthTokens.Contains(token) || _tmpTokens.Contains(token);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		public void Destroy()
		{
			_lock.EnterWriteLock();
			try
			{
				foreach (var dataset in Datasets)
				{
					dataset.Destroy();
				}
				Datasets.Clear();
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}
	}
}
