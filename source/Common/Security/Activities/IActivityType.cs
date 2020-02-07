namespace Common.Security.Activities
{
    public interface IActivityType
    {
        bool IsAuthorised(User user);
        ActivityType Initialise(ActivityType activity);
    }
}
