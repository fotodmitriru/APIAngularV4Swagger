using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using APIAngular.Database;
using Newtonsoft.Json;

namespace APIAngular.Models
{
    public class DeleteBulletin
    {
        //[RangeValue(1, ErrorMessage = "Значение поля bullId должно быть больше 0")]
        [JsonProperty(PropertyName = "bullId")]
        public Guid BullId { get; set; }

        //[RangeValue(1, ErrorMessage = "Значение поля userId должно быть больше 0")]
        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; } = Guid.Empty;
    }
}