
using System.Text.Json;

namespace ThoughtDatabase.StorageProviders
{
	public class JsonDatasetStorageProvider
	{
		public string BaseDirectory { get; init; }
		public JsonDatasetStorageProvider(string baseDirectory)
		{
			BaseDirectory = baseDirectory;
		}

		public void SaveDataset(ThoughtDataset dataset)
		{
			//Serialize the dataset and all of its objects and save it to the file system as a json file
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				WriteIndented = true
			};
			using (FileStream fs = new FileStream(Path.Combine(BaseDirectory, "dataset.json"), FileMode.Create))
			{
				JsonSerializer.Serialize(fs, dataset, options);
			}
		}

		public ThoughtDataset LoadDataset()
		{
			//Load the dataset from the file system and deserialize it
			using (FileStream fs = new FileStream(Path.Combine(BaseDirectory, "dataset.json"), FileMode.Open))
			{
				var dataset = JsonSerializer.Deserialize<ThoughtDataset>(fs);
				if (dataset != null)
				{
					return dataset;
				}
				else
				{
					throw new Exception("Failed to load dataset");
				}
			}
		}

		public void DeleteDataset(ThoughtDataset dataset)
		{
			//Delete the dataset file
			File.Delete(Path.Combine(BaseDirectory, "dataset.json"));
		}

		public FileSystemStorageProvider CreateFileStorageProvider(string name)
		{
			return new FileSystemStorageProvider(Path.Combine(BaseDirectory, name));
		}
	}
}
