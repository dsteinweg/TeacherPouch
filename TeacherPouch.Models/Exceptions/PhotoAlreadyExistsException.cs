using System;

namespace TeacherPouch.Models.Exceptions
{
    public class PhotoAlreadyExistsException : Exception
    {
        public string Path { get; set; }

        public PhotoAlreadyExistsException(string path)
        {
            this.Path = path;
        }
    }
}
