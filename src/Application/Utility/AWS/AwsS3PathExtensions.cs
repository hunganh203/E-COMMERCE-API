namespace Application.Utility.AWS
{
    public static class AwsS3PathExtensions
    {
        // upload path
        public static string GenerateAwsS3Key(string id, string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return $"{id}";
            }

            return $"{folder}/{id}";
        }

        // Generate file name
        public static string GenerateFileName(string id, string prefix = "")
        {
            return string.IsNullOrEmpty(prefix) ? $"{id}" : $"{prefix}_{id}";
        }
    }
}