using Newtonsoft.Json;

namespace APIAngular.Models
{
    /// <summary>
    /// модель в которую записывается ответ в виде {pages: n, count: n, bulls:[...]}  и отправляется клиенту
    /// </summary>
    public class ResultContainerOfBulletins
    {
        /// <summary>
        /// количество страниц в таблице объявлений
        /// </summary>
        [JsonProperty(PropertyName = "pages")]
        public int CountPages { get; set; }

        /// <summary>
        /// количество объявлений на странице
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int CountBulls { get; set; }

        /// <summary>
        /// список объявлений
        /// </summary>
        [JsonProperty(PropertyName = "bulls")]
        public object ListBulls { get; set; }
    }
}