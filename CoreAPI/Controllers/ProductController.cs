using CoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static CoreAPI.Models.ProductList;

namespace CoreAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ProductListController : ControllerBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public ProductListController(ILoggerFactory depLoggerFactory)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.QRController");
        }

        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Product List",
            Description = "Show all product list")]
        public async Task<IActionResult> Post()
        {
            DBModels dbModel = new DBModels();

            ProductList_OK reqResult = new ProductList_OK()
            {
                ProductDetail = await dbModel.GetAllProductDetail()
            };

            return Ok(reqResult);

        }

        [HttpPost("{Company}")]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation(Summary = "Product List By Company",
            Description = "Show all product list based on selected company")]
        public async Task<IActionResult> Post(string Company)
        {

            DBModels dbModel = new DBModels();
            ProductList_OK reqResult = new ProductList_OK()
            {
                ProductDetail = await dbModel.GetAllProductDetail(Company)
            };

            return Ok(reqResult);

        }

    }
}
