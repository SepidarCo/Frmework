using Microsoft.Extensions.Configuration;
using Sepidar.Framework.Extensions;
using System;
using System.Net;
using System.Net.Mail;

namespace Sepidar.Framework
{
    public class EmailHelper : IEmailHelper
    {
        private IConfiguration configuration;

        public EmailHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void SendVerificationEmail(string to)
        {
            using MailMessage mail = new MailMessage();

            mail.From = new MailAddress(configuration["Email:From"]);
            mail.To.Add(to);
            mail.Subject = configuration["Email:Subject"];
            Uri hostName = new Uri(configuration["Settings:SiteUrl"]);

            var activationLink = "<a href = " + new Uri(hostName, to.Hash()) + "> click here </a>";
            mail.IsBodyHtml = true;
            mail.Body = configuration["Email:Body"] + activationLink;
            using SmtpClient smtp = new SmtpClient(configuration["Email:From"], int.Parse(configuration["Email:SmtpPort"]));
            smtp.Host = configuration["Email:SmtpHost"];
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(configuration["Email:From"], configuration["Email:Password"]);

            smtp.Send(mail);
        }

        public void SendForgetPasswordEmail(string to, string password)
        {
            using MailMessage mail = new MailMessage();

            mail.From = new MailAddress(configuration["Email:From"]);
            mail.To.Add(to);
            mail.Subject = configuration["Email:Subject"];
            mail.IsBodyHtml = true;
            mail.Body = String.Format("Hi {0},<br /><br />Your password is {1}.<br /><br />Thank You.", to, password);
            using SmtpClient smtp = new SmtpClient(configuration["Email:From"], int.Parse(configuration["Email:SmtpPort"]));
            smtp.Host = configuration["Email:SmtpHost"];
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(configuration["Email:From"], configuration["Email:Password"]);

            smtp.Send(mail);
        }

    }
}
