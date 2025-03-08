namespace TrackerDeFavorisApi.Models
{
    public class InvalidAppSettingsException : Exception
    {
        public InvalidAppSettingsException() : base() { }
        public InvalidAppSettingsException(string message) : base(message) { }
    }
}
