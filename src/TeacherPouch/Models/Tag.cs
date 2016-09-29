using System.Collections.Generic;

namespace TeacherPouch.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public List<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }

        public override bool Equals(object obj)
        {
            return (GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
