namespace ThoughtDatabase.AI
{
	public interface IVisionClient : IConfigurableModel
	{
		//Interface for a vision client
		string Name { get; }
		VisionClientType Type { get; }

		//Methods for interacting with the vision client
		public void AnalyzeImage(byte[] imageData);
		public string[] GetResults();
		public KeyValuePair<string, string>[] GetKeyValues();
	}
}
