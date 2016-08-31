using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class QuestionCreateViewModel : IValidatableObject
    {
        public Photo Photo { get; set; }
        public IEnumerable<Tag> PhotoTags { get; set; }

        [Display(Name = "Question")]
        public string QuestionText { get; set; }

        [Display(Name = "Sentence Starters")]
        public string QuestionSentenceStarters { get; set; }

        [Display(Name = "Question Order (optional)")]
        public string QuestionOrder { get; set; }


        public QuestionCreateViewModel()
        {
            this.Photo = new Photo();
            this.PhotoTags = Enumerable.Empty<Tag>();
        }

        public QuestionCreateViewModel(Photo photo, IEnumerable<Tag> photoTags)
        {
            this.Photo = photo;
            this.PhotoTags = photoTags;
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Photo == null || this.Photo.ID <= 0)
                yield return new ValidationResult("Missing photo ID.");

            if (String.IsNullOrWhiteSpace(this.QuestionText))
                yield return new ValidationResult("Question text cannot be empty.");

            int questionOrder;
            if (!String.IsNullOrWhiteSpace(this.QuestionOrder) && !Int32.TryParse(this.QuestionOrder, out questionOrder))
                yield return new ValidationResult("Question order must be a whole number.");
        }
    }
}
