﻿using Newtonsoft.Json;
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
        public string StripeSecretKey { get; set; }
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }

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
            StripeSecretKey = keyObject.GetValue("StripeSecretKey").ToString();
            TwilioAccountSid = keyObject.GetValue("TwilioAccountSid").ToString();
            TwilioAuthToken = keyObject.GetValue("TwilioAuthToken").ToString();
        }

    }
}