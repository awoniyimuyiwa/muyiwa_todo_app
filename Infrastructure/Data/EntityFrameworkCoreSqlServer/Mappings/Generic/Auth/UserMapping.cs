using Domain.Generic.Auth;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer.Mappings.Generic.Auth
{
    class UserMapping
    {
        public UserMapping(EntityTypeBuilder<User> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(t => t.Id);

            entityTypeBuilder.Property(t => t.IdFromIdp).IsRequired();
            entityTypeBuilder.Property(t => t.CreatedAt).IsRequired();
            entityTypeBuilder.Property(t => t.UpdatedAt).IsRequired();

            entityTypeBuilder.HasIndex(t => t.IdFromIdp).IsUnique();
        }
    }
}
