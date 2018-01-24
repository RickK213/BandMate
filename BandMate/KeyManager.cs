using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BandMate
{
    public class KeyManager
    {
        //member variables

        //properties
        public string SendGridKey { get; set; }
        //constructor
        public KeyManager()
        {
            SetKeys();
        }

        //member methods
        void SetKeys()
        {
            JObject keyObject = JObject.Parse(File.ReadAllText(@"C:\Users\Rick Kippert\Dropbox\_devCodeCamp\Assignments\BandMate\App\BandMate\BandMate\JSON\keys.json"));
            SendGridKey = keyObject.GetValue("SendGridKey").ToString();
        }

    }
}