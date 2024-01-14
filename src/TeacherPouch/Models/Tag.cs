namespace TeacherPouch.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsPrivate { get; set; }
    public List<PhotoTag> PhotoTags { get; set; } = [];

    public override string ToString() => $"{Id} - {Name}";

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        return GetHashCode() == obj.GetHashCode();
    }

    public override int GetHashCode() => Id;
}
