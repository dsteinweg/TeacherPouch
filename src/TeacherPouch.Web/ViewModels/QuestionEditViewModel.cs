using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class QuestionEditViewModel : IValidatableObject
    {
        public QuestionEditViewModel()
        {

        }

        public QuestionEditViewModel(Question question, Photo photo)
        {
            QuestionId = question.Id;
            QuestionText = question.Text;
            QuestionSentenceStarters = question.SentenceStarters;

            if (question.Order.HasValue)
                QuestionOrder = question.Order.Value.ToString();

            Photo = photo;
            PhotoTags = photo.PhotoTags.Select(photoTag => photoTag.Tag);
        }

        public Photo Photo { get; set; } = new Photo();
        public IEnumerable<Tag> PhotoTags { get; set; } = Enumerable.Empty<Tag>();
        public int QuestionId { get; set; }
        [Display(Name = "Question")]
        public string QuestionText { get; set; }
        [Display(Name = "SentenceStarters")]
        public string QuestionSentenceStarters { get; set; }
        [Display(Name = "Question Order (optional)")]
        public string QuestionOrder { get; set; }
        public bool DisplayAdminLinks { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Photo == null || Photo.Id <= 0)
                results.Add(new ValidationResult("Missing photo ID."));

            if (QuestionId <= 0)
                results.Add(new ValidationResult("Missing question ID."));

            if (String.IsNullOrWhiteSpace(QuestionText))
                results.Add(new ValidationResult("Question text cannot be empty."));

            int questionOrder;
            if (!String.IsNullOrWhiteSpace(QuestionOrder) && !Int32.TryParse(QuestionOrder, out questionOrder))
                results.Add(new ValidationResult("Question order must be a whole number."));

            return results;
        }
    }
}
