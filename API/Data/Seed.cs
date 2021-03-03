using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {
           if(await userManager.Users.AnyAsync()) return;

           var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
           var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

           if(users== null) return;
         

         //Create roles for the database 
           var roles = new List<AppRole>()
           {
               new AppRole{ Name = "Member"},
               new AppRole{ Name = "Admin"},
               new AppRole{ Name = "Moderator"}
           };

           foreach (var role in roles)
           {
               await roleManager.CreateAsync(role);
           }

           foreach (var user in users)
           {
               user.UserName = user.UserName.ToLower();

               await userManager.CreateAsync(user,"Pa$$w0rd");
               await userManager.AddToRoleAsync(user,"Member");
           } 
          //create a new user with the name admin
           var admin = new AppUser()
           {
               UserName = "admin"
           };

         //Give the admin user a password
           await userManager.CreateAsync(admin,"Pa$$w0rd");
         //Give the admin user the roles
           await userManager.AddToRolesAsync(admin,new[] {"Admin","Moderator"});
        }
    }
}