using LionFire.MultiTyping;

namespace LionFire.Identity
{
    public interface SPersona
    {
        string Name
        {
            get;
        }
        string AccountName
        {
            get;
        }
    }

    public class Persona : MultiType, SPersona
    {
        #region Name

        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
            }
        }
        private string name;

        #endregion

        #region AccountName

        public string AccountName
        {
            get => accountName;
            set => accountName = value;
        }
        private string accountName;

        // TODO - Account property

        #endregion

    }
}
