using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace WebApi.Entities
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string Token { get; set; }

        public User(string FirstName, string LastName, string Email, string Company, string Role, string Password)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Company = Company;
            this.Role = Role;
            this.Password = Password;
        }
    }
}