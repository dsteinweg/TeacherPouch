using Microsoft.AspNetCore.Razor.TagHelpers;
using TeacherPouch.Models;

namespace TeacherPouch.TagHelpers
{
    [HtmlTargetElement("tag-button")]
    public class TagButtonTagHelper : TagHelper
    {
        public Tag Tag { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            var cssClasses = "tag";
            if (Tag.IsPrivate)
            {
                cssClasses = cssClasses + " private";

                output.Attributes.SetAttribute("title", "This tag is private");
            }

            output.Attributes.SetAttribute("class", cssClasses);

            output.Content.SetContent(Tag.Name);
        }
    }
}
