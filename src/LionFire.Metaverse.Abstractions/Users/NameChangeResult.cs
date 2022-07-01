
namespace LionFire.Metaverse.Users;

public class NameChangeResult
{
    public NameChangeResult() { }
    public NameChangeResult(string failReason) { IsSuccess = false; FailReason = failReason; }

    public bool IsSuccess { get; set; }
    public string FailReason { get; set; }
    public string ResultId { get; set; }


    public static NameChangeResult Success = new()
    {
        IsSuccess = true,
    };
    public static NameChangeResult UserDoesNotExist = new("UserId does not exist and auto-create policy is disabled");
    public static NameChangeResult UserDoesNotExistAndFailedToCreate = new("User Id does not exist and failed to create due to invalid UserId");
    public static NameChangeResult Already = new("New name is the same as the current name.");
    public static NameChangeResult TooSoon = new("Previous name change was too recent.");
    public static NameChangeResult Unavailable = new("Requested name is already taken.");
    public static NameChangeResult InvalidPrefix = new($"Name cannot start with {UserConstants.UnnamedPrefix}");

}
