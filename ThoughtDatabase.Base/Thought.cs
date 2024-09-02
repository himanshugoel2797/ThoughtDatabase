using System.Text.Json.Serialization;

namespace ThoughtDatabase
{
	//A thought contains immutable arbitrary data (a URL, an image, text, raw files, calendar events, emails, videos, etc.)
	//It also contains metadata (date created, tags, source etc.)
	//An LLM extracts keywords from the thought
	//Connections can be built between thoughts to form interconnected clusters of related thoughts, connections are determined at a higher level
	//Visualizations can be generated from these clusters
	//Each thought is immutable, but retains history of previous versions
	[Serializable]
	public record class Thought(string Content, ThoughtType Type, string Source, SourceType SourceType, DateTime DateCreated, string[] Keywords, KeyValuePair<string, string>[] Metadata, Thought? PreviousVersion = null)
	{
		public Thought Update(DateTime dateCreated, string? content = null, string[]? keywords = null, KeyValuePair<string, string>[]? metadata = null)
		{
			return new Thought(content ?? Content, Type, Source, SourceType, dateCreated, keywords ?? Keywords, metadata ?? Metadata, this);
		}

		public Thought Revert()
		{
			return PreviousVersion ?? this;
		}

		public override string ToString()
		{
			return $"Thought: {Content} Type: {Type} Source: {Source} SourceType: {SourceType} DateCreated: {DateCreated} Keywords: {string.Join(", ", Keywords)} Metadata: {string.Join(", ", Metadata)}";
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Content, Type, Source, SourceType, DateCreated, Keywords, Metadata);
		}
	}
}
