namespace TrackerDeFavorisApi.Models
{
    public class UserLogin
    {
        public PublicUser PublicUser { get; set; }
        public string JWT {  get; set; }

        public UserLogin(PublicUser publicUser, string jwt)
        {
            PublicUser = publicUser;
            JWT = jwt;
        }
    }
}
