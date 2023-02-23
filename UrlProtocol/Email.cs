using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*This class save almost all that a Email have to. 
 * the variable to save are the emails adress as To, CC and BCC with Encod Base 64 and decoded type, also the subject, body and files
 */

namespace UrlProtocol
{
    class Email
    {
        private List<string> _emailAdresseToBase64;
        private List<string> _emailAdresseToDecoded;
        private List<string> _emailAdresseCcBase64;
        private List<string> _emailAdresseCcDecoded;
        private List<string> _emailAdresseBccBase64;
        private List<string> _emailAdresseBccDecoded;
        private string _subject;
        private string _body;
        private List<string> _attachments;
        private string _signature;

        public List<string> EmailAdresseToBase64
        {
            get {return _emailAdresseToBase64; }

            set { _emailAdresseToBase64 = value;}
        }

        public string Subject
        {
            get {return _subject;}

            set { _subject = value; }
        }

        public string Body
        {
            get { return _body; }

            set { _body = value;}
        }

        public List<string> Attachments
        {
            get {return _attachments; }

            set { _attachments = value;}
        }

        public List<string> EmailAdresseToDecoded
        {
            get { return _emailAdresseToDecoded; }

            set { _emailAdresseToDecoded = value; }
        }

        public List<string> EmailAdresseCcBase64
        {
            get {return _emailAdresseCcBase64; }

            set { _emailAdresseCcBase64 = value;}
        }

        public List<string> EmailAdresseCcDecoded
        {
            get { return _emailAdresseCcDecoded; }

            set { _emailAdresseCcDecoded = value;}
        }

        public List<string> EmailAdresseBccBase64
        {
            get { return _emailAdresseBccBase64; }

            set { _emailAdresseBccBase64 = value; }
        }

        public List<string> EmailAdresseBccDecoded
        {
            get { return _emailAdresseBccDecoded;}

            set { _emailAdresseBccDecoded = value; }
        }

        public string Signature
        {
            get {return _signature;}

            set{ _signature = value;}
        }

        public Email(List<string> emailAdresseToBase64, List<string> emailAdresseCcBase64, List<string> emailAdresseBccBase64, string subject, string body, List<string> attachments, string signature)
        {
            _emailAdresseToBase64 = emailAdresseToBase64;
            _emailAdresseToDecoded = DecodedEmails(emailAdresseToBase64);

            _emailAdresseCcBase64 = emailAdresseCcBase64;
            _emailAdresseCcDecoded = DecodedEmails(emailAdresseCcBase64);

            _emailAdresseBccBase64 = emailAdresseBccBase64;
            _emailAdresseBccDecoded = DecodedEmails(emailAdresseBccBase64);

            _subject = subject;
            _body = body;
            _attachments = attachments;
            _signature = signature == String.Empty ? String.Empty : signature;
        }

        private List<string> DecodedEmails(List<string> encodedString)
        {
            List<string> emails = new List<string>();
            try
            {
                foreach (var email in encodedString)
                {
                    byte[] data = Convert.FromBase64String(email);
                    string decodedString = Encoding.UTF8.GetString(data);
                    emails.Add(decodedString);
                }

                return emails;
            }
            catch
            {
                Console.WriteLine("Bitte, Emails müssen mit Base64 verschlüsselt werden...");
                throw;
            }
            
        }

    }
}
