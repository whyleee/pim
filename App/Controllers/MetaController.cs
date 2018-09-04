using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Pim.Meta;

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

        [HttpGet]
        public ActionResult<BackendInfo> GetMeta()
        {
            var meta = _metadataProvider.GetBackendInfo(typeof(Backend));

            return Ok(meta);
        }
    }
}
