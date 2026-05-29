using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityBotWPF
{
    class ResponseHandler
    {

        // Delegate for random response selection
        public delegate string ResponseSelector(List<string> responses);

        
        
            private readonly Random _rng = new Random();
            public readonly ResponseSelector PickRandom;

            //Random responses per topic
            public readonly Dictionary<string, List<string>> RandomResponses = new Dictionary<string, List<string>>()
            {
                ["phishing"] = new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Check the sender's email address carefully, phishing emails often use addresses with small typos.",
                "Never click links in unsolicited emails. Go directly to the website by typing the address yourself.",
                "Always verify requests for sensitive information via a phone call to the official number.",
                "Look out for urgency in emails such as 'Act now!' or 'Your account will be closed!' are classic phishing red flags."
            },
                ["scam"] = new List<string>
            {
                "If an offer seems too good to be true, it probably is. Research before you engage.",
                "Scammers often impersonate banks or government agencies. Always verify the caller's identity.",
                "Never send money or gift cards to someone you have not met in person.",
                "Romance scams are rising online. Be wary of anyone who asks for financial help after only chatting online.",
                "Legitimate competitions never ask you to pay to claim a prize."
            },
                ["privacy"] = new List<string>
            {
                "Review the privacy settings on your social media accounts — limit what strangers can see.",
                "Use a VPN on public Wi-Fi to protect your internet traffic from eavesdroppers.",
                "Read app permissions carefully. Does a flashlight app really need access to your contacts?",
                "Delete accounts you no longer use — old accounts still hold your personal data.",
                "Use a private browser window when using shared or public computers."
            },
                ["password"] = new List<string>
            {
                "Use strong, unique passwords for each account. Avoid using personal details like birthdays.",
                "A passphrase — a sequence of random words — is both strong and easy to remember.",
                "Use a password manager like Bitwarden or KeePass to store complex passwords securely.",
                "Never share your password via email or phone — legitimate services will never ask for it.",
                "Change your passwords immediately if you suspect an account has been compromised."
            },
                ["malware"] = new List<string>
            {
                "Keep your operating system and software updated,patches close the holes malware exploits.",
                "Only download software from official, trusted sources. Cracked software often contains malware.",
                "Install a reputable antivirus and schedule regular system scans.",
                "Be careful with USB drives from unknown sources,they can carry malware that runs automatically.",
                "Ransomware encrypts your files and demands payment. Regular backups are your best defence."
            }
            };

            //Single keyword responses
            public readonly Dictionary<string, string> KeywordResponses = new Dictionary<string, string>()
            {
                ["mfa"] = "Multi-Factor Authentication adds an extra layer beyond your password. Enable it on all accounts — especially email and banking.",
                ["two-factor"] = "Two-Factor Authentication requires a second verification step. It dramatically reduces unauthorised access.",
                ["vpn"] = "A VPN encrypts your internet connection. Always use one on public Wi-Fi to prevent hackers intercepting your data.",
                ["firewall"] = "A firewall monitors and filters network traffic. Keep your device firewall enabled at all times.",
                ["encryption"] = "Encryption scrambles data so only authorised parties can read it. Always check for HTTPS before entering sensitive information.",
                ["backup"] = "Follow the 3-2-1 rule: 3 copies of data, on 2 different media, with 1 stored offsite or in the cloud.",
                ["ransomware"] = "Ransomware encrypts your files and demands payment. Never pay — it funds criminals and does not guarantee recovery.",
                ["social engineering"] = "Social engineering manipulates people rather than systems. Always verify identity before sharing sensitive information.",
                ["data breach"] = "Check haveibeenpwned.com to see if your email has been in a breach, and change affected passwords immediately.",
                ["identity theft"] = "Monitor your credit reports, use strong passwords, and enable fraud alerts with your bank.",
                ["https"] = "Always check for 'https://' and the padlock icon before entering login or payment details.",
                ["wifi"] = "Avoid accessing banking or sensitive accounts on public Wi-Fi. Use a VPN or your mobile data instead.",
                ["antivirus"] = "Use a reputable antivirus product, keep it updated, and run regular scans on your device.",
                ["update"] = "Keeping software updated is one of the most effective cybersecurity practices. Updates patch known vulnerabilities.",
                ["zero-day"] = "A zero-day is a flaw unknown to the vendor. Layered security — antivirus, firewall, monitoring — helps detect exploitation.",
                ["cyberbullying"] = "Document the abuse, block the perpetrator, and report it to the platform and law enforcement if serious."
            };

            public ResponseHandler()
            {
                PickRandom = responses => responses[_rng.Next(responses.Count)];
            }

            public string GetResponse(string topic)
            {
                if (RandomResponses.ContainsKey(topic))
                    return PickRandom(RandomResponses[topic]);

                if (KeywordResponses.ContainsKey(topic))
                    return KeywordResponses[topic];

                return "";
            }



        }
}
