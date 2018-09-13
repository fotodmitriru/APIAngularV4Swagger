using Newtonsoft.Json;

namespace APIAngular.Models
{
    /// <summary>
    /// модель в которую записывается ответ в виде {pages: n, count: n, users:[...]}  и отправляется клиенту
    /// </summary>
    public class ResultContainerOfUsers
    {
        /// <summary>
        /// количество страниц в таблице пользователей
        /// </summary>
        [JsonProperty(PropertyName = "pages")]
        public int CountPages { get; set; }

        /// <summary>
        /// количество пользователей на странице
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int CountUsers { get; set; }

        /// <summary>
        /// список пользователей
        /// </summary>
        [JsonProperty(PropertyName = "users")]
        public object ListUsers { get; set; }
    }
}