using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Identity;

namespace LionFire.Identity;

// TODO: Wire up to a real Identity service

/// <summary>
/// The login session.  Acquires and holds credentials for as long as the client
/// is logged into the network.
/// </summary>
public class AccountSession
{
    //public static Vob AccountVob
    //{
    //    get
    //    {
    //        throw new NotImplementedException("Use another way");
    //        //if (AccountService.Current.AccountSession == null) return null;
    //        //if (!AccountService.Current.AccountSession.IsLoggedIn) return null;

    //        //// OPTIMIZE ?
    //        //return VosApp.Instance.Accounts[AccountService.Current.AccountSession.Name];
    //    }
    //} //private static Vob accountData;

    //public DateTime SessionConfirmed { get; set; }

    #region Login State

    public bool IsLoggedIn
    {
        get { return SessionId.HasValue; }
    }

    public Guid? SessionId { get; private set; }

    //public Account Account
    //{
    //    get;
    //    set;
    //}

    public string Name { get; set; }
    public string Password { private get; set; }
    public bool SavePassword = true;

    //        public bool IsGuest { get; protected set; }
    //        public string AuthToken { get; protected set; }

    public Persona DefaultPersona
    {
        get;
        set;
    }

    //public string[] PersonaNames
    //{
    //    get
    //    {

    //    }
    //}

    public event EventHandler LoggedIn;
    public event EventHandler LoggedOut;

    public async Task<bool> Login()
    {
        await Task.Delay(0).ConfigureAwait(false);
        try
        {
            if (IsLoggedIn) return true;

            //this.Account = new Account { Name = name };

            //if (success)
            {
                SessionId = Guid.NewGuid(); // TODO: Get this from login server
                RaiseLoggedIn();

                return true;
            }
            //else
            //{
            //    return false;
            //}
        }
        finally
        {
            if (!SavePassword)
            {
                Password = null;
            }
        }
    }

    public void Logout()
    {
        if (!IsLoggedIn) return;

        //Account = null;
        SessionId = null;
        RaiseLoggedOut();
    }

    private void RaiseLoggedIn()
    {
        if (LoggedIn != null)
        {
            LoggedIn(this, null);
        }
    }

    private void RaiseLoggedOut()
    {
        if (LoggedOut != null)
        {
            LoggedOut(this, null);
        }
    }

    #endregion

    

}
