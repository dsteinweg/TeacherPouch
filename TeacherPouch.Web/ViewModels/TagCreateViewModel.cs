using TeacherPouch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeacherPouch.Web.ViewModels
{
    public class TagCreateViewModel
    {
        public Tag Tag { get; set; }
        public string ErrorMessage { get; set; }

        public TagCreateViewModel()
        {
            this.Tag = new Tag();
        }
    }
}