namespace Kenc.AbuseIPDB
{
    public enum Category
    {
        Invalid = 0,

        /// <summary>
        /// Altering DNS records resulting in improper redirection.
        /// </summary>
        DNSCompromise = 1,

        /// <summary>
        /// Falsifying domain server cache (cache poisoning).
        /// </summary>
        DNSPoisoning = 2,

        /// <summary>
        /// Fraudulent orders.
        /// </summary>
        FraudOrders = 3,

        /// <summary>
        /// Participating in distributed denial-of-service (usually part of botnet).
        /// </summary>
        DDoSAttack = 4,

        FTPBruteForce = 5,

        /// <summary>
        /// Oversized IP packet.
        /// </summary>
        PingOfDeath = 6,

        /// <summary>
        /// Phishing websites and/or email.
        /// </summary>
        Phishing = 7,

        FraudVoIP = 8,

        /// <summary>
        /// Open proxy, open relay, or Tor exit node.
        /// </summary>
        OpenProxy = 9,

        /// <summary>
        /// Comment/forum spam, HTTP referer spam, or other CMS spam.
        /// </summary>
        WebSpam = 10,

        /// <summary>
        /// Spam email content, infected attachments, and phishing emails.
        /// Note: Limit comments to only relevent information (instead of log dumps) and be sure to remove PII if you want to remain anonymous.
        /// </summary>
        EmailSpam = 11,

        ///   CMS blog comment spam.       
        BlogSpam = 12,

        /// <summary>
        /// Conjunctive category.
        /// </summary>
        VPNIP = 13,

        /// <summary>
        /// Scanning for open ports and vulnerable services.
        /// </summary>
        PortScan = 14,

        Hacking = 15,

        /// <summary>
        /// Attempts at SQL injection.
        /// </summary>
        SqlInjection = 16,

        /// <summary>
        /// Email sender spoofing.
        /// </summary>
        Spoofing = 17,

        /// <summary>
        /// Credential brute-force attacks on webpage logins and services like SSH, FTP, SIP, SMTP, RDP, etc.
        /// This category is seperate from DDoS attacks.
        /// </summary>
        BruteForce = 18,

        /// <summary>
        /// Webpage scraping (for email addresses, content, etc) and crawlers that do not honor robots.txt.
        /// Excessive requests and user agent spoofing can also be reported here.
        /// </summary>
        BadWebBot = 19,

        /// <summary>
        /// Host is likely infected with malware and being used for other attacks or to host malicious content.
        /// The host owner may not be aware of the compromise.
        /// This category is often used in combination with other attack categories.
        /// </summary>
        ExploitedHost = 20,

        /// <summary>
        /// Attempts to probe for or exploit installed web applications such as a CMS like WordPress/Drupal, e-commerce solutions, forum software, phpMyAdmin and various other software plugins/solutions.
        /// </summary>
        WebAppAttack = 21,

        /// <summary>
        /// Secure Shell (SSH) abuse. Use this category in combination with more specific categories.
        /// </summary>
        SSH = 22,

        /// <summary>
        /// Abuse was targeted at an "Internet of Things" type device. Include information about what type of device was targeted in the comments.
        /// </summary>
        IoTTargeted = 23,
    }
}
