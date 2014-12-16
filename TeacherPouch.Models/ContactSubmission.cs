using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;

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
            if (String.IsNullOrWhiteSpace(this.Name) &&
                String.IsNullOrWhiteSpace(this.Email) &&
                String.IsNullOrWhiteSpace(this.Comment))
            {
                yield return new ValidationResult("You must fill out the form before submitting.");
            }
        }

        public void SendEmail()
        {
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
                    message.Subject = String.Format("TeacherPouch.com Contact Submission - {0}", this.Name);
                    message.IsBodyHtml = true;
                    message.Body = String.Format(BODY_HTML_FORMAT, this.Name, this.Email, this.ReasonForContacting, this.Comment);

                    using (var smtp = new SmtpClient(smtpConfig.Network.Host))
                    {
                        smtp.Credentials = new NetworkCredential(smtpConfig.Network.UserName, smtpConfig.Network.Password);

                        smtp.Send(message);
                    }
                }
            }
        }
    }
}
