using System;

namespace TeacherPouch.Models
{
    public class Photo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Guid UniqueID { get; set; } = Guid.NewGuid();
        public bool IsPrivate { get; set; }

        public override string ToString()
        {
            return $"{ID} - {Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return ID == (obj as Photo).ID;
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}
