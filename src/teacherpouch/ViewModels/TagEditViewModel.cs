using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class TagEditViewModel
    {
        public TagEditViewModel()
        {

        }

        public TagEditViewModel(Tag tag)
        {
            TagName = tag.Name;
            IsPrivate = tag.IsPrivate;
        }

        public string TagName { get; set; }
        public bool IsPrivate { get; set; }
        public string ErrorMessage { get; set; }
    }
}
