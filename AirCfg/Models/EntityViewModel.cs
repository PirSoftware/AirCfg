using System;

namespace Ist.Pir.AirCfg.Models
{
    public class EntityViewModel
    {
        #region Public Properties

        public DateTime CreateTime { get; set; }
        public string Data { get; set; }
        public string EncryptedData { get; set; }
        public string Id { get; set; }
        public bool IsError { get; set; }
        public bool IsNew { get; set; }
        public bool IsSolved { get; set; } = false;
        public string Key { get; set; }
        public DateTime LastRequest { get; set; }
        public string Message { get; set; }
        public string OldKey { get; set; }
        public DateTime UpdateTime { get; set; }

        #endregion Public Properties
    }
}