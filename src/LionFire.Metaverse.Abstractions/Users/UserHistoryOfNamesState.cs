namespace LionFire.Metaverse.Users;

public class UserHistoryOfNamesState
{
    public SortedList<DateTime, UserNameChange> Changes { get; set; }
}