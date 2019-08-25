using CoreAPI.Models;
using CoreAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CoreAPI.Models.NewQRModel;
using static CoreAPI.Models.ProductList;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NewQRController : ControllerBase
    {

        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public NewQRController(ILoggerFactory depLoggerFactory)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.QRController");
        }
        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Buy New QR Code",
            Description = "Use credit from eWallet to buy new QR Code")]
        public async Task<IActionResult> Post([FromBody] NewQRReq reqForm)
        {
            _logger.LogInformation("start request new QR service");
            DBModels dbModel = new DBModels();
            //validate TransId

            string transIdResult = await dbModel.checkTransId(reqForm.UserID, reqForm.TransId);
            if (transIdResult !="")
            {
                return Ok(new NewQR_Fail() { Msg = transIdResult });
            }

            //TODO: Check promotion code
            List<ProductDetail> prodDetail = await dbModel.GetProductDetailFromProductList(reqForm.Company, reqForm.Product);

            if (prodDetail.Count == 0)
            {
                return Ok(new NewQR_Fail() { Msg = "Invalid Company or/and Product" });
            }

            if (reqForm.NewQty <= 0)
            {
                return Ok(new NewQR_Fail(){Msg = "Invalid new quantity"});
            }

            Double discount = 0;
            if(reqForm.PromotionCode.ToUpper()=="50% OFF")
            {
                discount = 0.5;
            }

            double ttlAmt = System.Math.Round(prodDetail[0].UnitPrice * reqForm.NewQty * (1 - discount), 2);//String.Format("{0:0.00}", 123.4567);  -->123.45
            DateTime transTime = DateTime.Now;

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

            string newQR = "";
            if (reqForm.ReqType == "Submit")
            {
                //deduct balance 
                if (!(await eWalletAPI.deductEWalletBalanceAsync(token, reqForm.UserID, ttlAmt, reqForm.TransId, reqForm.Product, reqForm.Company, reqForm.Remark)))
                {
                    return Ok(new NewQR_Fail() { Msg = "Deduct amount fail" });
                }

                await dbModel.InitNewQRCode(reqForm.UserID, reqForm.TransId, transTime, reqForm.NewQty, ttlAmt, reqForm.PromotionCode);
                
                //get new QR
                string reqResult = await iJoozAPI.reqQR("New", reqForm.TransId, reqForm.TransTime,  "member", reqForm.UserID, "-", prodDetail[0].Currency, prodDetail[0].UnitPrice, reqForm.NewQty, DateTime.Now.AddMonths(24));
                if (reqResult == "")
                {
                    return Ok(new NewQR_Fail() { Msg = "Get QR Code from iJooz Fail" });
                }
                else
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(reqResult);
                    if (data["result"] == "Success")
                    {
                        newQR = data["qrCode"];
                    }
                    else
                    {
                        return Ok(new NewQR_Fail() { Msg = "Get QR Code from iJooz Fail2" });
                    }
                }

                await dbModel.SaveNewQRCode(reqForm.UserID, reqForm.TransId, newQR, reqForm.Company, reqForm.NewQty, reqForm.Product, prodDetail[0].Currency, prodDetail[0].UnitPrice, DateTime.Now.AddYears(10));
            }
            else if (reqForm.ReqType != "Preview")
            {
                return Ok(new NewQR_Fail() { Msg = "Invalid Request Type" });
            }

            List<NewQRDetail> detailList = new List<NewQRDetail> {
                new NewQRDetail()
                {
                    TransId=reqForm.TransId,
                    ReqType=reqForm.ReqType,
                    Company=reqForm.Company,
                    Product=reqForm.Product,
                    QRCode=newQR,
                    TtlQty=reqForm.NewQty,
                    Currency=prodDetail[0].Currency,
                    Amt=ttlAmt,
                    ExpiredDate=DateTime.Now.AddYears(10)
                }
            };

            return Ok(new NewQR_OK()
            {
                QrDetail = detailList
            });

        }
    }
}