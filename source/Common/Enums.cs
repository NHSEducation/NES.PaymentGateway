using System.ComponentModel;

namespace Common
{
    /// <summary>
    /// Represents the different roles for users within the system
    /// </summary>
    public enum AuthorisedRoles
    {
        [Description("")]
        Default = 1,
        [Description("GG Mobs Incidents Restricted")]
        Restricted,
        [Description("GG Mobs Incidents Full")]
        Full
    }

    /// <summary>
    /// Statuses return in response object from SagePay
    /// </summary>
    public enum RegistrationStatus
    {
        ABORT,
        ERROR,
        INVALID,
        MALFORMED,
        NOTAUTHED,
        OK,
        REFUND,
        REJECTED
    }

    public enum PaymentReturnCode
    {
        BookingError,
        WaitingList,
        BookingConfirm,
        PaymentErrorNoParameters,
        PaymentErrorTransactionNotFound,
        PaymentErrorKeyMismatch,
        PaymentSuccess,
        PaymentMobileSuccess,
        PaymentSuccessMessageNotSent,
        PaymentSuccessNotAuthed,
        PaymentSuccessAbort,
        PaymentSuccessRejected,
        PaymentSuccessError,
        PaymentErrorTransactionUpdateError,
        PaymentErrorNotAuthenticated
    }
}
