using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoreAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static CoreAPI.Models.QrBalanceModel;
using static CoreAPI.Models.UseHistory;

namespace CoreAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class QrBalanceController : ControllerBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public QrBalanceController(ILoggerFactory depLoggerFactory)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.QRController");
        }

        private string genSignature(string TransId, string TransTime, string PartnerID, string UserID)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"TransId", TransId },
                {"TransTime", TransTime },
                {"PartnerID", PartnerID },
                {"UserID", UserID }
            };

            string prestr = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("", queryParams);
            return Common.Signature_SHA256.Sign(prestr);

        }


        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Get QR Code Balance",
            Description = "Get all QR code with balance qty")]
        public async Task<IActionResult> Post([FromBody] QrBalanceReq reqForm)
        {
            _logger.LogInformation("start get QR balance");

            DBModels dbModel = new DBModels();
            List<QrBalance> detailList = await dbModel.GetQRBalance(reqForm.UserID);

            return Ok(new QrBalance_OK()
            {
                QrBalance = detailList
            });

        }
    }
}
