using System.Numerics;
using static TrackerDeFavorisApi.Models.User;

namespace TrackerDeFavorisApi.Models;

public enum Role
{
    USER,
    ADMIN
}

public class User
{
    public static string RoleToString(Role role)
    {
        if (role == Role.USER)
        {
            return "User";
        }
        if (role == Role.ADMIN)
        {
            return "Admin";
        }
        return "Undefined";
    }
    public static Role RoleFromString(string str)
    {
        if( str == "Admin" )
        {
            return Role.ADMIN;
        }
        return Role.USER;
    }


    private PublicUser _publicUser;

    public int Id
    {
        get { return _publicUser.Id; }
        set { _publicUser.Id = value; }
    }

    public string Username
    {
        get{ return _publicUser.Username; }
        set { _publicUser.Username = value; }
    }

    public Role Role
    {
        get { return _publicUser.Role; }
        set { _publicUser.Role = value;}
    }

    public string Password { get; set; }
    
    public User(string username, string password, Role role)
    {
        _publicUser = new PublicUser(username, role);
        this.Password = password;
    }
    public User(string username, string password)
    {
        _publicUser = new PublicUser(username);
        this.Password = password;
    }

    public PublicUser AsPublicUser()
    {
        return _publicUser;
    }
}
