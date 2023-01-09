using System.ComponentModel.DataAnnotations;

namespace TeacherPouch.ViewModels;

public class TagCreateViewModel
{
    [Required]
    [Display(Name = "Tag Name")]
    public string? TagName { get; set; }
    public bool Private { get; set; }
    public string? ErrorMessage { get; set; }
}
