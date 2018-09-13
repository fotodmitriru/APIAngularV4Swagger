using System;
using System.ComponentModel;
using APIAngular.Database;
using Newtonsoft.Json;

namespace APIAngular.Models
{
    /// <summary>
    /// Модель фильтра для объявлений
    /// </summary>
    public class FilterBulletins
    {
        /// <summary>
        /// Начальный диапазон даты
        /// </summary>
        [JsonProperty(PropertyName = "bullCreateDateTimeBegin")]
        public DateTime BullCreateDateTimeBegin { get; set; } = Convert.ToDateTime("01.01.1900");
        /// <summary>
        /// Конечный диапазон даты
        /// </summary>
        [JsonProperty(PropertyName = "bullCreateDateTimeEnd")]
        public DateTime BullCreateDateTimeEnd { get; set; } = Convert.ToDateTime("01.01.1900");

        /// <summary>
        /// Общий поиск - работает по полям bullTxt, bullCreateDateTime, bullEditDateTime и userName
        /// </summary>
        [JsonProperty(PropertyName = "bullTxt")]
        public string BullTxt { get; set; }

        /// <summary>
        /// Рейтинг объявления. Должен находиться в диапазоне 1 - 10
        /// </summary>
        [RangeValue(1, 10, ErrorMessage = "Значение поля bullRate должно быть в диапазоне 1 - 10")]
        [JsonProperty(PropertyName = "bullRate")]
        public int BullRate { get; set; } = -1;

        /// <summary>
        /// Id пользователя, который опубликовал объявление
        /// </summary>
        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; } = Guid.Empty;

        /// <summary>
        /// Количество объявлений на странице, значение по умолчанию 10
        /// </summary>
        [JsonProperty(PropertyName = "countBulletinsOnPage")]
        public int CountBulletinsOnPage { get; set; } = 10;

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
        public string FieldForSort { get; set; } = "BullId";

        /// <summary>
        /// Сортировка. Поле принимает значения true/false(по умолчанию)
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty(PropertyName = "isDesc")]
        public bool IsDesc { get; set; }
    }
}