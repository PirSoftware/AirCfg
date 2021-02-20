using Ist.Pir.AirCfg.Infrastructure;
using Ist.Pir.AirCfg.Models;
using LiteDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ist.Pir.AirCfg.Controllers
{
    public class ApplicationController : Controller
    {
        #region Private Fields

        private readonly string startString = "_airCfg_Pirsoft_";
        private readonly string location ="";
        private readonly IEncryption _encryption;
        #endregion Private Fields

        #region Public Methods
        public ApplicationController(IWebHostEnvironment env, IEncryption encryption)
        {
            _encryption = encryption;
            location = "Filename="+ Path.Combine(env.WebRootPath, "data", "data.db") + ";connection=shared";
        }
        public IActionResult Decode([FromBody] EntityViewModel model)
        {
            model.Data = _encryption.DecryptString(model.EncryptedData, model.Key);
            if (model.Data.StartsWith(startString))
            {
                model.Data = model.Data.Substring(startString.Length);
                model.IsSolved = true;
            }
            else
                model.IsSolved = false;
            return Json(model);
        }

        public JsonResult EncodeAndSave([FromBody] EntityViewModel model)
        {
            model.IsError = false;
            model.Data = startString + model.Data;


            if (!model.IsNew && !model.IsSolved)
            {
                model.Message = "Before Password Resolved";
                model.IsError = true;
            }
            else
            {
                using (var db = new LiteDatabase(location))
                {
                    var records = db.GetCollection<Entity>("records");
                    Entity recordModel = null;
                    try
                    {
                        recordModel = records.FindById(new BsonValue(new ObjectId(model.Id)));
                    }
                    catch
                    {

                    }

                    if (recordModel != null)
                    {

                        var data = _encryption.DecryptString(recordModel.Data, model.OldKey);
                        if (!data.StartsWith(startString))
                        {
                            model.Message = "Old Password is Wong";
                            model.IsError = true;
                            return Json(model);
                        }

                        recordModel.UpdateTime = DateTime.Now;
                        recordModel.Data = _encryption.EncryptString(model.Data, model.Key);
                        recordModel.LastRequest = DateTime.Now;
                        records.Update(recordModel);
                        model.Message = "Saved Record...";
                    }
                    else
                    {
                        recordModel = new Entity()
                        {
                            Id = new ObjectId(model.Id),
                            CreateTime = DateTime.Now,
                            Data = _encryption.EncryptString(model.Data, model.Key),
                            LastRequest = DateTime.Now,
                            UpdateTime = DateTime.Now
                        };
                        records.Insert(recordModel);
                        model.Message = "Updated Record...";
                    }
                }
            }
            return Json(model);
        }

        public IActionResult Index()
        {
            EntityViewModel model = new EntityViewModel();
            return View(model);
        }
        [HttpGet("Application/{id}")]
        public IActionResult Index(string id)
        {
            ObjectId objId = null;
            try
            {
                objId = new ObjectId(id);
            }
            catch
            {
                objId = ObjectId.NewObjectId();
                return Redirect("/Application/" + objId.ToString());
            }



            EntityViewModel model = null;
            using (var db = new LiteDatabase(location))
            {
                Entity recordModel = null;
                var records = db.GetCollection<Entity>("records");
                try
                {
                    recordModel = records.FindById(new BsonValue(new ObjectId(id)));
                }
                catch
                {
                }

                if (recordModel != null)
                {
                    model = new EntityViewModel();
                    model.CreateTime = recordModel.CreateTime;
                    model.Data = recordModel.Data;
                    model.EncryptedData = recordModel.Data;
                    model.Id = recordModel.Id.ToString();
                    model.IsNew = false;
                    model.LastRequest = recordModel.LastRequest;
                    model.UpdateTime = recordModel.UpdateTime;
                }
            }
            if (model == null)
            {
                model = new EntityViewModel();
                model.Id = ObjectId.NewObjectId().ToString();
                model.IsNew = true;
            }

            return View(model);
        }

 
        #endregion Public Methods
    }
}