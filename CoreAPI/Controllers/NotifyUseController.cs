using CoreAPI.Common;
using CoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static CoreAPI.Models.UseDataModel;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyUseController : ControllerBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public NotifyUseController(ILoggerFactory depLoggerFactory)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.QRController");
        }
        private bool checkInput(UseDataReq reqForm)
        {
            Type type = reqForm.GetType();
            // Get all public instance properties. 
            foreach (System.Reflection.PropertyInfo info in type.GetProperties())
            {
                if (info.GetValue(reqForm, null) == null)
                {
                    return false;
                }
            }

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("QRCode", reqForm.QRCode);
            nvc.Add("MachineID", reqForm.MachineID);
            nvc.Add("TransId", reqForm.TransId);
            nvc.Add("TransTime", reqForm.TransTime.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            nvc.Add("balanceQty", reqForm.balanceQty.ToString());
            nvc.Add("ExpiryDate", reqForm.ExpiryDate.ToString("yyyy-MM-dd HH:mm:ss:fff"));

            string prestr = String.Join("&", nvc.AllKeys.Select(a => HttpUtility.UrlEncode(a) + "=" + HttpUtility.UrlEncode(nvc[a]))); //
            string Signature = Signature_SHA256.Sign(prestr);

            if (Signature == reqForm.Signature)
            {
                return true;
            }
            else { return false; }
        }

        // POST api/values
        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Usage Notification",
            Description = "Third party to nofity system to update use history and update QR code balance")]
        public async Task<IActionResult> Post([FromBody] UseDataReq reqForm)
        {
            _logger.LogInformation("start notify use history service");

            if (!checkInput(reqForm))
            {
                return Unauthorized();
            }

            DBModels dbModel = new DBModels();

            await dbModel.SaveUseHistory(reqForm.QRCode, reqForm.MachineID, reqForm.TransId, reqForm.TransTime, reqForm.balanceQty, reqForm.ExpiryDate);

            return Ok(new UseData_OK());

        }
    }
}