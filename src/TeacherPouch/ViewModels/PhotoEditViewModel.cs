using System.ComponentModel.DataAnnotations;

namespace TeacherPouch.ViewModels
{
    public class PhotoEditViewModel
    {
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsPrivate { get; set; }
        [DataType(DataType.MultilineText)]
        public string Tags { get; set; }
    }
}
