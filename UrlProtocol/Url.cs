using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UrlProtocol
{
    public class Url
    {
        private string _urlComplete;//complete url comand Ex. UrlProtocol://viewfile.[CompanyName]?f=UDovX1VSTC90ZXN0RG9rdS90ZXN0LnR4dA==
        private string _protocol;//protocol to execute Ex. keor
        private string _domain;//Ex. viewfile.keor
        private string _port;//a port number
        private string _path;// Ex. D:/program/keor
        private string _base64_path;// path with base64 encoded
        private string _parameters;//Ex. f=UDovX1VSTC90ZXN0RG9rdS90ZXN0LnR4dA==
        private string _queryTyp; //for a file: Example: ?f= => f (a file)or ?d= => d (a directory)
        private List<string> _queryEncodedPath; // Example ?f=0RG9rdS90ZXN0LnR4dA= then => 0RG9rdS90ZXN0LnR4dA=
        private string _querySimbol;// Example: '=' '!=' '||' '&&' .. etc.
        private List<string> _queryDecodedPath;// the decoded path from base64

        public string UrlComplete
        {
            get { return _urlComplete; }

            set { _urlComplete = value; }
        }
        public string Protocol
        {
            get { return _protocol; }

            set { _protocol = value; }
        }

        public string Domain
        {
            get { return _domain; }

            set { _domain = value; }
        }

        public string Port
        {
            get { return _port; }

            set { _port = value; }
        }

        public string Path
        {
            get { return _path; }

            set { _path = value; }
        }

        public string Parameters
        {
            get { return _parameters; }

            set { _parameters = value; }
        }

        public string Base64_path
        {
            get { return _base64_path; }

            set { _base64_path = value; }
        }

        public string QueryTyp
        {
            get { return _queryTyp;}

            set {_queryTyp = value;}
        }

        public List<string> QueryEncodedPath
        {
            get {return _queryEncodedPath; }

            set { _queryEncodedPath = value;}
        }

        public string QuerySimbol
        {
            get { return _querySimbol;}

            set {_querySimbol = value;}
        }

        public List<string> QueryDecodedPath
        {
            get { return _queryDecodedPath;}

            set { _queryDecodedPath = value;}
        }

        /*public url(string url, string protocol, string domain, string port, string path, string base64_path, string parameters)
        {
            _url = url;
            _protocol = protocol;
            _domain = domain;
            _port = port;
            _path = path.Substring(1); //to split the first "/" in the path
            _parameters = parameters;
        }*/

        public Url(string urlComplete, string protocol, string domain, string port, string base64_path, string parameters)
        {
            Regex exp = new Regex(@"^(?<querytyp>[a-zA-Z]+)(?<condition>[-!$%^&*()_+|~=`{}\[\]:<>?,.\/]*)(?<encode>.*)");
            var match = exp.Match(parameters);
            List<string> queryEncodedPath = new List<string>();
            List<string> queryDecodedPath = new List<string>();

            _urlComplete = urlComplete;
            _protocol = protocol;
            _domain = domain;
            _port = port;
            _base64_path = base64_path; //email description
            _path = Decoded(base64_path==""  ? String.Empty: base64_path.Substring(1)); //to split the first "/" in the path
            _parameters = parameters;
            _queryTyp = SetParameterValue(match.Groups[1].ToString() == "" ? String.Empty : match.Groups[1].ToString());
            _querySimbol = SetParameterValue(match.Groups[2].ToString() == "" ? String.Empty : match.Groups[2].ToString());


            foreach (var qep in match.Groups[3].ToString().Split(';'))
            {
                queryEncodedPath.Add(SetParameterValue(qep == "" ? String.Empty :qep));
            }
            foreach (var qdp in queryEncodedPath)
            {
                queryDecodedPath.Add(Decoded(qdp == "" ? String.Empty : qdp));
            }
            _queryEncodedPath = queryEncodedPath;
            _queryDecodedPath = queryDecodedPath;
            //_queryEncodedPath = SetParameterValue(match.Groups[3].ToString() == "" ? String.Empty : match.Groups[3].ToString());
            //_queryDecodedPath = Decoded(_queryEncodedPath == "" ? String.Empty : _queryEncodedPath);
        }

        private string Decoded(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            //byte[] data = Encoding.ASCII.GetBytes(encodedString);
            string decodedString = Encoding.UTF8.GetString(data);

            return decodedString;
        }
        private string SetParameterValue(string value)
        {
            return value;
        }

        
    }
}
