namespace ThoughtDatabase.AI
{
	public interface IConfigurableModel
	{
		ModelSetting[] ModelSettings { get; }
		void SetModelSetting(ModelSetting setting);
		void SetModelSettings(ModelSetting[] settings);
	}
}
