using System.Reflection;

namespace SolutionName.Shared.Keys
{
    /// <summary>
    /// Initializes all LocalizationKeys nested static classes via reflection.
    /// Call Initialize() once at app startup to wire up key values.
    /// </summary>
    public static class ReflectionLocalizationKeyProvider
    {
        private const string FileSeparator = ":";
        private const string KeySeparator = ".";
        private static readonly Dictionary<string, string> _keyMap = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initialize and assign all nested LocalizationKeys.* properties.
        /// </summary>
        public static void Initialize()
        {
            BuildKeyMap();
            AssignValuesToStaticProperties();
        }

        private static void BuildKeyMap()
        {
            var rootType = typeof(LocalizationKeys);
            foreach (var fileType in rootType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
            {
                string file = fileType.Name.ToLowerInvariant();
                WalkType(fileType, new[] { fileType.Name }, file);
            }
        }

        private static void WalkType(Type type, string[] pathSegments, string file)
        {
            var nested = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
            if (nested.Any())
            {
                foreach (var nestedType in nested)
                    WalkType(nestedType,
                             pathSegments.Append(nestedType.Name).ToArray(),
                             file);
            }
            else
            {
                foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Static))
                {
                    if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field) continue;
                    string name = member.Name;
                    var segments = pathSegments.Append(name).ToArray();
                    string lookup = string.Join(KeySeparator, segments);
                    string key = file + FileSeparator + string.Join(KeySeparator, segments.Skip(1));
                    _keyMap[lookup] = key;
                }
            }
        }

        private static void AssignValuesToStaticProperties()
        {
            var rootType = typeof(LocalizationKeys);
            foreach (var fileType in rootType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
                AssignForType(fileType, new[] { fileType.Name });
        }

        private static void AssignForType(Type type, string[] pathSegments)
        {
            var nested = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
            if (nested.Any())
            {
                foreach (var nestedType in nested)
                    AssignForType(nestedType, pathSegments.Append(nestedType.Name).ToArray());
            }
            else
            {
                // assign props
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    if (!prop.CanWrite) continue;
                    var lookup = string.Join(KeySeparator, pathSegments.Append(prop.Name));
                    if (_keyMap.TryGetValue(lookup, out var key))
                        prop.SetValue(null, key);
                }
                // assign fields
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var lookup = string.Join(KeySeparator, pathSegments.Append(field.Name));
                    if (_keyMap.TryGetValue(lookup, out var key))
                        field.SetValue(null, key);
                }
            }
        }
    }
}
