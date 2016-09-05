using System;
using System.Collections.Generic;

namespace TeacherPouch.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid UniqueId { get; set; } = Guid.NewGuid();
        public bool IsPrivate { get; set; }
        public List<PhotoTag> PhotoTags { get; set; }
        public List<Question> Questions { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Id == (obj as Photo).Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
