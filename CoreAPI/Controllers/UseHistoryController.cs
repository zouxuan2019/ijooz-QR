using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CoreAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static CoreAPI.Models.UseHistory;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class UseHistoryController : ControllerBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public UseHistoryController(ILoggerFactory depLoggerFactory)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.QRController");
        }

        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Usage History",
            Description = "Get last 1/3/12 months QR code usage history data")]
        public async Task<IActionResult> Post([FromBody] UseHistoryReq reqForm)
        {
            _logger.LogInformation("start get use history");

            if (reqForm.UserID == "" || reqForm.Range == "")
            {
                return Ok(new QueryUse_Fail(){Msg = "Invalid Request"});
            }

            if (reqForm.Range != "1" && reqForm.Range != "3" && reqForm.Range != "12")
            {
                return Ok(new QueryUse_Fail() { Msg = "Invalid Range" });
            }

            DBModels dbModel = new DBModels();

            return Ok(new QueryUse_OK() { UseDetail = (await dbModel.GetQRUseDetail(reqForm.UserID, reqForm.Range)) });
        }
    }
}