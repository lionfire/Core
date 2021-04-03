using System;

namespace LionFire.LiteDb
{
    public class LiteDbSize
    {
        public long Size { get; set; }
        public SizeUnit Unit { get; set; }

        public override string ToString()
        {
            return Size.ToString() + Unit switch
            {
                SizeUnit.Gigabytes => "GB",
                SizeUnit.Megabytes => "MB",
                SizeUnit.Kilobytes => "KB",
                _ => throw new ArgumentException(nameof(Unit)),
            };

        }
    }
}