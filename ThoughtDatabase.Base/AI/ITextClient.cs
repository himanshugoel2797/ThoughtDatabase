namespace ThoughtDatabase.AI
{
	public interface ITextClient : IConfigurableModel
	{
		//Interface for a text client
		string Name { get; }
		TextClientType Type { get; }
		public void AnalyzeText(string text);
		public string[] GetResults();

		public KeyValuePair<string, string>[] GetKeyValues();
	}
}
