using TeacherPouch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeacherPouch.Web.ViewModels
{
    public class ContactViewModel
    {
        public string ErrorMessage { get; set; }
        public ContactSubmission Submission { get; set; }

        public ContactViewModel()
        {
            this.Submission = new ContactSubmission();
        }
    }
}
