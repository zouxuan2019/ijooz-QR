using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPI.Models;
using CoreAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CoreAPI.Models.QrBalanceModel;
using static CoreAPI.Models.ShareQRModel;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ShareQRController : ControllerBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public ShareQRController(ILoggerFactory depLoggerFactory)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.QRController");
        }

        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Share QR Code",
            Description = "Generate a new QR Code to share to others")]
        public async Task<IActionResult> Post([FromBody] ShareQRReq reqForm)
        {
            _logger.LogInformation("start share QR balance");

            DBModels dbModel = new DBModels();

            //validate TransId
            string transIdResult = await dbModel.checkTransId(reqForm.UserID, reqForm.TransId);
            if (transIdResult != "")
            {
                return Ok(new ShareQR_Fail() { Msg = transIdResult });
            }

            //get product info based on QR
            List<QrBalance> prodDetail = await dbModel.GetProductDetailFromQR_Status(reqForm.UserID, reqForm.QRCode);

            if (prodDetail.Count == 0)
            {
                return Ok(new ShareQR_Fail() { Msg = "Invalid QR Code" });
            }

            if (reqForm.SharedQty <= 0)
            {
                return Ok(new ShareQR_Fail() { Msg = "Invalid shared quantity" });
            }
            else if(reqForm.SharedQty> prodDetail[0].Qty)
            {
                return Ok(new ShareQR_Fail() { Msg = "Shared quantity can't be higher than available quantity" });
            }

            DateTime transTime = DateTime.Now;
            int ttlQtyAfterShare = prodDetail[0].Qty - reqForm.SharedQty;
            DateTime ExpiryDate = prodDetail[0].ExpiryDate;

            await dbModel.InitShareQRCode(reqForm.UserID, reqForm.QRCode, reqForm.TransId, transTime, reqForm.SharedQty);

            //Share QR (need to get new ttl qty & new expirydate)
            string SharedQRCode = "";
            string reqResult = await iJoozAPI.reqQR("Share", reqForm.TransId, reqForm.TransTime, "member", reqForm.UserID, reqForm.QRCode, prodDetail[0].Currency, prodDetail[0].UnitPrice, reqForm.SharedQty, DateTime.Now.AddMonths(24));
            if (reqResult == "")
            {
                return Ok(new ShareQR_Fail() { Msg = "Get QR Code from iJooz Fail" });
            }
            else
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(reqResult);
                if (data["result"] == "Success")
                {
                    SharedQRCode = data["qrCode"];
                    ttlQtyAfterShare= Convert.ToInt16(data["balanceQty"]);
                }
                else
                {
                    return Ok(new ShareQR_Fail() { Msg = "Get QR Code from iJooz Fail2" });
                }
            }
            
            await dbModel.SaveShareQRCode(reqForm.UserID, reqForm.TransId, reqForm.QRCode, ttlQtyAfterShare, SharedQRCode, reqForm.SharedQty, reqForm.Remark);

            List<QrDetail> detailList = new List<QrDetail> {
                new QrDetail()
                {
                    TransId=reqForm.TransId,
                    QRCode=prodDetail[0].QRCode,
                    RemainQty=ttlQtyAfterShare,
                    SharedQRCode=SharedQRCode,
                    SharedQty=reqForm.SharedQty,
                    ExpiryDate=prodDetail[0].ExpiryDate,
                    Remark=reqForm.Remark
                }
            };

            return Ok(new ShareQR_OK()
            {
                QrDetail = detailList
            });

        }
    }
}