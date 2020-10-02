using Domain.Core;
using Domain.Generic.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.EntityFrameworkCoreSqlServer.Mappings.Core
{
    class TodoItemMapping
    {
        public TodoItemMapping(EntityTypeBuilder<TodoItem> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(t => t.Id);

            entityTypeBuilder.Property(t => t.Name).IsRequired().HasMaxLength(256);
            entityTypeBuilder.Property(t => t.NormalizedName).IsRequired().HasMaxLength(256);
            entityTypeBuilder.Property(t => t.IsCompleted).HasDefaultValue(false);
            entityTypeBuilder.Property(t => t.Slug).IsRequired();
            entityTypeBuilder.Property(t => t.CreatedAt).IsRequired();
            entityTypeBuilder.Property(t => t.UpdatedAt).IsRequired();

            entityTypeBuilder.HasIndex(t => t.NormalizedName).IsUnique();
            entityTypeBuilder.HasIndex(t => t.Slug).IsUnique();

            // M-1 from TodoItem to User
            // entityTypeBuilder.HasOne<User>("User")
            entityTypeBuilder.HasOne(t => t.User)
                .WithMany()
                .IsRequired();
        }
    }
}
