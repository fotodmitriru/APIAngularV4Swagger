using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace APIAngular.Database
{
    /// <summary>
    /// Сущность объявление.
    /// </summary>
    public class Bulletin
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        [JsonProperty(PropertyName = "bullId")]
        public Guid BullId { get; set; }

        [JsonProperty(PropertyName = "bullCreateDateTime")]
        public DateTime BullCreateDateTime { get; set; }

        [JsonProperty(PropertyName = "bullEditDateTime")]
        public DateTime BullEditDateTime { get; set; }

        [JsonProperty(PropertyName = "bullTxt")]
        public string BullTxt { get; set; }

        [JsonProperty(PropertyName = "bullRate")]
        public int BullRate { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }
        //public virtual User UserRelation { get; set; }
    }
}