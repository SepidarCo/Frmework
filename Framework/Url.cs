using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Framework
{
    public class Url : UriBuilder
    {
        Regex ipRegex = new Regex(@"://(\d*\.){3}\d*(:\d*)?/");

        public Url(string url) : base(url) { }

        public void AddParameter(string key, string value)
        {
            if (Query.IsNothing())
            {
                Query = "{0}={1}".Fill(key, value);
            }
            else
            {
                Query += "&{0}={1}".Fill(key, value);
            }
        }

        public string UrlString
        {
            get
            {
                return Uri.AbsoluteUri;
            }
        }

        public string SchemeHostPort
        {
            get
            {
                string result = "{0}://{1}{2}/".Fill(Scheme, Host, Port == 80 ? "" : ":{0}".Fill(Port));
                return result;
            }
        }

        public string Tld
        {
            get
            {
                if (ipRegex.Match(Host).Success)
                {
                    throw new FrameworkException("IP host does not have TLD");
                }
                var tld = Host.Split('.').Last();
                return tld;
            }
        }

        public string Domain
        {
            get
            {
                if (ipRegex.Match(Host).Success)
                {
                    throw new FrameworkException("IP host does not have domain");
                }
                var domain = Host.Split('.').Reverse().Skip(1).Take(1).Single();
                return domain;
            }
        }

        public string Subdomain
        {
            get
            {
                if (ipRegex.Match(Host).Success)
                {
                    throw new FrameworkException("IP host does not have subdomain");
                }
                var hostParts = Host.Split('.').ToList();
                if (hostParts.Count <= 2)
                {
                    return string.Empty;
                }
                return hostParts.First();
            }
            set
            {
                Host = "{0}.{1}".Fill(value, Host);
                //Host = "{0}.{1}.{2}".Fill(value, Domain, Tld);
            }
        }

        public string ApplySubdomainAndGetSchemeHostPort(string subdomain)
        {
            Subdomain = subdomain;
            return SchemeHostPort;
        }
    }
}
