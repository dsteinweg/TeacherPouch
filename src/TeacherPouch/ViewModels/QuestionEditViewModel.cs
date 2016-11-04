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
            QuestionOrder = question.Order;
            Photo = photo;
            Tags = photo.PhotoTags.Select(pt => pt.Tag);
        }

        public int QuestionId { get; set; }
        [Display(Name = "Question")]
        public string QuestionText { get; set; }
        [Display(Name = "Sentence Starters")]
        public string QuestionSentenceStarters { get; set; }
        [Display(Name = "Question Order (optional)")]
        public int? QuestionOrder { get; set; }
        public Photo Photo { get; set; } = new Photo();
        public IEnumerable<Tag> Tags { get; set; } = Enumerable.Empty<Tag>();
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

            return results;
        }
    }
}
