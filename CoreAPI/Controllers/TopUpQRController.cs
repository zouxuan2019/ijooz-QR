using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreAPI.Common;
using CoreAPI.Models;
using CoreAPI.Service;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static CoreAPI.Models.NewQRModel;
using static CoreAPI.Models.ProductList;
using static CoreAPI.Models.QrBalanceModel;
using static CoreAPI.Models.TopUpQRModel;
using static CoreAPI.Models.UseHistory;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class TopUpQRController : ControllerBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public TopUpQRController(ILoggerFactory depLoggerFactory)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.QRController");
        }

        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Top Up QR Code",
            Description = "Use eWallet credit to top up existing QR Codes")]
        public async Task<IActionResult> Post([FromBody] TopUpQRReq reqForm)
        {
            _logger.LogInformation("start share QR balance");

            DBModels dbModel = new DBModels();

            //validate TransId
            string transIdResult = await dbModel.checkTransId(reqForm.UserID, reqForm.TransId);
            if (transIdResult != "")
            {
                return Ok(new TopUpQR_Fail() { Msg = transIdResult });
            }

            //get product info based on QR
            List<QrBalance> prodDetail = await dbModel.GetProductDetailFromQR_Status(reqForm.UserID, reqForm.QRCode);

            if (prodDetail.Count == 0)
            {
                return Ok(new TopUpQR_Fail() { Msg = "Invalid QR Code" });
            }

            if (reqForm.TopUpQty <= 0)
            {
                return Ok(new TopUpQR_Fail(){Msg = "Invalid new quantity"});
            }

            //TODO: update promotion
            Double discount = 0;
            if(reqForm.PromotionCode.ToUpper()=="50% OFF")
            {
                discount = 0.5;
            }

            double ttlAmt = System.Math.Round(prodDetail[0].UnitPrice * reqForm.TopUpQty * (1 - discount), 2);//String.Format("{0:0.00}", 123.4567);  -->123.45

            //getToken
            var s = await authService.getToken();
            var data1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(await authService.getToken());
            string token = (data1["access_token"] ?? "").ToString();
            if (token == "")
            {
                return Unauthorized();
            }

            //check balance
            if (!(await eWalletAPI.checkEWalletBalanceAsync(token, reqForm.UserID, ttlAmt)))
            {
                return Ok(new NewQR_Fail() { Msg = "Not enough balance" });
            }

            DateTime transTime = DateTime.Now;
            int ttlQtyAfterTopUp = reqForm.TopUpQty + prodDetail[0].Qty;
            DateTime ExpiryDate = DateTime.Now.AddYears(10);
            if (reqForm.ReqType == "Submit")
            {
                await dbModel.InitTopUpQRCode(reqForm.UserID, reqForm.QRCode, reqForm.TransId, transTime, reqForm.TopUpQty, ttlAmt, reqForm.PromotionCode);

                //deduct balance 
                if (!(await eWalletAPI.deductEWalletBalanceAsync(token, reqForm.UserID, ttlAmt, reqForm.TransId, prodDetail[0].Product, prodDetail[0].Company, reqForm.Remark)))
                {
                    return Ok(new NewQR_Fail() { Msg = "Deduct amount fail" });
                }

                //topup QR (need to get new ttl qty & new expirydate)
                string reqResult = await iJoozAPI.reqQR("TopUp", reqForm.TransId, reqForm.TransTime, "member", reqForm.UserID, reqForm.QRCode, prodDetail[0].Currency, prodDetail[0].UnitPrice, reqForm.TopUpQty, DateTime.Now.AddMonths(24));
                if (reqResult == "")
                {
                    return Ok(new NewQR_Fail() { Msg = "Get QR Code from iJooz Fail" });
                }
                else
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(reqResult);
                    if (data["result"] == "Success")
                    {
                        ttlQtyAfterTopUp = Convert.ToInt16(data["balanceQty"]);
                    }
                    else
                    {
                        return Ok(new NewQR_Fail() { Msg = "Get QR Code from iJooz Fail2" });
                    }
                }

                await dbModel.SaveTopUpQRCode(reqForm.UserID, reqForm.TransTime, reqForm.QRCode, ttlQtyAfterTopUp, ExpiryDate);
            }
            else if (reqForm.ReqType != "Preview")
            {
                return Ok(new TopUpQR_Fail() { Msg = "Invalid Request Type" });
            }

            List<TopUpQrDetail> detailList = new List<TopUpQrDetail> {
                new TopUpQrDetail()
                {
                    TransId=reqForm.TransId,
                    ReqType=reqForm.ReqType,
                    Company=prodDetail[0].Company,
                    Product=prodDetail[0].Product,
                    QRCode=prodDetail[0].QRCode,
                    TopUpQty=reqForm.TopUpQty,
                    TtlQty=ttlQtyAfterTopUp,
                    Currency=prodDetail[0].Currency,
                    Amt=ttlAmt,
                    ExpiryDate=ExpiryDate
                }
            };

            return Ok(new TopUpQR_OK()
            {
                QrDetail = detailList
            });

        }
    }
}