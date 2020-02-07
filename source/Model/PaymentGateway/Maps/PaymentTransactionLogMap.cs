using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using PaymentGateway.Model.PaymentGateway.Context;

namespace PaymentGateway.Model.PaymentGateway.Maps
{
    public class PaymentTransactionLogMap : EntityTypeConfiguration<PaymentTransactionLog>
    {
        public PaymentTransactionLogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & column mappings
            this.ToTable("PaymentTransactionLog", schemaName: "dbo");
            this.Property(t => t.Id).HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.VendorTxCode).HasColumnName("VendorTxCode");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Amount).HasColumnType("Money");
            this.Property(t => t.VPSTxID).HasColumnName("VPSTxID");
            this.Property(t => t.RegistrationStatus).HasColumnName("RegistrationStatus");
            this.Property(t => t.RegistrationStatusDetail).HasColumnName("RegistrationStatusDetail");
            this.Property(t => t.RegistrationTime).HasColumnName("RegistrationTime");

            this.Property(t => t.SecurityKey).HasColumnName("SecurityKey");
            this.Property(t => t.AuthorisationStatus).HasColumnName("AuthorisationStatus");
            this.Property(t => t.AuthorisationStatusDetail).HasColumnName("AuthorisationStatusDetail");
            this.Property(t => t.AuthorisationTime).HasColumnName("AuthorisationTime");
            this.Property(t => t.CardType).HasColumnName("CardType");
            this.Property(t => t.LastFourDigits).HasColumnName("LastFourDigits");
        }
    }
}