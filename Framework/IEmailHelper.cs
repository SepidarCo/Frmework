namespace Sepidar.Framework
{
    public interface IEmailHelper
    {
        void SendVerificationEmail(string to);

        void SendForgetPasswordEmail(string to, string password);
    }
}
