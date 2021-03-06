using System.IO;
using System.Buffers;
using System.Text.Json;

namespace Archon
{
    internal class JsonWriterFactory 
    {
        /// <summary>
        /// Returns a Utf8JsonWriter with custom options to be used throughout the entirety of Archon.
        /// </summary>
        public static Utf8JsonWriter CreateJsonWriter(Stream stream)
        {
            JsonWriterOptions options = new();
            options.Indented = true;

            return new Utf8JsonWriter(stream, options);
        }
    }

    internal class JsonReaderFactory 
    {
        /// <summary>
        /// Returns a Utf8JsonWriter with custom options to be used throughout the entirety of Archon.
        /// </summary>
        public static Utf8JsonReader CreateJsonReader(ReadOnlySequence<byte> ros)
        {
            JsonReaderOptions options = new();

            return new Utf8JsonReader(ros, options);
        }
    }
}
