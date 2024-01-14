namespace TeacherPouch.ViewModels;

public class PhotoCreateViewModel(string pendingPhotoPath)
{
    public string PendingPhotoPath { get; set; } = pendingPhotoPath;
    public string? PhotoName { get; set; }
    public string? FileName { get; set; }
    public string? Tags { get; set; }
    public bool IsPrivate { get; set; }
    public string? Message { get; set; }
    public string? ProposedPhotoName { get; set; }
    public string? ErrorMessage { get; set; }
}
