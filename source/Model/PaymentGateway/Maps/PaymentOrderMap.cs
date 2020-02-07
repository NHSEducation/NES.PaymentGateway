using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using PaymentGateway.Model.PaymentGateway.Context;

namespace PaymentGateway.Model.PaymentGateway.Maps
{
    public class PaymentOrderMap : EntityTypeConfiguration<PaymentOrder>
    {
        public PaymentOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & column mappings
            this.ToTable("PaymentOrder", schemaName: "dbo");
            this.Property(t => t.Id).HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.VendorTxCode).HasColumnName("VendorTxCode");
            this.Property(t => t.BookingId).HasColumnName("BookingID");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.CostCentre).HasColumnName("CostCentre");
            this.Property(t => t.AccountCode).HasColumnName("AccountCode");
            this.Property(t => t.ProjectCode).HasColumnName("ProjectCode");
            this.Property(t => t.VPSTxID).HasColumnName("VPSTxID");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.StatusDetail).HasColumnName("StatusDetail");
            this.Property(t => t.ProcessedDate).HasColumnName("ProcessedDate");
        }
    }
}
