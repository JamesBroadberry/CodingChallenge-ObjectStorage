using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

namespace ObjectStorage.ObjectStore.Test
{
    public class ObjectStoreTest : IDisposable
    {
        private readonly string _rootFileLocation;

        // Setup
        public ObjectStoreTest()
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
        public void PuttingABookThenGettingById_Matches()
        {
            // Arrange
            var store = new ObjectStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var book = new Book
            {
                Id = id,
                Author = "Harper Lee",
                Title = "To Kill A Mockingbird",
                ISBN = "9781784752637"
            };

            // Act
            _ = store.Put(book);
            var retrievedBook = store.Get<Book>(id);

            // Assert
            Assert.Equal(book, retrievedBook);
        }

        [Fact]
        public void PuttingACarThenGettingById_Matches()
        {
            // Arrange
            var store = new ObjectStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var car = new Car
            {
                Id = id,
                Make = "Ford",
                Model = "Fiesta",
                Doors = 3
            };

            // Act
            _ = store.Put(car);
            var retrievedCar = store.Get<Car>(id);

            // Assert
            Assert.Equal(car, retrievedCar);
        }

        [Fact]
        public void AfterOverwritingABookWithACar_TheCarIsReturned()
        {
            // Arrange
            var store = new ObjectStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var book = new Book
            {
                Id = id,
                Author = "Harper Lee",
                Title = "To Kill A Mockingbird",
                ISBN = "9781784752637"
            };

            _ = store.Put(book);

            var car = new Car
            {
                Id = id,
                Make = "Ford",
                Model = "Fiesta",
                Doors = 3
            };

            // Act
            _ = store.Put(car);

            // Assert
            var retrievedCar = store.Get<Car>(id);
            Assert.Equal(car, retrievedCar);
        }


        [Fact]
        public void AfterDeletingACarFromTheStore_AttemptingToRetrieveItByIdThrowsNotFoundException()
        {
            // Arrange
            var store = new ObjectStore(_rootFileLocation);
            var id = Guid.NewGuid();
            var book = new Book
            {
                Id = id,
                Author = "Harper Lee",
                Title = "To Kill A Mockingbird",
                ISBN = "9781784752637"
            };

            _ = store.Put(book);

            // Act
            store.Delete(id);

            // Assert
            Assert.Throws<KeyNotFoundException>(() => { store.Get<Book>(id); });
        }
    }
}