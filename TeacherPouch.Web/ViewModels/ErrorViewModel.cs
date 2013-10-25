using System;
using System.Net;
using System.Web;

namespace TeacherPouch.Web.ViewModels
{
    public class ErrorViewModel
    {
        public string ExceptionDetails { get; set; }
        public int StatusCode { get; set; }

        public ErrorViewModel(int? httpStatusCode, Exception exception, bool showErrorDetails)
        {
            if (httpStatusCode.HasValue)
                this.StatusCode = httpStatusCode.Value;
            else if (exception != null && (exception is HttpException))
                this.StatusCode = ((HttpException)exception).GetHttpCode();
            else
                this.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (showErrorDetails)
                this.ExceptionDetails = exception.ToString();
        }
    }
}