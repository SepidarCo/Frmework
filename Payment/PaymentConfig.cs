using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Payment
{
    public class PaymentConfig : Config
    {
        public static string ZarinPalMerchantId
        {
            get
            {
                return GetSetting("ZarinPalMerchantId");
            }
        }

        public static string ZarinPalCallbackUrl
        {
            get
            {
                return GetSetting("ZarinPalCallbackUrl");
            }
        }

    }
}
