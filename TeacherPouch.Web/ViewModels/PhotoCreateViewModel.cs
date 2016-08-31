using TeacherPouch.Helpers;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class PhotoCreateViewModel
    {
        public PhotoCreateViewModel()
        {
            var pendingPhotoPath = PhotoHelper.PendingPhotoPath;
            if (!pendingPhotoPath.EndsWith(@"\"))
                pendingPhotoPath = pendingPhotoPath + @"\";

            PendingPhotoPath = pendingPhotoPath;
        }

        public Photo PhotoName { get; set; }
        public string PendingPhotoPath { get; set; }
        public string Message { get; set; }
        public string ProposedPhotoName { get; set; }
        public string LastTagsInput { get; set; }
        public string ErrorMessage { get; set; }
    }
}
