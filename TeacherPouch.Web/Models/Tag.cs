namespace TeacherPouch.Models
{
    public class Tag
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }

        public override bool Equals(object obj)
        {
            return (GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public override string ToString()
        {
            return $"{ID} - {Name}";
        }
    }
}
