namespace TrackerDeFavorisFront.Models
{
    public class UserUpdate : UserInfo
    {
        public Role Role { get; set; }

        public UserUpdate(string username, string password, Role role) : base(username, password)
        {
            Role = role;
        }   
    }
}
