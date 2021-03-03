using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, 
    IdentityUserClaim<int>,AppUserRole, IdentityUserLogin<int>, 
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {}

       public DbSet<UserLike> Likes { get; set; }

       public DbSet<Message> Messages { get; set; }

       
       protected override void OnModelCreating(ModelBuilder builder) //this will be override from base clas, which is a virtual function
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur =>ur.UserId)
            .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur =>ur.RoleId)
            .IsRequired();

            //primary keys
            builder.Entity<UserLike>().HasKey(k => new {k.SourceUserId, k.LikedUserId}); 
            //One to many relationship
            builder.Entity<UserLike>().HasOne(s => s.SourceUser) // one user
                                      .WithMany(l => l.LikedUsers) // likes other users
                                      .HasForeignKey(s => s.SourceUserId)
                                      .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>().HasOne(s => s.LikedUser) // one user
                                      .WithMany(l => l.LikedByUsers) // is liked by other users
                                      .HasForeignKey(s => s.LikedUserId)
                                      .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Message>().HasOne(u => u.Recipient)
                                     .WithMany(m => m.MessageReceived)
                                     .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>().HasOne(s => s.Sender)
                                     .WithMany(m => m.MessageSent)
                                     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}