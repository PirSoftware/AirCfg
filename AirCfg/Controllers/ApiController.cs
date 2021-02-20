using Ist.Pir.AirCfg.Infrastructure;
using Ist.Pir.AirCfg.Models;
using LiteDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ist.Pir.AirCfg.Controllers
{
    
    [ApiController]
    public class ApiController : ControllerBase
    {
        #region Private Fields

        private readonly IEncryption _encryption;
        private readonly string location = "";
        private readonly string startString = "_airCfg_Pirsoft_";

        #endregion Private Fields

        #region Public Methods

        public ApiController(IWebHostEnvironment env, IEncryption encryption)
        {
            _encryption = encryption;
            location = "Filename=" + Path.Combine(env.WebRootPath, "data", "data.db") + ";connection=shared";
        }

        // POST api/<ApiController>
        [HttpPost("api")]
        public string Post([FromBody] SearchModel value)
        {
            using (var db = new LiteDatabase(location))
            {
                var records = db.GetCollection<Entity>("records");
                Entity recordModel = null;
                try
                {
                    recordModel = records.FindById(new BsonValue(new ObjectId(value.Id)));
                }
                catch
                {
                }

                if (recordModel != null)
                {
                    var data = _encryption.DecryptString(recordModel.Data, value.Key);
                    if (!data.StartsWith(startString))
                    {
                        return "Wrong password";
                    }
                    recordModel.LastRequest = DateTime.Now;
                    records.Update(recordModel);
                    return data.Substring(startString.Length).Replace("\0", "");
                }
                else
                {
                    return "Not found";
                }
            }
        }
    }

    #endregion Public Methods
}