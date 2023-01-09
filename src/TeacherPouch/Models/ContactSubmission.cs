using System.ComponentModel.DataAnnotations;

namespace TeacherPouch.Models;

public class ContactSubmission : IValidatableObject
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? ReasonForContacting { get; set; }
    public string? Comment { get; set; }


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Name) &&
            string.IsNullOrWhiteSpace(Email) &&
            string.IsNullOrWhiteSpace(Comment))
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

        if (    !string.IsNullOrWhiteSpace(to)
             && smtpConfig is not null
             && !string.IsNullOrWhiteSpace(smtpConfig.From)
             && smtpConfig.Network is not null
             && !string.IsNullOrWhiteSpace(smtpConfig.Network.Host)
             && !string.IsNullOrWhiteSpace(smtpConfig.Network.UserName)
             && !string.IsNullOrWhiteSpace(smtpConfig.Network.Password))
        {
            using (var message = new MailMessage(smtpConfig.From, to))
            {
                message.Subject = string.Format("TeacherPouch.com Contact Submission - {0}", Name);
                message.IsBodyHtml = true;
                message.Body = string.Format(BODY_HTML_FORMAT, Name, Email, ReasonForContacting, Comment);

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
