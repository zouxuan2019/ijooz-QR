using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreAPI.Service
{
    public class authService
    {
        public static async Task<string> getToken()
        {

            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                client.QueryString.Add("grant_type", "client_credentials");
                client.QueryString.Add("client_id", "thirdParty");
                client.QueryString.Add("client_secret", "thirdPartySecret");
                client.QueryString.Add("scope", "EWallet");

                try
                {

                    byte[] responsebytes = await client.UploadValuesTaskAsync(Startup.gateway_auth, "POST", client.QueryString);//FromQuery  
                    return System.Text.Encoding.UTF8.GetString(responsebytes);
                }
                catch (WebException e)
                {
                    // check e.Status as above etc..
                    return "";
                }
            }
        }
    }
}
