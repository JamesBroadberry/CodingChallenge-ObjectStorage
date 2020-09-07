using System;
using ObjectStorage.JsonStore;

namespace ObjectStorage.ObjectStore.Test
{
    public class Book : IUnique, IEquatable<Book>
    {
        public Guid Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }

        public bool Equals(Book other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && Author == other.Author && Title == other.Title && ISBN == other.ISBN;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Book) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Author, Title, ISBN);
        }
    }
}