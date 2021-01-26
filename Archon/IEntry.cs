using System;
using System.Text.Json;

namespace Archon
{
    interface IEntry
    {
        public void AddToJsonWriter(Utf8JsonWriter jsonWriter);
    }
}
