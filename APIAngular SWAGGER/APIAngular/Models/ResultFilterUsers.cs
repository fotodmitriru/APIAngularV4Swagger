using System;
using Newtonsoft.Json;

namespace APIAngular.Models
{
    /// <summary>
    /// Результрующая модель поиска для пользователей
    /// </summary>
    public class ResultFilterUsers
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// mail пользователя
        /// </summary>
        [JsonProperty(PropertyName = "userMail")]
        public string UserMail { get; set; }

        /// <summary>
        /// Дата регистрации пользователя
        /// </summary>
        [JsonProperty(PropertyName = "userDateTimeRegister")]
        public DateTime UserDateTimeRegister { get; set; } = DateTime.Now;

        /// <summary>
        /// Количество объявлений, поданных пользователем
        /// </summary>
        [JsonProperty(PropertyName = "countBulletins")]
        public int CountBulletins { get; set; }
    }
}