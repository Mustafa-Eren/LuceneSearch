using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.App;

namespace Lucene.Controllers
{
    [ApiController]
    public class LuceneController : BaseController
    {
        [HttpGet("GetLucene")]
        public async Task<LuceneResponseModel> GetLuceneSearch([FromQuery] LuceneCommand command)
        {
            return await Mediator.Send(command);
            

        }
    }
}