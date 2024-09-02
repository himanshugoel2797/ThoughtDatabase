namespace ThoughtDatabase.Server
{
    public class ConfigOptions
    {
        public static ConfigOptions Config { get; private set; }

        private ConfigOptions()
        {
        }

        public static void Initialize(IConfiguration configuration)
        {
            Config = new ConfigOptions();
            Config.UserDataLocation = configuration.GetValue("UserDataLocation", Config.UserDataLocation) ?? Config.UserDataLocation;
		}

        public string UserDataLocation { get; set; } = "users.json";
    }
}
