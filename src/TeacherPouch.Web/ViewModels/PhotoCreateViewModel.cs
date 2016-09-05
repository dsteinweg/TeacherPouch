namespace TeacherPouch.ViewModels
{
    public class PhotoCreateViewModel
    {
        public PhotoCreateViewModel(string pendingPhotoPath)
        {
            PendingPhotoPath = pendingPhotoPath;
        }

        public string PendingPhotoPath { get; set; }
        public string PhotoName { get; set; }
        public string FileName { get; set; }
        public string Tags { get; set; }
        public bool IsPrivate { get; set; }
        public string Message { get; set; }
        public string ProposedPhotoName { get; set; }
    }
}
