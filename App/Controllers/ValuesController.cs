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
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IValueStore _valueStore;

        public ValuesController(IValueStore valueStore)
        {
            _valueStore = valueStore;
        }

        [HttpGet]
        public async Task<ActionResult<IList<Value>>> Get(CancellationToken ct)
        {
            var result = await _valueStore.GetAllAsync(ct);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Value>> Get(string id, CancellationToken ct)
        {
            var value = await _valueStore.GetByIdAsync(id, ct);

            if (value == null)
            {
                return NotFound();
            }

            return Ok(value);
        }

        [HttpPost]
        public async Task<ActionResult<Value>> Post(Value value, CancellationToken ct)
        {
            await _valueStore.InsertAsync(value, ct);

            return Ok(value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Value>> Put(string id, Value value, CancellationToken ct)
        {
            var result = await _valueStore.ReplaceAsync(value, ct);

            if (result.MatchedCount == 0)
            {
                return NotFound();
            }

            return Ok(value);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken ct)
        {
            var result = await _valueStore.DeleteAsync(id, ct);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
