namespace TrackerDeFavorisApi.Models
{
    public class PublicUser
    {
        public Role Role { get; set; }

        public int Id { get; set; }
        public string Username { get; set; }

        public PublicUser(string username, Role role)
        {
            this.Username = username;
            this.Role = role;
        }
        public PublicUser(string username)
        {
            this.Username = username;
            this.Role = Role.USER;
        }
    }
}
