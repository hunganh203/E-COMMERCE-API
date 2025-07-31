namespace Application.Constants
{
    public class FileContentType
    {
        public const string IMAGE = "image/";
        public const string AUDIO = "audio/";
        public const string VIDEO = "video/";
        public const string PDF = "application/pdf";
        public const string TEXT = "text/plain";
        public static string[] DOC = { "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
        public static string[] EXCEL = { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
        public static string[] PPTX = { "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation" };
        public static string[] ZIP = { "application/zip", "application/x-zip-compressed", "application/octet-stream" };
    }

    /// <summary>
    /// Files uploaded origin source: Task, Chat, Upload directly...
    /// </summary>
    public class FileSourceFrom
    {
        /// <summary>
        /// Files uploaded from GED
        /// </summary>
        public const string Storage = "STORAGE";

        /// <summary>
        /// Files attached in Task
        /// </summary>
        public const string Task = "TASK";

        /// <summary>
        /// Files attached in Chat
        /// </summary>
        public const string Chat = "CHAT";

        /// <summary>
        /// Files uploaded as Avatar
        /// </summary>
        public const string Avatar = "AVATAR";
    }
}