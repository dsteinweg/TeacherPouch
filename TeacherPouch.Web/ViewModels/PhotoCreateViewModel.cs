using System.Web;

using TeacherPouch.Models;
using TeacherPouch.Utilities;
using TeacherPouch.Utilities.Caching;

namespace TeacherPouch.Web.ViewModels
{
    public class PhotoCreateViewModel
    {
        public Photo Photo { get; private set; }
        public string PendingPhotoPath { get; private set; }
        public string LastTagsInput { get; private set; }
        public string ErrorMessage { get; set; }

        public PhotoCreateViewModel()
        {
            this.Photo = new Photo();

            var pendingPhotoPath = PhotoHelper.PendingPhotoPath;
            if (!pendingPhotoPath.EndsWith(@"\"))
                pendingPhotoPath = pendingPhotoPath + @"\";

            this.PendingPhotoPath = pendingPhotoPath;

            this.LastTagsInput = CacheHelper.RetrieveFromCache<string>("LastTagsInput");
        }
    }
}
