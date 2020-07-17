using LionFire.Dependencies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Identity
{
    public class AccountService
    {
        public static AccountService Current => DependencyContext.Current.GetService<AccountService>();

        public AccountSession AccountSession { get; protected set; }

        public AccountService(AccountSession accountSession)
        {
            AccountSession = accountSession;
        }

        public Task<bool> Login()
        {
            throw new NotImplementedException();
        }
    }
}
