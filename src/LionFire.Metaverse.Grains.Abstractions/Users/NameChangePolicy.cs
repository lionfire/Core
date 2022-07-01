using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Metaverse.Users
{
    public class NameChangePolicy
    {
        public TimeSpan? LockUserNamesToOldUsersDuration { get; set; } 
        public TimeSpan? NameChangeCooldown { get; set; }
        public bool FreeChangeToOldLockedName { get; set; }

        public bool UsersCanChangeNames { get; set; }
        public bool CreateUserOnFirstNameChange { get; set; }

        public static NameChangePolicy CreateDefault => new()
        {
            CreateUserOnFirstNameChange = true,
            FreeChangeToOldLockedName = true,
            NameChangeCooldown = TimeSpan.FromDays(180),
            LockUserNamesToOldUsersDuration = TimeSpan.FromDays(365 * 2),
            UsersCanChangeNames = true,
        };

        public static NameChangePolicy CreateTest  => new ()
        {
            CreateUserOnFirstNameChange = true,
            FreeChangeToOldLockedName = true,
            NameChangeCooldown = TimeSpan.FromMinutes(2),
            LockUserNamesToOldUsersDuration = TimeSpan.FromMinutes(10),
            UsersCanChangeNames = true,
        };

    }
}
