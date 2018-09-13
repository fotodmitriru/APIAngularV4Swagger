using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using APIAngular.Database;
using Newtonsoft.Json;

namespace APIAngular.Models
{
    public class NewUser
    {
        //[RangeValue(1, ErrorMessage = "Значение поля UserRelation должно быть больше 0")]
        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; } //= Guid.NewGuid();

        [Required(ErrorMessage = "Параметр UserName не может быть пустым и должен содержать не менее 3 и не более 100 символов.")]
        [StringLength(100, MinimumLength = 3)]
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [StringLength(30, MinimumLength = 5)]
        [EmailAddress]
        [JsonProperty(PropertyName = "userMail")]
        public string UserMail { get; set; }
    }
}