using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Analytics.Statistics
{
    public interface IStatistic
    {
        void AddValue(dynamic val);
        dynamic Result { get; }
    }

    public class Sum : IStatistic
    {
        private dynamic sum;

        public void AddValue(dynamic val)
        {
            if (sum == null) sum = val;
            else sum += val;
        }

        public dynamic Result {
            get {
                return sum;
            }
        }

    }

    public class MeanAverage : IStatistic
    {
        public dynamic Sum;
        public int Count = 0;

        public void AddValue(dynamic val)
        {
            Count++;
            if (Sum == null) Sum = val;
            else Sum += val;
        }

        public dynamic Result {
            get {
                return Sum;
            }
        }

    }
}
