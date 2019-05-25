//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace LionFire.Identity
//{

//    public interface SAccountServer
//    {

//        Guid Login(string username, string password);
//        void Logout(string username, string password);
//    }

//    public class AccountManager
//    {

//        public AccountManager()
//        {
//            passwords.Add("Jared", "Jared123");
//            passwords.Add("MasterServer", "MasterServer123");
//            passwords.Add("TopServer1", "TopServer1123");
//            passwords.Add("TopServer2", "TopServer2123");
//            passwords.Add("UserServer1", "UserServer1123");
//            passwords.Add("UserServer2", "UserServer2123");
//        }

//        Dictionary<string, string> passwords = new Dictionary<string, string>();


//        public Guid? Login(string username, string password)
//        {
//            string storedPassword = passwords.TryGetValue(username);

//            if (storedPassword == null)
//            {
//                return null;
//            }

//            if (storedPassword.Equals(password))
//            {

//            }
//        }

//    }
//}
