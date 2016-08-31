using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherPouch.Models
{
    public class ContactSubmission : IValidatableObject
    {
        private const string BODY_HTML_FORMAT =
            "<div>" +
                "<div><b>Name:</b></div>" +
                "<div>{0}</div>" +
                "<br>" +
                "<div><b>Email:</b></div>" +
                "<div>{1}</div>" +
                "<br>" +
                "<div><b>Reason for contacting:</b></div>" +
                "<div>{2}</div>" +
                "<br>" +
                "<div><b>Comment/Question:</b></div>" +
                "<div>{3}</div>" +
            "</div>";

        public string Name { get; set; }
        public string Email { get; set; }
        public string ReasonForContacting { get; set; }
        public string Comment { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (String.IsNullOrWhiteSpace(Name) &&
                String.IsNullOrWhiteSpace(Email) &&
                String.IsNullOrWhiteSpace(Comment))
            {
                errors.Add(new ValidationResult("You must fill out the form before submitting."));
            }

            return errors;
        }

        public void SendEmail()
        {
            // TODO: re-implement, using options
            /*
            var to = ConfigurationManager.AppSettings["ContactEmailAddress"];
            var smtpConfig = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;

            if (    !String.IsNullOrWhiteSpace(to)
                 && smtpConfig != null
                 && !String.IsNullOrWhiteSpace(smtpConfig.From)
                 && smtpConfig.Network != null
                 && !String.IsNullOrWhiteSpace(smtpConfig.Network.Host)
                 && !String.IsNullOrWhiteSpace(smtpConfig.Network.UserName)
                 && !String.IsNullOrWhiteSpace(smtpConfig.Network.Password))
            {
                using (var message = new MailMessage(smtpConfig.From, to))
                {
                    message.Subject = String.Format("TeacherPouch.com Contact Submission - {0}", Name);
                    message.IsBodyHtml = true;
                    message.Body = String.Format(BODY_HTML_FORMAT, Name, Email, ReasonForContacting, Comment);

                    using (var smtp = new SmtpClient(smtpConfig.Network.Host))
                    {
                        smtp.Credentials = new NetworkCredential(smtpConfig.Network.UserName, smtpConfig.Network.Password);

                        smtp.Send(message);
                    }
                }
            }
            */
        }
    }
}
