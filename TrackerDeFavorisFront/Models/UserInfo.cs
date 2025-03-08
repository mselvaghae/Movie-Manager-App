namespace TrackerDeFavorisFront.Models
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public UserInfo(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
