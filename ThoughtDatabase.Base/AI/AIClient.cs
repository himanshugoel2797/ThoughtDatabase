using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThoughtDatabase.AI
{
	//Singleton class for interacting with all registered AI services
	public static class AIClient
	{
		private static readonly Dictionary<string, IVisionClient> _visionProviders = new();
		private static readonly Dictionary<string, ITextClient> _textProviders = new();
		private static readonly Dictionary<string, IImageGenClient> _imageGenProviders = new();

		public static string DefaultVisionProvider { get; set; } = "";
		public static string DefaultTextProvider { get; set; } = "";
		public static string DefaultImageGenProvider { get; set; } = "";

		public static void RegisterVisionProvider(IVisionClient provider)
		{
			if (_visionProviders.ContainsKey(provider.Name))
			{
				throw new InvalidOperationException("Provider with the same name already exists");
			}
			_visionProviders[provider.Name] = provider;
			if (DefaultVisionProvider == null)
			{
				DefaultVisionProvider = provider.Name;
			}
		}

		public static void RegisterTextProvider(ITextClient provider)
		{
			if (_textProviders.ContainsKey(provider.Name))
			{
				throw new InvalidOperationException("Provider with the same name already exists");
			}
			_textProviders[provider.Name] = provider;
			if (DefaultTextProvider == null)
			{
				DefaultTextProvider = provider.Name;
			}
		}

		public static void RegisterImageGenProvider(IImageGenClient provider)
		{
			if (_imageGenProviders.ContainsKey(provider.Name))
			{
				throw new InvalidOperationException("Provider with the same name already exists");
			}
			_imageGenProviders[provider.Name] = provider;
			if (DefaultImageGenProvider == null)
			{
				DefaultImageGenProvider = provider.Name;
			}
		}

		public static IVisionClient GetVisionProvider(string name)
		{
			if (_visionProviders.ContainsKey(name))
			{
				return _visionProviders[name];
			}
			throw new InvalidOperationException("Provider not found");
		}

		public static ITextClient GetTextProvider(string name)
		{
			if (_textProviders.ContainsKey(name))
			{
				return _textProviders[name];
			}
			throw new InvalidOperationException("Provider not found");
		}

		public static IImageGenClient GetImageGenProvider(string name)
		{
			if (_imageGenProviders.ContainsKey(name))
			{
				return _imageGenProviders[name];
			}
			throw new InvalidOperationException("Provider not found");
		}

		public static IVisionClient GetDefaultVisionProvider()
		{
			return GetVisionProvider(DefaultVisionProvider);
		}

		public static ITextClient GetDefaultTextProvider()
		{
			return GetTextProvider(DefaultTextProvider);
		}

		public static IImageGenClient GetDefaultImageGenProvider()
		{
			return GetImageGenProvider(DefaultImageGenProvider);
		}
	}
}
