namespace TrackerDeFavorisFront.Services
{
    public class ServiceException : Exception
    {
        public ServiceException() : base () { }
        public ServiceException(string message) : base(message) { }
    }
}
