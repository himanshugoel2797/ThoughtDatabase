using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThoughtDatabase.Web
{
	public class ServiceCollectionManager
	{
		public static void RegisterServices(IServiceCollection services)
		{
			services.AddSingleton<SessionManagerService>();
		}
	}
}
