using System;

namespace LionFire.Applications.Activation
{
    public class ActivationResult
    {
        bool IsSuccess { get; set; }
        public string ProductName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime StartDate { get; set; }
    }
}
