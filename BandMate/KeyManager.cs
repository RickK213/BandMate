using System;
using System.Collections.Generic;
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

        }
    }
}