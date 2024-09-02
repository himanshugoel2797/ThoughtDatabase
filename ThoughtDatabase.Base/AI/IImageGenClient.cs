namespace ThoughtDatabase.AI
{
	public interface IImageGenClient : IConfigurableModel
	{
		string Name { get; }
		public void GenerateImage(string text);
	}
}
