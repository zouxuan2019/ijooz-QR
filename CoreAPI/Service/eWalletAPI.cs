using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static CoreAPI.Models.eWalletModel;

namespace CoreAPI.Service
{
    public class eWalletAPI
    {
        public static async Task<string> getEWalletBalanceAsync(string token, string user)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);

                try
                {
                    string reqUrl = String.Format(Startup.gateway_eWalletBalance, user);
                    string result = await client.DownloadStringTaskAsync(reqUrl);
                    return result;
                }
                catch (WebException e)
                {
                    // check e.Status as above etc..
                    return "";
                }

            }
        }

        public static async Task<bool> checkEWalletBalanceAsync(string token, string UserID, double reqAmt)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(await eWalletAPI.getEWalletBalanceAsync(token, UserID));
            string strBalance = (data["balance"] ?? "").ToString();
            if (strBalance == "")
            {
                return false;
            }
            double balance = Convert.ToDouble(strBalance);

            if(balance>= reqAmt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> deductEWalletBalanceAsync(string token, string userId, double amount, string transactionId, string product, string company, string comment)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);

                eWalletDeductionReqModel postData = new eWalletDeductionReqModel() { userId = userId, amount = amount, transactionId = transactionId , product = product, company = company , comment = comment };   
                string json = JsonConvert.SerializeObject(postData);
                Uri newUri = new Uri(Startup.gateway_eWalletDedectuion);
   

                try
                {
                    client.UploadStringAsync(newUri, json);                    
                    return true;
                }
                catch (WebException e)
                {
                    // check e.Status as above etc..
                    return false;
                }

            }
        }

    }
}
