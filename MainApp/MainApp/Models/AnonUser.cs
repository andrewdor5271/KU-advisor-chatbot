using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace MainApp.Models
{   
    public class AnonUser
    {
        public int AnonUserId { get; set; }
        public String PublicToken { get; set; } // cookie security thingy

        public DateTime CreationDatetime { get; set; }

        public DateTime LastChangeDatetime { get; set; }

        public AnonUser()
        {
            this.PublicToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
    public class AnonUserCookieModel
    {
        public int AnonUserId { get; set; }
        public required String PublicToken { get; set; }

        [SetsRequiredMembers]
        public AnonUserCookieModel(AnonUser user)
        {
            this.AnonUserId = user.AnonUserId;
            this.PublicToken = user.PublicToken;
        }

        [JsonConstructor]
        public AnonUserCookieModel(int anonUserId, String publicToken)
        {
            this.AnonUserId = anonUserId;
            this.PublicToken = publicToken;
        }
    }
}

