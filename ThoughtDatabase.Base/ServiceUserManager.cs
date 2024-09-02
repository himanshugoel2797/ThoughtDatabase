using System.Collections.Concurrent;
using System.Text.Json;

namespace ThoughtDatabase
{
	public class ServiceUserManager
	{
		public static ServiceUserManager Instance { get; private set; } = new();

		public static void Load(string path)
		{
			if (File.Exists(path))
			{
				var inst = JsonSerializer.Deserialize<ServiceUserManager>(File.ReadAllText(path));
				if (inst != null)
				{
					Instance = inst;
				}
				else
				{
					throw new Exception("Failed to load user data");
				}
			}
			else
			{
				Instance = new ServiceUserManager();
			}
			Instance.SaveFile = path;
			Instance.Save(path);
		}

		public void Save(string path)
		{
			//Pretty print the json
			JsonSerializerOptions options = new() { WriteIndented = true };
			using (FileStream fs = new FileStream(path, FileMode.Create))
			{
				JsonSerializer.Serialize(fs, this, options);
			}
		}

		public void Save()
		{
			Save(SaveFile);
		}

		public string SaveFile { get; set; } = "users.json";
		public List<ServiceUser> Users { get; set; } = new();
		public ServiceUserManager() { }

		private ConcurrentDictionary<string, string> _authdTokens = new();
		private ReaderWriterLockSlim _lock = new();

		public void RegisterUser(string username, string password)
		{
			_lock.EnterWriteLock();
			try
			{
				Users.Add(new ServiceUser(username, password) { IsAdmin = Users.Count == 0 });
				Save();
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public string? AuthenticateUser(string username, string password, bool RememberMe)
		{
			_lock.EnterReadLock();
			try
			{
				foreach (var user in Users)
				{
					if (user.Username == username && user.Password == password)
					{
						string token = user.GenerateToken(!RememberMe);
						_authdTokens[token] = username;
						Save();
						return token;
					}
				}
			}
			finally
			{
				_lock.ExitReadLock();
			}
			return null;
		}

		public bool AuthenticateToken(string token)
		{
			_lock.EnterReadLock();
			try
			{
				if (_authdTokens.ContainsKey(token))
				{
					return true;
				}
				foreach (var user in Users)
				{
					if (user.ValidateToken(token))
					{
						_authdTokens[token] = user.Username;
						return true;
					}
				}
			}
			finally
			{
				_lock.ExitReadLock();
			}
			return false;
		}

		public void RevokeToken(string token)
		{
			_lock.EnterWriteLock();
			try
			{
				if (_authdTokens.ContainsKey(token))
				{
					var username = _authdTokens[token];
					var user = Users.First(u => u.Username == username);
					user.RevokeToken(token);
					_authdTokens.Remove(token, out _);

					Save();
					return;
				}

				foreach (var user in Users)
				{
					user.RevokeToken(token);
				}
				Save();
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public ServiceUser? GetUser(string token)
		{
			_lock.EnterReadLock();
			try
			{
				if (_authdTokens.ContainsKey(token))
				{
					return Users.First(u => u.Username == _authdTokens[token]);
				}
			}
			finally
			{
				_lock.ExitReadLock();
			}
			return null;
		}

		public bool DeleteUser(string username, string token)
		{
			_lock.EnterWriteLock();
			try
			{
				if (_authdTokens.ContainsKey(token) && (Users.First(u => u.Username == _authdTokens[token]).IsAdmin || _authdTokens[token] == username))
				{
					var user = Users.First(u => u.Username == username);
					Users.Remove(user);
					user.Destroy();
					Save();
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public bool SetAdminStatus(string username, bool isAdmin, string token)
		{
			_lock.EnterWriteLock();
			try
			{
				if (_authdTokens.ContainsKey(token) && Users.First(u => u.Username == _authdTokens[token]).IsAdmin)
				{
					var user = Users.First(u => u.Username == username);
					if (!isAdmin && user.IsAdmin && Users.Count(u => u.IsAdmin) == 1)
					{
						throw new Exception("Cannot remove the last admin");
					}
					user.IsAdmin = isAdmin;
					Save();
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}
	}
}
