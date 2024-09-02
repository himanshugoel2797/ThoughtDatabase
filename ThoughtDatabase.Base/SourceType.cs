namespace ThoughtDatabase
{
	public enum SourceType
	{
		Unknown = 0,
		ManualEntry, //User entered the thought manually
		Integration, //Thought was created by an integration (e.g. email, calendar, etc.)
		ScreenCapture, //Thought was created by a screen capture
	}
}
