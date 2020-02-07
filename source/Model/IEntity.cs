namespace PaymentGateway.Model
{
    public interface IEntity
    {
        // Entity Framework automatically maps [Id] to the primary
        // key when creating the database, BUT the Id needs to be
        // of type Integer.
        int Id { get; set; }
    }
}
