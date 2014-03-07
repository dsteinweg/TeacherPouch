using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using TeacherPouch.Models;

namespace TeacherPouch.Web.ViewModels
{
    public class QuestionEditViewModel : IValidatableObject
    {
        public Photo Photo { get; set; }
        public IEnumerable<Tag> PhotoTags { get; set; }

        public int QuestionID { get; set; }
        [Display(Name = "Question")]
        public string QuestionText { get; set; }
        [Display(Name = "SentenceStarters")]
        public string QuestionSentenceStarters { get; set; }
        [Display(Name = "Question Order (optional)")]
        public string QuestionOrder { get; set; }

        public bool DisplayAdminLinks { get; set; }


        public QuestionEditViewModel()
        {
            this.Photo = new Photo();
            this.PhotoTags = Enumerable.Empty<Tag>();
        }

        public QuestionEditViewModel(Question question, Photo photo, IEnumerable<Tag> photoTags)
        {
            this.QuestionID = question.ID;
            this.QuestionText = question.Text;
            this.QuestionSentenceStarters = question.SentenceStarters;

            if (question.Order.HasValue)
                this.QuestionOrder = question.Order.Value.ToString();

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