using System;

namespace TeacherPouch.Models
{
    public class Question
    {
        public int ID { get; set; }
        public int PhotoID { get; set; }
        public string Text { get; set; }
        public string SentenceStarters { get; set; }
        public int? Order { get; set; }


        public override string ToString()
        {
            return String.Format("{0} - {1}", this.ID, this.Text);
        }
    }
}
