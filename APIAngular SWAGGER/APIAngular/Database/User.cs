using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace APIAngular.Database
{
    /// <summary>
    /// Сущность пользователь.
    /// </summary>
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "userMail")]
        public string UserMail { get; set; }

        [JsonProperty(PropertyName = "userDateTimeRegister")]
        public DateTime UserDateTimeRegister { get; set; } = DateTime.Now;
        //public virtual ICollection<Bulletin> UserBulletins { get; set; }
    }
}