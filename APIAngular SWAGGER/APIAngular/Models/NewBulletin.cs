using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using APIAngular.Database;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace APIAngular.Models
{
    public class NewBulletin
    {
        [JsonProperty(PropertyName = "bullCreateDateTimeBegin")]
        public DateTime BullCreateDateTime { get; } = DateTime.Now;

        [JsonProperty(PropertyName = "bullCreateDateTimeEnd")]
        public DateTime BullEditDateTime { get; } = DateTime.Now;

        [Required(ErrorMessage =
            "Параметр bullTxt не может быть пустым и должен содержать не менее 5 и не более 1000 символов.")]
        [StringLength(1000, MinimumLength = 5)]
        [JsonProperty(PropertyName = "bullTxt")]
        public string BullTxt { get; set; }

        /// <summary>
        /// Рейтинг объявления. Должен находиться в диапазоне 1 - 10
        /// </summary>
        [RangeValue(1, 10, ErrorMessage = "Значение поля bullRate должно быть в диапазоне 1 - 10")]
        [JsonProperty(PropertyName = "bullRate")]
        public int BullRate { get; set; } = 1;

        //[RangeValue(1, ErrorMessage = "Значение поля UserId должно быть больше 0")]
        [Required(ErrorMessage = "Параметр userId не может быть пустым и должен содержать Guid пользователя")]
        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }
    }
}