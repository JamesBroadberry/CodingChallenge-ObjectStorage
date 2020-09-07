using System;

namespace ObjectStorage.JsonStore
{
    public interface IStore
    {
        IStorable Get(Guid id);
        IStorable Put(IStorable item);
        void Delete(Guid id);
    }

}
