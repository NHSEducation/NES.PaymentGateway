using System;
using Common.Security;
using PaymentGateway.Util.Session;

namespace PaymentGateway.Util.ViewModel
{
    [Serializable]
    public abstract class ViewModel
    {
        public SessionManager SessionManager { get; set; }
        public User CurrentUser => SessionManager.CurrentUser;
        public string CurrentVersion => SessionManager.CurrentVersion;
    }
}