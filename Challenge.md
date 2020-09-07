# Coding Challenge Specification

## Create a dotnet standard 2.0 Solution containing the following:

1. A C# project that implements an `IStore` called JsonStore that stores each item locally in a json file (one file per object). Items should persist unless deleted.

```csharp
public interface IUnique
{
    Guid Id { get; }
}

public interface IStorable : IUnique
{
    IDictionary<string, object> Properties { get; }
}

public interface IStore
{
    IStorable Get(Guid id);
    IStorable Put(IStorable item);
    void Delete(Guid id);
}
```

2. A C# unittest project that proves your JsonStore works. Your choice of test/assertion library.

3. A C# project that implements an `IObjectStore` called ObjectStore. ObjectStore should use your `IStore` implementation for its underlying storage needs. You may add type parameter constraints to `IObjectStore` if necessary.

```csharp
public interface IObjectStore
{
    T Get<T>(Guid id);
    T Put<T>(T item);
    void Delete(Guid id);
}
```

4. A C# unittest project that proves your ObjectStore works for Cars and Books. You may extend Car and Book.

```csharp
public class Car : IUnique
{
    public Guid Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Doors { get; set; }
}

public class Book : IUnique
{
    public Guid Id { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
}
```
