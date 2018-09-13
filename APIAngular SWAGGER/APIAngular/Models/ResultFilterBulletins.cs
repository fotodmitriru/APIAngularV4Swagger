using System;
using Newtonsoft.Json;

namespace APIAngular.Models
{
    /// <summary>
    /// Результативная модель, в которую сохраняются данные от фильтра GetListBulletinsByFilter
    /// </summary>
    public class ResultFilterBulletins
    {
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

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
    }
}