using System;
using System.Collections.Generic;

namespace ObjectStorage.JsonStore
{
    public class StorableBase : IStorable
    {
        public IDictionary<string, object> Properties { get; set; }

        public Guid Id { get; set;  }
    }
}
