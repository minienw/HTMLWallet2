﻿using Org.BouncyCastle.Crypto;

namespace CheckInQrWeb.Core.Models
{
    public class HttpPostTokenCommandArgs
    {
        public string AccessTokenServiceUrl { get; set; }
        public string InitiatingToken { get; set; }
        
        //Data the wallet has created or read from the validation service identity.
        public AsymmetricKeyParameter WalletPublicKey { get; set; }
    }
}
