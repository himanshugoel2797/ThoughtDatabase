using System.Net.Http.Json;
using System.Text.Json;
using ThoughtDatabase.REST;

namespace ThoughtDatabase.Web
{
	public class SessionManagerService
	{
		public string? Username { get; private set; } = "User";
		public bool IsAdmin { get; private set; }
		public bool IsAuthenticated { get; private set; } = false;
		public string? AuthToken { get; private set; }
		public string BaseUrl { get; set; }

		private readonly HttpClient _httpClient;

		public SessionManagerService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<bool> AuthenticateToken(string token)
		{
			if (IsAuthenticated) return true;

			AuthToken = token;
			//if (string.IsNullOrEmpty(AuthToken)) AuthToken = _localStorageService.GetItem<string>("authToken");
			if (string.IsNullOrEmpty(AuthToken)) return false;
			var request = new AuthRequired(AuthToken);
			try
			{
				var user = await _httpClient.GetFromJsonAsync<UserData>($"{BaseUrl}/api/user/get_user?token={AuthToken}");
				if (user == null) return false;
				IsAuthenticated = true;
				Username = user.Username;
				IsAdmin = user.IsAdmin;
				return true;
			}
			catch (HttpRequestException e)
			{
				IsAuthenticated = false;
				return false;
			}
		}

		public async Task<bool> Login(string username, string password, bool rememberMe)
		{
			var request = new UserRequest(username, password, rememberMe);
			var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/user/login", request);
			if (!response.IsSuccessStatusCode)
			{
				IsAuthenticated = false;
				return false;
			}
			AuthToken = response.Content.ReadAsStringAsync().Result;
			bool result = await AuthenticateToken(AuthToken);
			if (!result)
			{
				AuthToken = null;
			}
			return result;
		}

		public async Task Logout()
		{
			if (string.IsNullOrEmpty(AuthToken)) return;
			if (IsAuthenticated)
			{
				var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/user/logout", new AuthRequired(AuthToken));
				if (response.IsSuccessStatusCode)
				{
					AuthToken = null;
					IsAuthenticated = false;
					Username = "User";
					IsAdmin = false;
				}
			}
		}

		public async Task<HttpResponseMessage?> Register(string username, string password, bool isAdmin)
		{
			var request = new UserRequest(username, password, false);
			var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/user/register", request);
			if (isAdmin && response.IsSuccessStatusCode && this.IsAdmin)
			{
				var adminStatus = new AdminStatus(AuthToken, true, username);
				response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/user/set_admin", adminStatus);
			}
			return response;
		}

		public async Task<string[]> GetDatasetNames()
		{
			var response = await _httpClient.GetFromJsonAsync<string[]>($"{BaseUrl}/api/user/get_datasets?token={AuthToken}");
			return response ?? Array.Empty<string>();
		}

		public async Task<bool> CreateDataset(string name, string description)
		{
			var request = new ThoughtDatasetCreateInfo(AuthToken, name, description);
			var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/dataset/create", request);
			return response.IsSuccessStatusCode;
		}
	}
}
