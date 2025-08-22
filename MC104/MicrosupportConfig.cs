using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MC104.Models
{
    public class MicrosupportConfig
    {
        public Resolutions Resolutions { get; set; }
        public Params Params { get; set; }

        public static MicrosupportConfig LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<MicrosupportConfig>(json);
        }
    }

    public class Resolutions
    {
        public double axisX { get; set; }
        public double axisY { get; set; }
        public double axisZ { get; set; }
    }

    public class Params
    {
        public int maxControllers { get; set; }
        public int iconSizeU { get; set; }
        public int iconSizeV { get; set; }
        public int spacing { get; set; }
        public int startX { get; set; }
        public int startY { get; set; }

    }
}