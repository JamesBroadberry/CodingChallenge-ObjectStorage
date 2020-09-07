using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Xunit;

namespace ObjectStorage.JsonStore.Test
{
    public class JsonStoreTest : IDisposable
    {
        private readonly string _rootFileLocation;

        // Setup
        public JsonStoreTest()
        {
            _rootFileLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "StorageLocation");
        }

        // Teardown
        public void Dispose()
        {
            if (Directory.Exists(_rootFileLocation))
            {
                Directory.Delete(_rootFileLocation, true);
            }
        }

        [Fact]
        public void WhenStoringAnObject_AFileIsWrittenToDisk()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var itemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "value"}
                }
            };

            // Act
            _ = store.Put(itemToStore);

            // Assert
            Assert.True(File.Exists(Path.Combine(_rootFileLocation, id.ToString())));

        }

        [Fact]
        public void AfterAnObjectIsStored_RetrievingItMatches()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var itemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "value"}
                }
            };

            _ = store.Put(itemToStore);

            // Act
            var storedItem = store.Get(id);

            // Assert (using serialization as a quick way to deep check the values of dictionary)
            var itemToStorePropertiesAsJson = JsonSerializer.Serialize(itemToStore.Properties);
            var storedPropertiesAsJson = JsonSerializer.Serialize(storedItem.Properties);

            Assert.Equal(itemToStore.Id, storedItem.Id);
            Assert.Equal(itemToStorePropertiesAsJson, storedPropertiesAsJson);
        }

        [Fact]
        public void AfterOverwritingAnExistingObjectUsingPut_OnlyOneFileExistsOnDisk()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var initialItemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "value"}
                }
            };

            _ = store.Put(initialItemToStore);

            // Act
            var itemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "newValue"}
                }
            };

            _ = store.Put(itemToStore);

            // Assert
            Assert.Single(Directory.GetFiles(_rootFileLocation));
        }

        [Fact]
        public void WhenOverwritingAnExistingObjectUsingPut_TheNewerObjectIsReturned()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var initialItemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "value"}
                }
            };

            _ = store.Put(initialItemToStore);

            // Act
            var itemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "newValue"}
                }
            };

            _ = store.Put(itemToStore);

            var storedItem = store.Get(id);

            // Assert
            Assert.Equal("newValue", storedItem.Properties["key"].ToString());
        }

        [Fact]
        public void WhenGettingAnObjectWhichDoesNotExist_ANotFoundExceptionIsThrown()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);


            // Assert
            Assert.Throws<KeyNotFoundException>(() =>
            {
                store.Get(Guid.NewGuid());
            });
        }

        [Fact]
        public void WhenDeletingAnObjectWhichDoesNotExist_ANotFoundExceptionIsThrown()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);

            // Assert
            Assert.Throws<KeyNotFoundException>(() =>
            {
                store.Delete(Guid.NewGuid());
            });
        }

        [Fact]
        public void AfterDeletingAnObject_AttemptingToGetItThrowsANotFoundException()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var initialItemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "value"}
                }
            };

            _ = store.Put(initialItemToStore);

            // Act
            store.Delete(id);

            // Assert
            Assert.Throws<KeyNotFoundException>(() =>
            {
                store.Get(id);
            });
        }

        [Fact]
        public void AfterDeletingAnObject_TheCorrespondingFileIsDeletedFromDisk()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var initialItemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "value"}
                }
            };

            _ = store.Put(initialItemToStore);

            // Act
            store.Delete(id);

            // Assert
            Assert.Empty(Directory.GetFiles(_rootFileLocation));
        }

        [Fact]
        public void WhenAttemptingToUseJsonStoreWithUnauthorizedDirectory_ExceptionIsThrown()
        {
            // Arrange
            var store = new JsonStore("C:\\"); // This will only break on Windows - could make the path OS dependent
            var id = Guid.NewGuid();
            var itemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "value"}
                }
            };

            // Assert
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                _ = store.Put(itemToStore);
            });
        }

        [Fact]
        public void WhenPuttingPropertiesWithComplexValueTypes_TheyAreReturnedCorrectly()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var complexValue = new ExampleComplexSerializableType
            {
                StringValue = "example",
                IntegerValue = 12345
            };
            var itemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", complexValue}
                }
            };

            // Act
            _ = store.Put(itemToStore);
            var retrievedItem = store.Get(id);

            // Assert
            var jsonKeyToCheck = retrievedItem.Properties["key"];
            var deserializedValue = JsonSerializer.Deserialize<ExampleComplexSerializableType>(((JsonElement)jsonKeyToCheck).GetRawText());
            Assert.Equal(complexValue.StringValue, deserializedValue.StringValue);
            Assert.Equal(complexValue.IntegerValue, deserializedValue.IntegerValue);

        }

        [Fact]
        public void WhenPuttingThenGettingMultipleTimes_TheOriginalResultShouldBeTheSame()
        {
            // Arrange
            var store = new JsonStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var initialItemToStore = new StorableBase
            {
                Id = id,
                Properties = new Dictionary<string, object>
                {
                    {"key", "value"}
                }
            };

            // Act
            _ = store.Put(initialItemToStore);
            var retrievedFirstTime = store.Get(id);
            _ = store.Put(retrievedFirstTime);
            var retrievedSecondTime = store.Get(id);
            _ = store.Put(retrievedSecondTime);
            var retrievedThirdTime = store.Get(id);

            // Assert (using serialization as a quick way to deep check the values of dictionary)
            var itemToStorePropertiesAsJson = JsonSerializer.Serialize(initialItemToStore.Properties);
            var storedPropertiesAsJson = JsonSerializer.Serialize(retrievedThirdTime.Properties);

            Assert.Equal(initialItemToStore.Id, retrievedThirdTime.Id);
            Assert.Equal(itemToStorePropertiesAsJson, storedPropertiesAsJson);
        }
    }
}
