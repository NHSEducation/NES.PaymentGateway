using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Common.Helpers;

namespace PaymentGateway.Model.PaymentGateway.Context
{
    public class RegisteredUser : IEntity
    {
        private string _password;

        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [PasswordPropertyText]
        [Required]
        [DisplayName("New Password")]
        public string Password { get; set; }
        //public string Password {
        //    get => _password == null ? null : EncryptionHelper.Decrypt(_password);
        //    set => _password = EncryptionHelper.Encrypt(value);
        //}
        //public string Password
        //{
        //    get => (PasswordStored == null) ? null : EncryptionHelper.Decrypt(PasswordStored);
        //    set => PasswordStored = EncryptionHelper.Encrypt(value);
        //}
        //public virtual string PasswordStored { get; set; }
        public string EmailId { get; set; }
        public DateTime Created { get; set; }
    }
}
