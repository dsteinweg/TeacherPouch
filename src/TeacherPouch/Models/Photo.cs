namespace TeacherPouch.Models;

public class Photo
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid UniqueId { get; set; } = Guid.NewGuid();
    public bool IsPrivate { get; set; }
    public List<PhotoTag> PhotoTags { get; set; } = [];
    public List<Question> Questions { get; set; } = [];

    public override string ToString() => $"{Id} - {Name}";

    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        if (obj is Photo photo)
            return Id == photo.Id;

        return false;
    }

    public override int GetHashCode() => Id;
}
