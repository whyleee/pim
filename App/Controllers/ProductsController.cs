using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Business.Data;
using App.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductStore _productStore;

        public ProductsController(IProductStore productStore)
        {
            _productStore = productStore;
        }

        [HttpGet]
        public async Task<ActionResult<IList<Product>>> Get(CancellationToken ct)
        {
            var result = await _productStore.GetAllAsync(ct);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(string id, CancellationToken ct)
        {
            var value = await _productStore.GetByIdAsync(id, ct);

            if (value == null)
            {
                return NotFound();
            }

            return Ok(value);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product value, CancellationToken ct)
        {
            await _productStore.InsertAsync(value, ct);

            return Ok(value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Put(string id, Product value, CancellationToken ct)
        {
            var result = await _productStore.ReplaceAsync(value, ct);

            if (result.MatchedCount == 0)
            {
                return NotFound();
            }

            return Ok(value);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken ct)
        {
            var result = await _productStore.DeleteAsync(id, ct);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
