using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Business.Data;
using App.Models;
using App.Models.Scheme;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetaController : ControllerBase
    {
        private readonly MetadataProvider _metadataProvider;

        public MetaController(MetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        [HttpGet("{itemType}")]
        public ActionResult<ItemTypeInfo> GetTypeInfo(ItemType itemType)
        {
            var type = ResolveType(itemType);
            var meta = _metadataProvider.GetTypeInfo(type);

            return Ok(meta);
        }

        private Type ResolveType(ItemType itemType)
        {
            if (itemType == ItemType.Product)
            {
                return typeof(Product);
            }

            throw new NotSupportedException($"Uknown item type {itemType}");
        }
    }
}
