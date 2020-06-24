using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApi.Entities;
using WebApi.Helpers;
using System;

namespace WebApi.Services
{
    public interface IDBService
    {
        User GetUser(string Email, string Password);
        User CreateUser(string FirstName, string LastName, string Email, string Company, string Role, string Password);
    }

    public class DBService : IDBService
    {
        private readonly DBSettings _DBSettings;
        private readonly MongoClient Client;
        private readonly IMongoDatabase PoduleDB;
        private readonly IMongoCollection<User> Users;

        public DBService(DBSettings DBSettings)
        {
            try
            {
                _DBSettings = DBSettings;
                this.Client = new MongoClient(_DBSettings.Connection);
                this.PoduleDB = Client.GetDatabase(_DBSettings.DBName);
                this.Users = PoduleDB.GetCollection<User>(_DBSettings.Collection);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating DBService in AuthService: " + e);
            }
        }

        public User GetUser(string Email, string Password) {
            try {
                var EmailFilter = Builders<User>.Filter.Eq("Email", Email);
                var PasswordFilter = Builders<User>.Filter.Eq("Password", Password);
                User user = this.Users.Find(EmailFilter & PasswordFilter).First();
                return user;
            } catch (Exception){
                return null;
            }
        }

        public User CreateUser(string FirstName, string LastName, string Email, string Company, string Role, string Password) {
            var EmailFilter = Builders<User>.Filter.Eq("Email", Email);

            try
            {
                User DBUser = this.Users.Find(EmailFilter).First();
            }
            catch (Exception)
            {
                User user = new User(FirstName, LastName, Email, Company, Role, Password);
                this.Users.InsertOne(user);
                return user;
            }

            return null;
        }
    }
}
