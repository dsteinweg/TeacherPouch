using System;

namespace TeacherPouch.Models
{
    public class Tag
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }


        public Tag()
        {

        }

        public Tag(string name)
        {
            this.Name = name;
        }


        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", this.ID, this.Name);
        }
    }
}
