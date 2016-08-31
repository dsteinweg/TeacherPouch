using System;
using System.Net;

namespace TeacherPouch.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorViewModel(int? httpStatusCode, Exception exception, bool showErrorDetails)
        {
            if (httpStatusCode.HasValue)
                StatusCode = httpStatusCode.Value;

            if (showErrorDetails)
                ExceptionDetails = exception.ToString();
        }

        public string ExceptionDetails { get; set; }
        public int StatusCode { get; set; } = (int)HttpStatusCode.InternalServerError;
    }
}
