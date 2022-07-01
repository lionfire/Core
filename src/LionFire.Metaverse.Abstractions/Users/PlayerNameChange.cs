using System;

namespace LionFire.Metaverse.Users;

public class UserNameChange
{
    public DateTime Date { get; set; }
    public string UserId { get; set; }
    public string FromName { get; set; }
    public string ToName { get; set; }
    public string ChangedBy { get; set; }
    public NameChangeReason ChangeReason { get; set; }
    public string ChangeReasonDetail { get; set; }
    public bool Forced { get; set; }
    public string Comment { get; set; }
}
