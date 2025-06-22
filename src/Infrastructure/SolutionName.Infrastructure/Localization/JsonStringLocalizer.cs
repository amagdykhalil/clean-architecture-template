using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolutionName.Shared.Keys;
using System.Reflection;

namespace SolutionName.Infrastructure.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private const string RESOURCE_BASE_PATH = "SolutionName.Shared.Resources";
        private readonly IDistributedCache _cache;
        private readonly JsonSerializer _serializer = new();
        private readonly Assembly _resourcesAssembly;

        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
            _resourcesAssembly = typeof(LocalizationKeys).Assembly;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, arguments))
                    : actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            //var resourceName = GetResourceName(Thread.CurrentThread.CurrentCulture.Name);
            //using var stream = _resourcesAssembly.GetManifestResourceStream(resourceName);
            //if (stream == null) yield break;

            //using var streamReader = new StreamReader(stream);
            //using var reader = new JsonTextReader(streamReader);

            //while (reader.Read())
            //{
            //    if (reader.TokenType != JsonToken.PropertyName)
            //        continue;

            //    var key = reader.Value as string;
            //    reader.Read();
            //    var value = _serializer.Deserialize<string>(reader);
            //    yield return new LocalizedString(key, value);
            //}
            throw new NotImplementedException();
        }

        private string GetString(string key)
        {
            // Expecting key in format "fileName:nested.key.path"
            var split = key.Split(':', 2);
            if (split.Length != 2)
                return string.Empty;

            var fileName = split[0];
            var propertyPath = split[1];

            var culture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            var cacheKey = $"locale_{culture}_{fileName}_{propertyPath}";
            var cacheValue = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheValue))
                return cacheValue;

            var result = GetValueFromJSON(propertyPath, fileName, culture);

            if (!string.IsNullOrEmpty(result))
                _cache.SetString(cacheKey, result);

            return result ?? key;
        }

        private string GetValueFromJSON(string propertyPath, string fileName, string culture)
        {
            var resourceName = GetResourceName(culture, fileName);
            using var stream = _resourcesAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return string.Empty;

            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var jObject = JToken.ReadFrom(jsonReader);

            // Use JSONPath to directly select nested tokens
            var token = jObject.SelectToken(propertyPath);
            if (token == null)
                return string.Empty;

            // Return the raw string or JSON as needed
            if (token.Type == JTokenType.String || token.Type == JTokenType.Null)
                return token.ToString();

            return token.ToString(Formatting.None);
        }

        private static string GetResourceName(string culture, string fileName)
            => $"{RESOURCE_BASE_PATH}.{culture}.{fileName}.json";
    }
}
