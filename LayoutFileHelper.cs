namespace DesktopICO
{
    public class LayoutFileHelper
    {
        public record LayoutFileInfo(string Prefix, string Resolution, string UserName, DateTime Timestamp);

        public static string CreateFileName(string prefix, bool autoBackup = false)
        {
            string resolution = $"{Screen.PrimaryScreen?.Bounds.Width}x{Screen.PrimaryScreen?.Bounds.Height}";
            string userName = Environment.UserName;
            string timestamp = DateTime.Now.ToString("yyyy_MMdd_HHmmss");
            return $"{(autoBackup ? "自动备份" : prefix)}_{resolution}_{userName}_{timestamp}";
        }

        public static string CreateDisplayName(string prefix, string resolution)
        {
            return $"{prefix}[{resolution}]";
        }

        public static LayoutFileInfo ParseFileName(string fileName)
        {
            var parts = Path.GetFileNameWithoutExtension(fileName).Split('_');
            if (parts.Length < 6)
                throw new ArgumentException("Invalid file name format");

            string dateString = $"{parts[3]}{parts[4]}{parts[5]}";
            return new LayoutFileInfo(
                parts[0],
                parts[1],
                parts[2],
                DateTime.ParseExact(dateString, "yyyyMMddHHmmss", null)
            );
        }
    }
}
