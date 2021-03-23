using System;
using System.Text.Json;

namespace Archon
{
    public interface IEntry
    {
        public void AddToJsonWriter(Utf8JsonWriter jsonWriter);
        public string GetData();
    }
}
