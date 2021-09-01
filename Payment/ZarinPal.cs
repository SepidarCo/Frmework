using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Sepidar.Payment
{
    public class ZarinPal
    {
        private const string payload = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body><PaymentRequest xmlns=""http://zarinpal.com/"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><MerchantID>{0}</MerchantID><Amount>{1}</Amount><Description>{2}</Description><Email>{3}</Email><Mobile>{4}</Mobile><CallbackURL>{5}</CallbackURL></PaymentRequest></s:Body></s:Envelope>";
        private const string verificationPayload = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body><PaymentVerification xmlns=""http://zarinpal.com/"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><MerchantID>{0}</MerchantID><Authority>{1}</Authority><Amount>{2}</Amount></PaymentVerification></s:Body></s:Envelope>";
        private const string paymentUrl = @"https://www.zarinpal.com/pg/StartPay/{0}";
        private static Regex paymentTokenRequestResponseStatusPattern = new Regex(@"(?<=<ns1:Status>)[^<]*");
        private static Regex paymentTokenRequestResponseAuthorityPattern = new Regex(@"(?<=<ns1:Authority>)[^<]*");
        private static Regex transactionPattern = new Regex(@"(?<=<ns1:RefID>)[^<]*");

        /*
        response sample:
        <?xml version="1.0" encoding="UTF-8"?>
        <SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns1="http://zarinpal.com/"><SOAP-ENV:Body><ns1:PaymentRequestResponse><ns1:Status>100</ns1:Status><ns1:Authority>000000000000000000000000000022496699</ns1:Authority></ns1:PaymentRequestResponse></SOAP-ENV:Body></SOAP-ENV:Envelope>
        */

        /*
        response sample:
        <?xml version="1.0" encoding="UTF-8"?>
        <SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns1="http://zarinpal.com/"><SOAP-ENV:Body><ns1:PaymentVerificationResponse><ns1:Status>100</ns1:Status><ns1:RefID>41100568842</ns1:RefID></ns1:PaymentVerificationResponse></SOAP-ENV:Body></SOAP-ENV:Envelope>
        */

        public static string RequestPaymentToken(decimal amount, string description = null, string email = null, string MobileNumber = null, string callbackUrl = null)
        {
            description = description ?? Guid.NewGuid().ToString();
            callbackUrl = callbackUrl ?? PaymentConfig.ZarinPalCallbackUrl;
            string payloadInstance = payload.Fill(PaymentConfig.ZarinPalMerchantId, amount, description, email, MobileNumber, callbackUrl);
            using (var http = new HttpClient())
            {
                var response = http.PostAsync("https://www.zarinpal.com/pg/services/WebGate/service", new StringContent(payloadInstance)).Result.Content.ReadAsStringAsync().Result;
                var status = paymentTokenRequestResponseStatusPattern.Match(response).Value.ToInt();
                EnsureResponseStatusIsOk(status);
                var authority = paymentTokenRequestResponseAuthorityPattern.Match(response).Value;
                if (authority.Length != 36)
                {
                    throw new FrameworkException("Invalid ZarinPal authority: {0}".Fill(authority));
                }
                return authority;
            }
        }

        private static void EnsureResponseStatusIsOk(int status)
        {
            switch (status)
            {
                case -1:
                    throw new BusinessException("اطلاعات ارسال شده ناقص است");
                case -2:
                    throw new BusinessException("IP و یا مرچنت کد پذیرنده صحیح نیست");
                case -3:
                    throw new BusinessException("با توجه به محدودیت های شاپرک امکان پرداخت با رقم درخواست شده میسر نمی باشد");
                case -4:
                    throw new BusinessException("سطح تأیید پذیرنده پایین تر از سطح نقره ای است");
                case -11:
                    throw new BusinessException("درخواست مورد نظر یافت نشد");
                case -12:
                    throw new BusinessException("امکان ویرایش درخواست میسر نمی باشد");
                case -21:
                    throw new BusinessException("هیچ نوع عملیات مالی برای این تراکنش یافت نشد");
                case -22:
                    throw new BusinessException("تراکنش ناموفق می باشد");
                case -33:
                    throw new BusinessException("رقم تراکنش با رقم پرداخت شده مطابقت ندارد");
                case -34:
                    throw new BusinessException("سقف تقسیم تراکنش از لحاظ تعداد یا رقم عبور نموده است");
                case -40:
                    throw new BusinessException("اجازه دسترسی به متد مربوطه وجود ندارد");
                case -41:
                    throw new BusinessException("اطلاعات ارسال شده مربوط به AdditionalData غیرمعتبر می باشد");
                case -42:
                    throw new BusinessException("مدت زمان معتبر طول عمر شناسه پرداخت باید بین 30 دقیقه تا 45 روز باشد");
                case -54:
                    throw new BusinessException("درخواست مورد نظر آرشیو شده است");
                case 100:
                    return;
                case 101:
                    // علملیات پرداخت موفق بوده و قبلا PaymentVerification تراکنش انجام شده است
                    return;
                default:
                    throw new FrameworkException("Invalid ZarinPal payment token response status");
            }
        }

        public static string GetPaymentUrl(string token)
        {
            return paymentUrl.Fill(long.Parse(token));
        }

        public static string VerifyAndGetTransactionId(string token, decimal amount)
        {
            string payloadInstance = verificationPayload.Fill(PaymentConfig.ZarinPalMerchantId, token, amount);
            using (var http = new HttpClient())
            {
                var response = http.PostAsync("https://www.zarinpal.com/pg/services/WebGate/service", new StringContent(payloadInstance)).Result.Content.ReadAsStringAsync().Result;
                var status = paymentTokenRequestResponseStatusPattern.Match(response).Value.ToInt();
                EnsureResponseStatusIsOk(status);
                var transaction = transactionPattern.Match(response).Value;
                return transaction;
            }
        }
    }
}
