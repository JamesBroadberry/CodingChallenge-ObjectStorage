using System;
using System.Collections.Generic;
using System.Text.Json;
using ObjectStorage.JsonStore;

namespace ObjectStorage.ObjectStore
{
    public class ObjectStore : IObjectStore
    {
        private readonly JsonStore.JsonStore _jsonStore;

        public ObjectStore(string rootFileLocation)
        {
            _jsonStore = new JsonStore.JsonStore(rootFileLocation);
        }

        public T Get<T>(Guid id) where T : IUnique
        {
            var storedItem = _jsonStore.Get(id);
            var convertedItem = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(storedItem.Properties));

            if (!convertedItem.Id.Equals(id))
            {
                // Filename ID mismatches Id in file contents
            }

            return convertedItem;
            

            //var itemToReturn = (T)Activator.CreateInstance(typeof(T));
            //var itemType = itemToReturn.GetType();

            //foreach (var propertyInfo in itemType.GetProperties())
            //{
            //    // Setting the value here returns a JsonElement, which doesn't match the expected type so throws.
            //    // Using serialization and deserialization is a safer way to achieve the same goal here without having to convert types manually.
            //    // There may be a performance decrease but for this solution, works well.
            //    propertyInfo.SetValue(itemToReturn, storedItem.Properties[propertyInfo.Name], null);
            //}

            //return itemToReturn;

        }

        public T Put<T>(T item) where T : IUnique
        {
            var type = item.GetType();
            var properties = new Dictionary<string, object>();

            foreach (var propertyInfo in type.GetProperties())
            {
                properties.Add(propertyInfo.Name, propertyInfo.GetValue(item));
            }

            var storable = new StorableBase { Id = item.Id, Properties = properties };
            _jsonStore.Put(storable);

            return item;
        }

        public void Delete(Guid id)
        {
            _jsonStore.Delete(id);
        }
    }
}
