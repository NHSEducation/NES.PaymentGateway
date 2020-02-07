using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using PaymentGateway.Model.PaymentGateway.Context;

namespace PaymentGateway.Model.PaymentGateway.Maps
{
    public class RegisteredUserMap : EntityTypeConfiguration<RegisteredUser>
    {
        public RegisteredUserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & column mappings
            this.ToTable("RegisterUser", schemaName: "dbo");
            this.Property(t => t.Id).HasColumnName("UserID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Username).HasColumnName("Username");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.EmailId).HasColumnName("EmailID");
            this.Property(t => t.Created).HasColumnName("Created");
        }
    }
}
