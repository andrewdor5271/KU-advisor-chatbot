using MainApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MainApp.Data
{

    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        // this describes what we actually store in the db
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<AnonUser> AnonUsers => Set<AnonUser>();

        // this is the constraints galore generator
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Message independent fields constraints
            builder.Entity<Message>()
            .Property(t => t.Text)
            .IsRequired();
            builder.Entity<Message>()
            .ToTable(t => t.HasCheckConstraint("CK_Message_Text_Nonempty", GlobalStrings.NonEmptyStringConstraint("Text")));
            builder.Entity<Message>()
            .Property(t => t.SenderType)
            .IsRequired();
            builder.Entity<Message>()
            .Property(t => t.CreationDatetime)
            .IsRequired();


            // Conversation independent fields constraints
            builder.Entity<Conversation>()
            .Property(t => t.Title)
            .IsRequired();
            builder.Entity<Conversation>()
            .ToTable(t => t.HasCheckConstraint("CK_Message_Title_Nonempty", GlobalStrings.NonEmptyStringConstraint("Title")));
            builder.Entity<Conversation>()
            .Property(t => t.CreationDatetime)
            .IsRequired();

            // relation between Conversation and Message
            builder.Entity<Message>()
            .HasOne(t => t.Conversation)
            .WithMany()
            .HasForeignKey(t => t.ConversationId)
            .IsRequired();

            builder.Entity<Conversation>()
            .HasKey(t => t.ConversationId);

            // relation between IdentityUser and Conversation
            builder.Entity<Conversation>()
            .HasOne(t => t.IdentityUser)
            .WithMany()
            .HasForeignKey(t => t.IdentityUserId);

            // relation between AnonUser and Conversation
            builder.Entity<Conversation>()
            .HasOne(t => t.AnonUser)
            .WithMany()
            .HasForeignKey(t => t.AnonUserId);
        }


    }
}