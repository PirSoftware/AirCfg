using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ist.Pir.AirCfg.Models
{
    public class EntityViewModel
    {

        public DateTime CreateTime { get; set; }
        public string Data { get; set; }
        public string EncryptedData { get; set; }
         public string Id { get; set; }
        public string OldKey { get; set; }
        public string Key { get; set; }
        public DateTime LastRequest { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IsNew { get; set; }
        public bool IsSolved { get; set; } = false;
        public string Message { get; set; }
        public bool  IsError { get; set; }
    }
}
