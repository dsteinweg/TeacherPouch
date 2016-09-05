using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class QuestionCreateViewModel : IValidatableObject
    {
        public QuestionCreateViewModel()
        {

        }

        public QuestionCreateViewModel(Photo photo)
        {
            Photo = photo;
            Tags = photo.PhotoTags.Select(photoTag => photoTag.Tag);
        }

        public Photo Photo { get; set; } = new Photo();
        public IEnumerable<Tag> Tags { get; set; } = Enumerable.Empty<Tag>();

        [Display(Name = "Question")]
        public string QuestionText { get; set; }

        [Display(Name = "Sentence Starters")]
        public string QuestionSentenceStarters { get; set; }

        [Display(Name = "Question Order (optional)")]
        public string QuestionOrder { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Photo == null || Photo.Id <= 0)
                results.Add(new ValidationResult("Missing photo ID."));

            if (String.IsNullOrWhiteSpace(QuestionText))
                results.Add(new ValidationResult("Question text cannot be empty."));

            int questionOrder;
            if (!String.IsNullOrWhiteSpace(QuestionOrder) && !Int32.TryParse(QuestionOrder, out questionOrder))
                results.Add(new ValidationResult("Question order must be a whole number."));

            return results;
        }
    }
}
