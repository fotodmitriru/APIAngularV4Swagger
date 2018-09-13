using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace APIAngular.Models
{
    /// <summary>
    /// Модель фильтра для пользователей
    /// </summary>
    public class FilterUsers
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [StringLength(100)]
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// mail пользователя
        /// </summary>
        [StringLength(30)]
        [JsonProperty(PropertyName = "userMail")]
        public string UserMail { get; set; }

        /// <summary>
        /// Дата регистрации. Начальный диапазон
        /// </summary>
        [JsonProperty(PropertyName = "userDateTimeRegisterBegin")]
        public DateTime UserDateTimeRegisterBegin { get; set; } = Convert.ToDateTime("01.01.1900");

        /// <summary>
        /// Дата регистрации. Конечный диапазон
        /// </summary>
        [JsonProperty(PropertyName = "userDateTimeRegisterEnd")]
        public DateTime UserDateTimeRegisterEnd { get; set; } = Convert.ToDateTime("01.01.1900");

        /// <summary>
        /// Количество пользователей на странице, значение по умолчанию 10
        /// </summary>
        [JsonProperty(PropertyName = "countUsersOnPage")]
        public int CountUsersOnPage { get; set; } = 10;

        /// <summary>
        /// Номер страницы, значение по умолчанию 0
        /// </summary>
        [DefaultValue(0)]
        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }

        /// <summary>
        /// Имя поля для сортировки
        /// </summary>
        [JsonProperty(PropertyName = "fieldForSort")]
        public string FieldForSort { get; set; } = "UserId";

        /// <summary>
        /// Сортировка. Поле принимает значения true/false(по умолчанию)
        /// </summary>
        [JsonProperty(PropertyName = "isDesc")]
        public bool IsDesc { get; set; }
    }
}