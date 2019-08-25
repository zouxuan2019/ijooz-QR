using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreAPI.Models;
using CoreAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CoreAPI.Models.QrBalanceModel;
using static CoreAPI.Models.RegenQRModel;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class RegenQRController : ControllerBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public RegenQRController(ILoggerFactory depLoggerFactory)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.QRController");
        }

        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Generate New QR Code",
            Description = "Generate a new QR Code to replace existing QR code")]
        public async Task<IActionResult> Post([FromBody] RegenQRReq reqForm)
        {
            _logger.LogInformation("start gen QR"); 

            DBModels dbModel = new DBModels();
            
            //validate TransId
            string transIdResult = await dbModel.checkTransId(reqForm.UserID, reqForm.TransId);
            if (transIdResult != "")
            {
                return Ok(new RegenQR_Fail() { Msg = transIdResult });
            }
            List<QrBalance> prodDetail = await dbModel.GetProductDetailFromQR_Status(reqForm.UserID, reqForm.QRCode);

            if (prodDetail.Count == 0)
            {
                return Ok(new RegenQR_Fail() { Msg = "Invalid QR Code" });
            }

            DateTime transTime = DateTime.Now;

            await dbModel.InitRegenQRCode(reqForm.UserID, reqForm.QRCode, reqForm.TransId, transTime);

            //Regen QR (need to get new ttl qty & new expirydate)
            string NewQRCode = "";
            int balanceQty = 0;
            string reqResult = await iJoozAPI.reqQR("Regen", reqForm.TransId, transTime.ToString("yyyy-MM-dd HH:mm:ss"), "member", reqForm.UserID, reqForm.QRCode, prodDetail[0].Currency, prodDetail[0].UnitPrice, 0, transTime.AddMonths(24));
            if (reqResult == "")
            {
                return Ok(new RegenQR_Fail() { Msg = "Get QR Code from iJooz Fail" });
            }
            else
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(reqResult);
                if (data["result"] == "Success")
                {
                    NewQRCode = data["qrCode"];
                    balanceQty = Convert.ToInt16(data["balanceQty"]);
                }
                else
                {
                    return Ok(new RegenQR_Fail() { Msg = "Get QR Code from iJooz Fail2" });
                }
            }

             await dbModel.SaveRegenQRCode(reqForm.UserID, reqForm.TransId, reqForm.QRCode, NewQRCode, balanceQty);

            List<QrDetail> detailList = new List<QrDetail> {
                new QrDetail()
                {
                    TransId=reqForm.TransId,
                    NewQRCode=NewQRCode,
                    TtlQty=balanceQty,
                    ExpiryDate=prodDetail[0].ExpiryDate
                }
            };

            return Ok(new RegenQR_OK()
            {
                QrDetail = detailList
            });
        }
    }
}