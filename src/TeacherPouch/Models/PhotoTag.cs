namespace TeacherPouch.Models;

public class PhotoTag
{
    public int PhotoId { get; set; }
    public Photo Photo { get; set; } = default!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}
