using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using LionFire.Collections;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace LionFire.Structures
{
    // TODO: Make freezable?
    //[JsonConvert(typeof(FlagCollectionJsonExConverter))] TODO TOPORT
    [DataContract]
    public class FlagCollection : MultiBindableCollection<Flag>, INotifyPropertyChanged
    {
        #region Construction

        public FlagCollection()
        {
            this.CollectionChanged += FlagCollection_CollectionChanged;
        }
        public FlagCollection(string flagString) : this()
        {
            this.FlagsString = flagString;
        }

        #endregion

        #region Properties


        #region IsCaseSensitive

        public bool IsCaseSensitive
        {
            get { return isCaseSensitive; }
            set { isCaseSensitive = value; }
        }
        private bool isCaseSensitive = false;

        #endregion

        //[SerializeDefaultValue(false)]
        //[DataMember]
        //public List<Flag> Flags = new List<Flag>();

        [Ignore]
        public string FlagsString
        {
            get
            {
                return this.ToString();
            }
            set
            {
                var newFlags = FlagCollection.Parse(value);
                this.AssignFrom(newFlags);
            }
        }

        public string CanonicalFlagsString
        {
            get
            {
                return GetCanonicalFlagsString(this);
            }
        }
        private static string GetCanonicalFlagsString(FlagCollection flags)
        {
            if (flags == null) return string.Empty;

            var nonZero = flags.Where(f => f.Magnitude != 0).OrderBy(x => x.Name);

            var result = !nonZero.Any() ? string.Empty : nonZero.Select(f => f.ToString()).Aggregate((x, y) => x + Flag.SeparatorWithSpace + y);
            if (!flags.IsCaseSensitive) result = result.ToLowerInvariant();
            //l.Trace("CanonicalFlagsString: " + result);
            return result;
        }

        #endregion

        #region (Static) Implicit Operators

        public static implicit operator FlagCollection(string flagsString)
        {
            return Parse(flagsString);
        }

        #endregion

        //public bool AttachChildEvents

        #region Flag Changed Events

        private void FlagCollection_CollectionChanged(INotifyCollectionChangedEventArgs<Flag> e)
        {
            OnPropertyChanged("FlagsString");

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                l.Error("Not Implemented: reset");
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        item.PropertyChanged += item_PropertyChanged;

                        var ev = FlagChangedFromTo;
                        if (ev != null) ev(null, item);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        item.PropertyChanged -= item_PropertyChanged;
                        var ev = FlagChangedFromTo;
                        if (ev != null) ev(item, null);
                    }
                }
            }
        }


        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("FlagsString");
            var ev = FlagChangedFromTo;
            var flag = sender as Flag;
            if (ev != null) ev(flag, flag);
        }

        public event Action<Flag, Flag> FlagChangedFromTo;

        #endregion

        #region INotifyPropertyChanged Implementation

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        //{

        //    var ev = PropertyChanged;
        //    if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        //}

        #endregion

        #region Set / Get Methods

        public bool HasFlag(string flagName, bool? isSet = null)
        {
            var f = TryGet(flagName);
            if (f == null) return false;
            if (f.EffectiveMagnitude == 0) return false;
            if (isSet.HasValue)
            {
                if (isSet.Value) return f.EffectiveMagnitude > 0;
                else return f.EffectiveMagnitude < 0;
            }
            // else return true whether positive or negative
            return true;
        }

        public Flag TryGetNonZero(string flagName)
        {
            var f = TryGet(flagName);
            if (f == null) return null;
            if (f.Magnitude == 0) return null;
            return f;
        }

        public Flag TryGet(string flagName)
        {
            if (!IsCaseSensitive) { flagName = flagName.ToLowerInvariant(); }

            for (int i = 0; i < this.Count; i++)
            {
                if (IsCaseSensitive)
                {
                    if (this[i].Name == flagName)
                    {
                        return this[i];
                    }
                }
                else
                {
                    if (this[i].Name.ToLowerInvariant() == flagName)
                    {
                        return this[i];
                    }
                }
            }
            return null;
        }

        public Flag Get(string flagName)
        {
            var flag = TryGet(flagName);
            if (flag != null) return flag;

            flag = new Flag(flagName);
            flag.Magnitude = 0;
            SetFlag(flag); // REVIEW: Triggers save event
            return flag;
        }
        public void SetFlag(Flag flag, bool allowReplace = true)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (string.Equals(this[i].Name, flag.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (!allowReplace)
                    {
                        return;
                        //throw new AlreadySetException("flag already set and allowReplace == false");
                    }
                    //flag.Name = this[i].Name;
                    this[i].AssignFrom(flag);
                    return;
                }
            }

            this.Add(flag);
        }

        #endregion

        public void AssignFrom(FlagCollection newFlags)
        {
            var removals = new List<Flag>();
            foreach (var f in this)
            {
                removals.Add(f);
            }

            foreach (var newFlag in newFlags)
            {
                removals.Remove(newFlag);
                this.SetFlag(newFlag);
            }
            foreach (var removal in removals)
            {
                this.Remove(removal);
            }
        }

        #region Parse / ToString

        #region (Static) Parse

        public static FlagCollection Parse(string str)
        {
            FlagCollection newFC;
            if (!TryParse(str, out newFC)) { return null; }

            return newFC;
        }

        public static bool TryParse(string str, out FlagCollection fc)
        {

            var newFC = new FlagCollection();

            if (str != null)
            {
                var chunks = str.Split(new char[] { Flag.SeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var chunk in chunks)
                {
                    Flag flag;
                    if (!Flag.TryParse(chunk, out flag)) { fc = null; return false; }
                    newFC.Add(flag);
                }
            }
            fc = newFC;
            return true;
        }

        #endregion

        // FUTURE: Sort flags so this is canonical?
        public override string ToString()
        {
            var nonZero = this.Where(f => f.Magnitude != 0);

            return !nonZero.Any() ? "" : nonZero.Select(f => f.ToString()).Aggregate((x, y) => x + Flag.SeparatorWithSpace + y);
        }

        public override bool Equals(object obj)
        {
            FlagCollection other = obj as FlagCollection;
            if (other == null) return false;
            return this.CanonicalFlagsString == other.CanonicalFlagsString;
        }
        public static bool Equals(FlagCollection left, FlagCollection right)
        {
            if (left == null && right == null) return true;
            var l = GetCanonicalFlagsString(left);
            var r = GetCanonicalFlagsString(right);
            return l.Equals(r);
        }

        public override int GetHashCode()
        {
            return CanonicalFlagsString.GetHashCode();
        }

        #endregion

        #region Misc


        private static readonly ILogger l = Log.Get();

        #endregion


        public bool PassesFilter(FlagCollection filter)
        {
            if (filter == null) return true;
            foreach (var negFlag in filter.Where(f => f.IsNegative))
            {
                if (this.HasFlag(negFlag.Name, true))
                {
                    l.Trace("[filter] " + this + " Matches rejected flag: " + negFlag);
                    return false;
                }
            }
            foreach (var posFlag in filter.Where(f => f.IsSet))
            {
                if (!this.HasFlag(posFlag.Name, true))
                {
                    //l.Trace("[filter] " + this + " Does not have required flag: " + posFlag);
                    return false;
                }
            }
            //l.Debug("[FILTER] "+this.ToString()+" passes filter: " + filter.FlagsString);
            return true;
        }

        public double GetMultiplier(FlagCollection filter)
        {
            var multipliers = new List<double>();
            double upperLimit = double.PositiveInfinity;
            double lowerLimit = double.NegativeInfinity;


            if (filter == null) return 1.0;

            foreach (var negFlag in filter.Where(f => f.IsNegative))
            {
                if (this.HasFlag(negFlag.Name, true))
                {
                    if (negFlag.IsMultiplier)
                    {
                        multipliers.Add(-negFlag.EffectiveMagnitude);
                    }
                    else
                    {
                        //l.Trace("[filter] " + this + " Matches rejected flag: " + negFlag);
                        return 0.0;
                    }
                }
            }
            foreach (var posFlag in filter.Where(f => f.IsSet))
            {
                if (!this.HasFlag(posFlag.Name, true))
                {
                    //l.Trace("[filter] " + this + " Does not have required flag: " + posFlag);
                    return 0;
                }
                else if (posFlag.IsMultiplier)
                {
                    if (posFlag.IsUpperLimit)
                    {
                        upperLimit = Math.Min(posFlag.EffectiveMagnitude, upperLimit);
                    }
                    else if (posFlag.IsLowerLimit)
                    {
                        lowerLimit = Math.Max(posFlag.EffectiveMagnitude, lowerLimit);
                    }
                    else
                    {
                        multipliers.Add(posFlag.EffectiveMagnitude);
                    }
                }
            }

            double multiplier = multipliers.Count == 0 ? 1.0 : multipliers.Aggregate((x, y) => x * y);
            if (multiplier != 1.0)
            {
                l.LogCritical("[FILTER UNTESTED] " + this.ToString() + " multiplier: " + filter.FlagsString + " " + multiplier);
            }
            return multiplier;
        }


    }
}
