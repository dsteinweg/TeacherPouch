using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeacherPouch.Web.ViewModels
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string LoginErrorMessage { get; set; }

        public LoginViewModel(string userName, string loginErrorMessage)
        {
            this.UserName = userName;
            this.LoginErrorMessage = loginErrorMessage;
        }
    }
}