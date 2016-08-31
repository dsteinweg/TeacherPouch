using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class QuestionDetailsViewModel : IValidatableObject
    {
        public Photo Photo { get; set; }
        public IEnumerable<Tag> PhotoTags { get; set; }

        public int QuestionID { get; set; }
        [Display(Name = "Question")]
        public string QuestionText { get; set; }
        [Display(Name = "Sentence Starters")]
        public string QuestionSentenceStarters { get; set; }
        [Display(Name = "Question Order (optional)")]
        public string QuestionOrder { get; set; }

        public bool DisplayAdminLinks { get; set; }


        public QuestionDetailsViewModel()
        {
            this.Photo = new Photo();
            this.PhotoTags = Enumerable.Empty<Tag>();
        }

        public QuestionDetailsViewModel(Question question, Photo photo, IEnumerable<Tag> photoTags)
        {
            this.QuestionID = question.ID;
            this.QuestionText = question.Text;
            this.QuestionSentenceStarters = !String.IsNullOrWhiteSpace(question.SentenceStarters) ? question.SentenceStarters : "(None)";
            this.QuestionOrder = (question.Order.HasValue) ? question.Order.Value.ToString() : "(None)";

            this.Photo = photo;
            this.PhotoTags = photoTags;
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Photo == null || this.Photo.ID <= 0)
                yield return new ValidationResult("Missing photo ID.");

            if (this.QuestionID <= 0)
                yield return new ValidationResult("Missing question ID.");

            if (String.IsNullOrWhiteSpace(this.QuestionText))
                yield return new ValidationResult("Question text cannot be empty.");

            int questionOrder;
            if (!String.IsNullOrWhiteSpace(this.QuestionOrder) && !Int32.TryParse(this.QuestionOrder, out questionOrder))
                yield return new ValidationResult("Question order must be a whole number.");
        }
    }
}
