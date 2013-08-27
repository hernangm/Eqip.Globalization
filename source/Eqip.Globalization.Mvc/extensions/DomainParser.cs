using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


internal static class DomainParser
{

    #region Private Members
    private static readonly List<string> COUNTRIES_TLDS = new List<string>(new string[] { "AC", "AD", "AE", "AERO", "AF", "AG", "AI", "AL", "AM", "AN", "AO", "AQ", "AR", "ARPA", "AS", "ASIA", "AT", "AU", "AW", "AX", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BIZ", "BJ", "BM", "BN", "BO", "BR", "BS", "BT", "BV", "BW", "BY", "BZ", "CA", "CAT", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "COM", "COOP", "CR", "CU", "CV", "CX", "CY", "CZ", "DE", "DJ", "DK", "DM", "DO", "DZ", "EC", "EDU", "EE", "EG", "ER", "ES", "ET", "EU", "FI", "FJ", "FK", "FM", "FO", "FR", "GA", "GB", "GD", "GE", "GF", "GG", "GH", "GI", "GL", "GM", "GN", "GOV", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY", "HK", "HM", "HN", "HR", "HT", "HU", "ID", "IE", "IL", "IM", "IN", "INFO", "INT", "IO", "IQ", "IR", "IS", "IT", "JE", "JM", "JO", "JOBS", "JP", "KE", "KG", "KH", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY", "MA", "MC", "MD", "ME", "MG", "MH", "MIL", "MK", "ML", "MM", "MN", "MO", "MOBI", "MP", "MQ", "MR", "MS", "MT", "MU", "MUSEUM", "MV", "MW", "MX", "MY", "MZ", "NA", "NAME", "NC", "NE", "NET", "NF", "NG", "NI", "NL", "NO", "NP", "NR", "NU", "NZ", "OM", "ORG", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PRO", "PS", "PT", "PW", "PY", "QA", "RE", "RO", "RS", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "ST", "SU", "SV", "SY", "SZ", "TC", "TD", "TEL", "TF", "TG", "TH", "TJ", "TK", "TL", "TM", "TN", "TO", "TP", "TR", "TRAVEL", "TT", "TV", "TW", "TZ", "UA", "UG", "UK", "US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU", "WF", "WS", "XN--0ZWM56D", "XN--11B5BS3A9AJ6G", "XN--80AKHBYKNJ4F", "XN--9T4B11YI5A", "XN--DEBA0AD", "XN--FIQS8S", "XN--FIQZ9S", "XN--FZC2C9E2C", "XN--G6W251D", "XN--HGBK6AJ7F53BBA", "XN--HLCJ6AYA9ESC7A", "XN--J6W193G", "XN--JXALPDLP", "XN--KGBECHTV", "XN--KPRW13D", "XN--KPRY57D", "XN--MGBAAM7A8H", "XN--MGBAYH7GPA", "XN--MGBERP4A5D4AR", "XN--O3CW4H", "XN--P1AI", "XN--PGBS0DH", "XN--WGBH1C", "XN--XKC2AL3HYE2A", "XN--YGBI2AMMX", "XN--ZCKZAH", "YE", "YT", "ZA", "ZM", "ZW" });
    #endregion

    #region Methods
    public static DomainParserResults Parse(string host)
    {
        host = Regex.Replace(host, "^http(s)?://", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        List<string> subdomains = new List<string>();
        string domain = string.Empty;
        List<string> topLevelDomains = new List<string>();

        List<string> fragments = host.Split('.').Reverse().ToList();
        if (fragments.Last().Equals("www", StringComparison.InvariantCultureIgnoreCase))
        {
            fragments.Remove(fragments.Last());
        }

        foreach (var f in fragments.ToArray())
        {
            if (isTLD(f) && fragments.Count > 1)
            {
                topLevelDomains.Add(f);
                fragments.Remove(f);
            }
            else
            {
                break;
            }
        }
        domain = fragments.First();
        fragments.RemoveAt(0);

        foreach (var f in fragments)
        {
            subdomains.Add(f);
        }
        subdomains.Reverse();
        return new DomainParserResults(subdomains, domain, topLevelDomains);
    }

    public static bool isTLD(string fragment)
    {
        return COUNTRIES_TLDS.Contains(fragment.Trim(), StringComparer.InvariantCultureIgnoreCase);
    }
    #endregion

}


internal class DomainParserResults
{

    public DomainParserResults(List<string> subdomains, string domain, List<string> topLevelDomains)
    {
        this.Subdomains = subdomains ?? new List<string>();
        this.Domain = domain;
        this.TopLevelDomains = topLevelDomains ?? new List<string>();
    }

    public List<string> Subdomains { get; private set; }
    public string Domain { get; private set; }
    public List<string> TopLevelDomains { get; private set; }

    public string TopLevelDomainsAsString { get { return string.Join(".", this.TopLevelDomains.ToArray().Reverse()); } }
    public bool HasSubdomains { get { return this.Subdomains.Count() > 0; } }
}