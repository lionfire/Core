using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications.Activation
{
    public class ActivationService
    {
        public ActivationResult TryActivate(string code)
        {
            throw new NotImplementedException();

            //string productName = null;
            //int i = 0;
            //if (!key.IsChecksumValid()) return null;
            //foreach (var v in AppKeyCodes.Get.Validators)
            //{
            //    if (v.IsValid(key))
            //    {
            //        productName = v.ProductName;
            //        i++;
            //    }
            //}
            //if (i > 1)
            //{
            //    l.LogCritical("Multiple products match");
            //}
            //if (productName != null)
            //{
            //    if (RegistrationSettings.Instance.Codes.ContainsKey(productName))
            //    {
            //        var existing = RegistrationSettings.Instance.Codes[productName];
            //        if (existing != key)
            //        {
            //            Alerter.Alert("A different key is already registered for '" + productName + "'.  Please contact support if you wish to replace it.");
            //            return null;
            //        }
            //        return productName;
            //    }
            //    else
            //    {
            //        RegistrationSettings.Instance.Codes.Add(productName, key);
            //        RegistrationSettings.VRegistrationSettings.Save();
            //    }
            //}
            //return productName;

        }
    }
}
