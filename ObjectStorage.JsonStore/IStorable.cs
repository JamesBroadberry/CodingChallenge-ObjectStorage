using System.Collections.Generic;

namespace ObjectStorage.JsonStore
{
    public interface IStorable : IUnique
    {
        IDictionary<string, object> Properties { get; }
    }
}