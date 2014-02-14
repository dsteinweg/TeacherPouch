using TeacherPouch.Models;
using TeacherPouch.Utilities;

namespace TeacherPouch.Web.ViewModels
{
    public class PhotoCreateViewModel
    {
        public Photo Photo { get; private set; }
        public string PendingPhotoPath { get; private set; }
        public string Message { get; set; }
        public string ProposedPhotoName { get; set; }
        public string LastTagsInput { get; set; }
        public string ErrorMessage { get; set; }

        public PhotoCreateViewModel()
        {
            this.Photo = new Photo();

            var pendingPhotoPath = PhotoHelper.PendingPhotoPath;
            if (!pendingPhotoPath.EndsWith(@"\"))
                pendingPhotoPath = pendingPhotoPath + @"\";

            this.PendingPhotoPath = pendingPhotoPath;
        }
    }
}
