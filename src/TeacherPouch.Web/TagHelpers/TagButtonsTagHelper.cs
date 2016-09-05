using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TeacherPouch.Models;

namespace TeacherPouch.TagHelpers
{
    public class TagButtonsTagHelper : TagHelper
    {
        public IEnumerable<Tag> Tags { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            foreach (var tag in Tags)
            {
                var tagButtonTagHelper = new TagButtonTagHelper { Tag = tag };

                tagButtonTagHelper.Process(context, output);
            }
        }
    }
}
