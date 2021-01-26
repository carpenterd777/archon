using System.IO;
using System.Text.Json;

namespace Archon
{
    public class ArchonJsonWriterFactory 
    {
        /// <summary>
        /// Returns a Utf8JsonWriter with custom options to be used throughout the entirety of Archon.
        /// </summary>
        public static Utf8JsonWriter CreateArchonJsonWriter(Stream stream)
        {
            JsonWriterOptions options = new();
            options.Indented = true;

            return new Utf8JsonWriter(stream, options);
        }
    }
}
