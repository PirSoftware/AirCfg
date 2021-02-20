using LiteDB;
using System;

namespace Ist.Pir.AirCfg.Models
{
    public class Entity
    {
        #region Public Properties

        public DateTime CreateTime { get; set; }
        public string Data { get; set; }
        public ObjectId Id { get; set; }
        public DateTime LastRequest { get; set; }
        public DateTime UpdateTime { get; set; }

        #endregion Public Properties
    }
}