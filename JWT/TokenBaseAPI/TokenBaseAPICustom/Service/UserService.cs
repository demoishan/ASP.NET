using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TokenBaseAPICustom.Models;

namespace TokenBaseAPICustom.Service
{
    // IUserService interface
    public interface IUserService
    {
        User Validate(string email, string password);
        List<User> GetUserList();
        User GetUserById(int id);
        List<User> SearchByName(string name);
    }

    // UserService concrete class
    public class UserService : IUserService
    {

        private List<User> userList = new List<User>();
        public UserService()
        {
            for (var i = 1; i <= 10; i++)
            {
                userList.Add(new User
                {
                    Id = i,
                    Name = $"User {i}",
                    Password = $"pass{i}",
                    Email = $"user{i}@dummy.com",
                    Age = "AAA",
                    Roles = new string[] { i % 2 == 0 ? "Admin" : "User" }
                });
            }
        }

        public User Validate(string email, string password) => userList.FirstOrDefault(x => x.Email == email && x.Password == password);

        public List<User> GetUserList() => userList;

        public User GetUserById(int id) => userList.FirstOrDefault(x => x.Id == id);

        public List<User> SearchByName(string name) => userList.Where(x => x.Name.Contains(name)).ToList();
    }

}