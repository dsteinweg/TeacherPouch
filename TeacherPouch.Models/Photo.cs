using System;
using System.Collections.Generic;

namespace TeacherPouch.Models
{
    public class Photo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Guid UniqueID { get; set; }
        public bool IsPrivate { get; set; }


        public Photo()
        {
            this.UniqueID = Guid.NewGuid();
        }


        public override string ToString()
        {
            return String.Format("{0} - {1}", this.ID, this.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.ID == (obj as Photo).ID;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}
