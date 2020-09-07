using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ObjectStorage.JsonStore
{
    public class JsonStore : IStore
    {
        private readonly string _rootFileDirectory;

        public JsonStore(string rootFileDirectory)
        {
            _rootFileDirectory = rootFileDirectory;

            if (!Directory.Exists(_rootFileDirectory))
            {
                Directory.CreateDirectory(_rootFileDirectory);
            }
        }

        public void Delete(Guid id)
        {
            var fileLocation = Path.Combine(_rootFileDirectory, id.ToString());

            if (!File.Exists(fileLocation))
            {
                // TODO: Custom exception
                throw new KeyNotFoundException();
            }

            File.Delete(fileLocation);
        }

        public IStorable Get(Guid id)
        {
            var fileLocation = Path.Combine(_rootFileDirectory, id.ToString());

            if (!File.Exists(fileLocation))
            {
                // TODO: Custom exception
                throw new KeyNotFoundException();
            }

            var fileContents = File.ReadAllText(fileLocation);

            var returnedProperties = JsonSerializer.Deserialize<Dictionary<string, object>>(fileContents);

            var objectToReturn = new StorableBase
            {
                Id = id,
                Properties = returnedProperties
            };

            return objectToReturn;
        }

        public IStorable Put(IStorable item)
        {
            var fileLocation = Path.Combine(_rootFileDirectory, item.Id.ToString());

            var serializedObject = JsonSerializer.Serialize(item.Properties);

            File.WriteAllText(fileLocation, serializedObject);

            return item;
        }
    }
}
