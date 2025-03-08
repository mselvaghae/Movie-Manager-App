namespace TrackerDeFavorisFront.Models
{
    public class PublicUser
    {
        public Role Role { get; set; }

        // propriétés (= getters/setters propres en C#)
        public int Id { get; set; }
        public string Username { get; set; }


        // Constructeurs
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
        public PublicUser() : this("") { ; }


    }
}
