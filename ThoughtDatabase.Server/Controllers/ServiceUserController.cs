using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThoughtDatabase;
using ThoughtDatabase.REST;
using ThoughtDatabase.StorageProviders;

namespace ThoughtDatabase.Server.Controllers
{

    [ApiController]
    [Route("/api/user/")]
    public class ServiceUserController : ControllerBase
    {
        public ServiceUserController()
        {

        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] UserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password must be provided");
            }
            if (ServiceUserManager.Instance.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest("User already exists");
            }

            ServiceUserManager.Instance.RegisterUser(request.Username, request.Password);
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult AuthenticateUser([FromBody] UserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password must be provided");
            }

            var token = ServiceUserManager.Instance.AuthenticateUser(request.Username, request.Password, request.RememberMe);
            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        [HttpPost("logout")]
        public IActionResult LogoutUser([FromBody] AuthRequired Token)
        {
            if (string.IsNullOrWhiteSpace(Token.Token))
            {
                return BadRequest("Auth token must be provided");
            }

            if (!ServiceUserManager.Instance.AuthenticateToken(Token.Token))
            {
                return Unauthorized();
            }

            ServiceUserManager.Instance.RevokeToken(Token.Token);
            return Ok();
        }

        [HttpPost("delete_user")]
        public IActionResult DeleteUser([FromBody] DeleteUserRequest request)
        {
            var authToken = request.Token;
            if (string.IsNullOrWhiteSpace(authToken))
            {
                return BadRequest("Auth token must be provided");
            }
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Username must be provided");
            }

            if (!ServiceUserManager.Instance.AuthenticateToken(authToken))
            {
                return Unauthorized();
            }

            var result = ServiceUserManager.Instance.DeleteUser(request.Username, authToken);
            if (!result)
            {
                return BadRequest("Failed to delete user");
            }
            return Ok();
        }

        [HttpPost("set_admin")]
        public IActionResult SetAdminStatus([FromBody] AdminStatus request)
        {
            var authToken = request.Token;
			if (string.IsNullOrWhiteSpace(authToken))
            {
                return BadRequest("Auth token must be provided");
            }
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Username must be provided");
            }

            if (!ServiceUserManager.Instance.AuthenticateToken(authToken))
            {
                return Unauthorized();
            }

            var result = ServiceUserManager.Instance.SetAdminStatus(request.Username, request.IsAdmin, authToken);
            if (!result)
            {
                return BadRequest("Failed to set admin status");
            }
            return Ok();
        }

        [HttpGet("get_user")]
		public IActionResult GetUser([FromQuery] string token)
		{
			var authToken = token;
            if (string.IsNullOrWhiteSpace(authToken))
			{
				return BadRequest("Auth token must be provided");
			}

			if (!ServiceUserManager.Instance.AuthenticateToken(authToken))
			{
				return Unauthorized();
			}

			var user = ServiceUserManager.Instance.GetUser(authToken);
			if (user == null)
			{
				return BadRequest("Failed to get user");
			}
            UserData ud = new UserData(user.Username, user.IsAdmin, user.DateOfRegistration);
			return Ok(ud);
		}

        [HttpGet("get_datasets")]
		public IActionResult GetDatasets([FromQuery] ThoughtDatasetListRequest request)
		{
            var authToken = request.Token;
			if (string.IsNullOrWhiteSpace(authToken))
			{
				return BadRequest("Auth token must be provided");
			}

			if (!ServiceUserManager.Instance.AuthenticateToken(authToken))
			{
				return Unauthorized();
			}

            var user = ServiceUserManager.Instance.GetUser(authToken);
			if (user == null)
            {
				return BadRequest("Failed to get user");
			}

            int count = request.Count ?? user.Datasets.Count;
            if (count > user.Datasets.Count) count = user.Datasets.Count;
			var datasets = user.Datasets.GetRange(request.Skip, count).Select(a => a.Name).ToArray();
			if (datasets == null)
			{
				return BadRequest("Failed to get datasets");
			}
			return Ok(datasets);
		}

        [HttpGet("get_dataset_count")]
		public IActionResult GetDatasetCount([FromQuery] string token)
		{
			var authToken = token;
			if (string.IsNullOrWhiteSpace(authToken))
			{
				return BadRequest("Auth token must be provided");
			}

			if (!ServiceUserManager.Instance.AuthenticateToken(authToken))
			{
				return Unauthorized();
			}

			var user = ServiceUserManager.Instance.GetUser(authToken);
			if (user == null)
			{
				return BadRequest("Failed to get user");
			}

			return Ok(user.Datasets.Count);
		}
	}
}
