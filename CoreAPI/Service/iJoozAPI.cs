using CoreAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreAPI.Service
{
    public class iJoozAPI
    {
        public static async Task<string> reqQR(string ReqType, string TransId, string TransTime, string PartnerID, string UserID, string QRCode, string Currency, double UnitPrice, int Qty, DateTime ExpiryDate)
        {
            #region NotUse: HttpWebRequest
            /*
            var queryParams = new Dictionary<string, string>()
            {
                {"TransId", TransId },
                {"TransTime", TransTime },
                {"ReqType", "New" },
                {"PartnerID", PartnerID },
                {"UserID", UserID },
                {"QRCode", QRCode },
                {"Currency", Currency },
                {"UnitPrice", UnitPrice.ToString("0.00")},
                {"Qty", Qty.ToString() },
                {"ExpiryDate", ExpiryDate.ToString("yyyy-MM-dd") }
            };
            string URL = QueryHelpers.AddQueryString("http://47.74.230.157/qr4member/api/Req", queryParams);
            string Signature = Signature_SHA256.Sign(URL);
            URL += "&Signature=" + Signature;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            //request.Headers.Add("Authorization", "Bearer " + CONST.TOKEN);

            Encoding code = Encoding.GetEncoding("utf-8");
            request.Method = "get";
            request.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string strResult;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                strResult = new StreamReader(responseStream).ReadToEnd();
                responseStream.Close();
                return strResult;
            }
            else { return ""; }
            */
            #endregion

            using (WebClient client = new WebClient())
            {
                TransTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                client.QueryString.Add("TransId", TransId);
                client.QueryString.Add("TransTime", TransTime);
                client.QueryString.Add("ReqType", ReqType);
                client.QueryString.Add("PartnerID", PartnerID);
                client.QueryString.Add("UserID", UserID);
                client.QueryString.Add("QRCode", QRCode);
                client.QueryString.Add("Currency", Currency);
                client.QueryString.Add("UnitPrice", UnitPrice.ToString("0.00"));
                client.QueryString.Add("Qty", Qty.ToString());
                client.QueryString.Add("ExpiryDate", ExpiryDate.ToString("yyyy-MM-dd"));

                string prestr = String.Join("&", client.QueryString.AllKeys.Select(a => System.Web.HttpUtility.UrlEncode(a) + "=" + System.Web.HttpUtility.UrlEncode(client.QueryString[a]))); //
                string Signature = Signature_SHA256.Sign(prestr);
                client.QueryString.Add("Signature", Signature);

                try
                {

                    byte[] responsebytes =await client.UploadValuesTaskAsync(Startup.gateway_ReqQR, "POST", client.QueryString);//FromQuery  
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
