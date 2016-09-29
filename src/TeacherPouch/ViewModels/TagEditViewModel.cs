using System.ComponentModel.DataAnnotations;
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
            Name = tag.Name;
            IsPrivate = tag.IsPrivate;
        }

        [Required]
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
    }
}
