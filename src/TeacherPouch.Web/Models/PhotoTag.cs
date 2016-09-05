namespace TeacherPouch.Models
{
    public class PhotoTag
    {
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }

        public string TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
