using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace LionFire.Structures
{
    //public class FlagQuery
    //{
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Recommended usage:
    ///  - Magnitude: between -10 and 10, in increments of 1.  If NaN, assume 10.
    /// </remarks>
    [DataContract]
    public class Flag : INotifyPropertyChanged
    {
        #region Constants

        public const string Separator = ",";
        public const string SeparatorWithSpace = ", ";
        public const char SeparatorChar = ',';

        public const string MagnitudeSeparator = "=";
        public const char MagnitudeSeparatorChar = '=';

        // Flag Filters:
        public const string MultiplierPrefix = "*";
        //public const string CapPrefix = "v|"; // Allows multiple flags to multiply a value, but the multiplier flags are not multiplied against one another; only the bottom multiplier gets used.
        //public const string CeilingPrefix = "^|"; // Allows multiple flags to multiply a value, but the multiplier flags are not multiplied against one another; only the top multiplier gets used.

        #endregion

        #region Properties

        #region Name

        [Ignore]
        public string FlagName { get { return Name; } set { Name = value; } }

        [DataMember]
        [SetOnce]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != null) throw new AlreadySetException();
                if (value == null) return;
                if (value.Contains(SeparatorChar)) throw new ArgumentException("Flag names cannot contain '" + Separator + "'");

                name = value;
                OnPropertyChanged("Name");
            }
        }
        private string name;

        #endregion

        /// <summary>
        /// When used as a fraction, divide the value stored here by 10.
        /// E.g. a Magnitude of 9 should be treated as 90%.
        /// </summary>
        [DefaultValue(double.NaN)]
        [SerializeDefaultValue(false)]
        public double Magnitude
        {
            get
            {
                return magnitude;
            }
            set
            {
                if (value == DefaultMagnitude) value = double.NaN;

                if (magnitude == value) return;
                magnitude = value;
                //Log.Trace("Magnitude: " + magnitude); // TEMP
                OnPropertyChanged("Magnitude");
                OnPropertyChanged("MagnitudeDisplayString");
            }
        }
        private double magnitude = double.NaN;

        public bool IsDefaultMagnitude
        {
            get { return double.IsNaN(Magnitude) || Magnitude == DefaultMagnitude; }
        }

        public static double DefaultMagnitude = 1.0;
        public static double DefaultMagnitudeDisplayMultiplier = 10.0;

        public double EffectiveMagnitude
        {
            get
            {
                double mag = Magnitude;
                if (double.IsNaN(mag)) { mag = DefaultMagnitude; }
                return mag;
            }
        }

        public bool IsSet { get { return EffectiveMagnitude > 0; } }

        public bool IsNegative { get { return EffectiveMagnitude < 0; } }
        public bool IsMultiplier { get; set; }
        public bool IsUpperLimit { get; set; } // UNUSED
        public bool IsLowerLimit { get; set; } // UNUSED

        public string MagnitudeDisplayString
        {
            get
            {
                if (IsDefaultMagnitude) return "";
                var mag = EffectiveMagnitude;
                mag *= DefaultMagnitudeDisplayMultiplier;
                return mag.ToString();
            }
        }
        public void ClearMagnitude()
        {
            Magnitude = double.NaN;
        }

        #endregion

        #region Construction, Implicit operators

        public Flag() { }
        public Flag(string s)
        {
            AssignFrom(s); // Might throw
        }

        public static implicit operator Flag(string s)
        {
            return new Flag(s);
        }

        public static Flag Parse(string str)
        {
            Flag f = new Flag(str);
            return f;
        }
        public static bool TryParse(string str, out Flag flag)
        {
            try
            {
                Flag f = new Flag();
                f.TryAssignFrom(str);
                flag = f;
                return true;
            }
            catch
            {
                flag = null;
                return false;
            }
        }

        #endregion

        #region To / From String

        public override string ToString()
        {
            if (double.IsNaN(Magnitude) || Magnitude == 1.0f)
            {
                return Name;
            }

            return Name + MagnitudeSeparator + Magnitude.ToString();
        }

        public void AssignFrom(string str)
        {
            if (str == null) str = String.Empty;

            var chunks = str.Trim().Split(MagnitudeSeparatorChar);

            if (chunks.Length > 2) throw new ArgumentException("Can contain a maximum of one MagnitudeSeparator: " + MagnitudeSeparator);

            if (chunks.Length <= 0)
            {
                Name = "";
                ClearMagnitude();
            }

            if (chunks.Length >= 2)
            {
                double mag = double.Parse(chunks[1]);
                mag /= DefaultMagnitudeDisplayMultiplier;
                this.Magnitude = mag;
            }
            else
            {
                ClearMagnitude();
            }
            this.Name = chunks[0].Trim();
        }

        /// <summary>
        /// Only fails on failure to parse magnitude as double
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool TryAssignFrom(string str)
        {
            if (str == null) str = String.Empty;

            if (str.StartsWith(MultiplierPrefix))  // FUTURE REFACTOR, add caps
            {
                //Log.Get(typeof(Flag).FullName).LogCritical("Got multiplier: " + str);
                str = str.Substring(MultiplierPrefix.Length);
                IsMultiplier = true;
            }

            var chunks = str.Trim().Split(MagnitudeSeparatorChar);

            if (chunks.Length > 2) throw new ArgumentException("Can contain a maximum of one MagnitudeSeparator: " + MagnitudeSeparator);

            if (chunks.Length <= 0)
            {
                Name = "";
                ClearMagnitude();
            }

            if (chunks.Length >= 2)
            {
                double mag;
                if (!double.TryParse(chunks[1], out mag)) return false;
                this.Magnitude = mag;
            }
            else
            {
                ClearMagnitude();
            }
            this.Name = chunks[0].Trim();

            return true;
        }

        #endregion


        #region Misc

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion


        public void AssignFrom(Flag flag)
        {
            //if (this.Name != flag.Name)
            if (!string.Equals(this.Name, flag.Name, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Name must match");

            this.Magnitude = flag.Magnitude;
        }
    }
}
