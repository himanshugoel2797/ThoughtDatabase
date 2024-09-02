using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtDatabase.StorageProviders;

namespace ThoughtDatabase
{

	//Collection of thoughtcollections associated with one user
	[Serializable]
	public class ThoughtDataset
	{
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public DateTime Date { get; set; }

		public JsonDatasetStorageProvider StorageProvider { get; set; }

		public List<ThoughtCollection> Collections { get; set; } = new();

		public ThoughtDataset(JsonDatasetStorageProvider storageProvider)
		{
			StorageProvider = storageProvider;
			Date = DateTime.Now;
		}

		public void AddCollection(string name)
		{
			var fileCollection = new FileCollection(StorageProvider.CreateFileStorageProvider(name));
			var collection = new ThoughtCollection(name, fileCollection);
			Collections.Add(collection);
		}

		public void RemoveCollection(ThoughtCollection collection)
		{
			Collections.Remove(collection);
			collection.Destroy();
		}

		public void Destroy()
		{
			foreach (var collection in Collections)
			{
				collection.Destroy();
			}
			StorageProvider.DeleteDataset(this);
		}
	}
}
