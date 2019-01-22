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
    public class VariantsController : ControllerBase
    {
        private readonly IVariantStore _variantStore;

        public VariantsController(IVariantStore variantStore)
        {
            _variantStore = variantStore;
        }

        [HttpGet]
        public async Task<ActionResult<IList<Variant>>> GetList(string productId = null, CancellationToken ct = default)
        {
            var result = !string.IsNullOrEmpty(productId)
                ? await _variantStore.GetAllByProductIdAsync(productId, ct)
                : await _variantStore.GetAllAsync(ct);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Variant>> Get(string id, CancellationToken ct)
        {
            var value = await _variantStore.GetByIdAsync(id, ct);

            if (value == null)
            {
                return NotFound();
            }

            return Ok(value);
        }

        [HttpPost]
        public async Task<ActionResult<Variant>> Post(Variant value, CancellationToken ct)
        {
            await _variantStore.InsertAsync(value, ct);

            return Ok(value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Variant>> Put(string id, Variant value, CancellationToken ct)
        {
            var result = await _variantStore.ReplaceAsync(value, ct);

            if (result.MatchedCount == 0)
            {
                return NotFound();
            }

            return Ok(value);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken ct)
        {
            var result = await _variantStore.DeleteAsync(id, ct);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
