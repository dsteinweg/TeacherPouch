using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherPouch.ViewModels
{
    public class ContactViewModel : IValidatableObject
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ReasonForContacting { get; set; }
        public string Comment { get; set; }
        public string ErrorMessage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (String.IsNullOrWhiteSpace(Name) ||
                String.IsNullOrWhiteSpace(Email) ||
                String.IsNullOrWhiteSpace(Comment))
            {
                errors.Add(new ValidationResult("You must fill out the form before submitting."));
            }

            return errors;
        }
    }
}
