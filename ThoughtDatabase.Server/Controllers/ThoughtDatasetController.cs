using Microsoft.AspNetCore.Mvc;
using ThoughtDatabase.REST;

namespace ThoughtDatabase.Server.Controllers
{

	[ApiController]
	[Route("/api/dataset/")]
	public class ThoughtDatasetController : ControllerBase
	{
		public ThoughtDatasetController()
		{

		}

		[HttpPost("create")]
		public IActionResult CreateDataset([FromBody] ThoughtDatasetCreateInfo request)
		{
			if (string.IsNullOrWhiteSpace(request.Name))
			{
				return BadRequest("Name must be provided");
			}

			if (!ServiceUserManager.Instance.AuthenticateToken(request.Token))
			{
				return Unauthorized();
			}

			var user = ServiceUserManager.Instance.GetUser(request.Token);
			if (user == null)
			{
				return Unauthorized();
			}

			user.AddDataset(request.Name, request.Description ?? "");
			ServiceUserManager.Instance.Save();
			return Ok();
		}
	}
}
