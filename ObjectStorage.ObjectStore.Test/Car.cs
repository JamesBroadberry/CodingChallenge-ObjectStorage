using System;
using ObjectStorage.JsonStore;

namespace ObjectStorage.ObjectStore.Test
{
    public class Car : IUnique, IEquatable<Car>
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Doors { get; set; }

        public bool Equals(Car other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && Make == other.Make && Model == other.Model && Doors == other.Doors;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Car) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Make, Model, Doors);
        }
    }
}