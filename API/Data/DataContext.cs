using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {}
       public DbSet<AppUser> Users {get;set;}

       public DbSet<UserLike> Likes { get; set; }

       public DbSet<Message> Messages { get; set; }

       
       protected override void OnModelCreating(ModelBuilder builder) //this will be override from base clas, which is a virtual function
        {
            base.OnModelCreating(builder);
            
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