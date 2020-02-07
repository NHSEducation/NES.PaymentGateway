using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using PaymentGateway.Model.PaymentGateway.Context;

namespace PaymentGateway.Model.PaymentGateway.Maps
{
    public class RegisteredApplicationMap : EntityTypeConfiguration<RegisteredApplication>
    {
        public RegisteredApplicationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & column mappings
            this.ToTable("RegisterApplication", schemaName: "dbo");
            this.Property(t => t.Id).HasColumnName("ApplicationID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Created).HasColumnName("Created");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
