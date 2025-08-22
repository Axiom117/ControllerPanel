using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MC104
{
    public class ControllerMapping
    {
        public Dictionary<string, int> Controllers { get; set; }

        /// <summary>
        /// Loads a <see cref="ControllerMapping"/> object from a JSON file.
        /// </summary>
        /// <remarks>This method reads the entire content of the specified file and attempts to
        /// deserialize it into a <see cref="ControllerMapping"/> object. Ensure the file contains valid JSON formatted
        /// data that matches the structure of the <see cref="ControllerMapping"/> class.</remarks>
        /// <param name="filePath">The path to the JSON file containing the controller mapping data. Must not be null or empty.</param>
        /// <returns>A <see cref="ControllerMapping"/> object deserialized from the specified file.</returns>
        public static ControllerMapping LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<ControllerMapping>(json);
        }
    }
}
