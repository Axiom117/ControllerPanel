using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace MC104
{
    public sealed class KeyboardMappingConfig
    {
        public List<KeyboardBinding> Bindings { get; set; } = new List<KeyboardBinding>();

        public static KeyboardMappingConfig Load(string path)
        {
            var json = File.ReadAllText(path);
            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<KeyboardMappingConfig>(json);
            return config ?? new KeyboardMappingConfig();
        }
    }

    public sealed class KeyboardBinding
    {
        public string Action { get; set; }

        public string Key { get; set; }
    }
}