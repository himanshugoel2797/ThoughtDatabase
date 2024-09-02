using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtDatabase.AI;

namespace ThoughtDatabase
{
	//Collection of thoughts, associated files and ai client names
	[Serializable]
	public class ThoughtCollection
	{
		private static readonly HashSet<ThoughtType> thoughtTypeWithFile = new HashSet<ThoughtType> { ThoughtType.Audio, ThoughtType.Video, ThoughtType.Image, ThoughtType.File };

		public HashSet<Thought> Thoughts { get; set; } = new();
		public FileCollection FileCollection { get; set; }

		public List<string> KeywordClients { get; set; } = new();
		public List<string> ImageKeywordClients { get; set; } = new();
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";

		public ThoughtCollection(string name, FileCollection fileCollection)
		{
			Name = name;
			FileCollection = fileCollection;
		}

		//Adds a thought to the collection
		public void AddThought(string content, ThoughtType type, string source, SourceType sourceType, DateTime dateCreated, string[]? keywords = null, KeyValuePair<string, string>[]? keyValuePairs = null, byte[]? fileData = null)
		{
			if (thoughtTypeWithFile.Contains(type) && (fileData == null || fileData.Length == 0))
			{
				throw new InvalidOperationException("File data is required for this thought type");
			}

			if (thoughtTypeWithFile.Contains(type) && fileData != null && fileData.Length > 0)
			{
				content = FileCollection.AddFile(fileData);
			}

			//Analyze the thought content and add keywords
			List<string> keywords_full = new();
			List<KeyValuePair<string, string>> metadata = new();
			if (keywords != null)
				keywords_full.AddRange(keywords);
			if (keyValuePairs != null)
				metadata.AddRange(keyValuePairs);
			switch (type)
			{
				case ThoughtType.Text:
				case ThoughtType.Email:
				case ThoughtType.CalendarEvent:
				case ThoughtType.Location:
				case ThoughtType.Contact:
				case ThoughtType.Task:
					keywords_full.AddRange(KeywordClients.SelectMany(client =>
					{
						var provider = AIClient.GetTextProvider(client);
						provider.AnalyzeText(content);
						return provider.GetResults();
					}
					));

					metadata.AddRange(KeywordClients.SelectMany(client =>
					{
						var provider = AIClient.GetTextProvider(client);
						provider.AnalyzeText(content);
						return provider.GetKeyValues();
					}
					));

					break;
				case ThoughtType.Image:
					keywords_full.AddRange(ImageKeywordClients.SelectMany(client =>
					{
						var provider = AIClient.GetVisionProvider(client);
						provider.AnalyzeImage(fileData);
						return provider.GetResults();
					}
					));

					metadata.AddRange(ImageKeywordClients.SelectMany(client =>
					{
						var provider = AIClient.GetVisionProvider(client);
						provider.AnalyzeImage(fileData);
						return provider.GetKeyValues();
					}
					));
					break;
			}

			Thoughts.Add(new Thought(content, type, source, sourceType, dateCreated, keywords_full.ToArray(), metadata.ToArray()));
		}

		public void UpdateThought(DateTime dateCreated, string? content = null, string[]? keywords = null, KeyValuePair<string, string>[]? metadata = null, Thought? originalThought = null)
		{
			if (originalThought == null)
			{
				throw new InvalidOperationException("Original thought is required for updating a thought");
			}

			var updatedThought = originalThought.Update(dateCreated, content, keywords, metadata);
			Thoughts.Remove(originalThought);
			Thoughts.Add(updatedThought);
		}

		public void DeleteThought(Thought thought)
		{
			Thoughts.Remove(thought);
		}

		public IEnumerable<Thought> GetThoughts()
		{
			return Thoughts;
		}

		public void Destroy()
		{
			Thoughts.Clear();
			FileCollection.Destroy();
		}
	}
}
