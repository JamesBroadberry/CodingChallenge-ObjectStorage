using System;

namespace ObjectStorage.JsonStore
{
    public interface IUnique
    {
        Guid Id { get; }
    }
}