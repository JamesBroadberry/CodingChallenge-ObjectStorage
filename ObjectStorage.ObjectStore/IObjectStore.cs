using System;
using ObjectStorage.JsonStore;

namespace ObjectStorage.ObjectStore
{ public interface IObjectStore 
    {
        T Get<T>(Guid id) where T: IUnique;
        T Put<T>(T item) where T : IUnique;
        void Delete(Guid id);
    }
}
