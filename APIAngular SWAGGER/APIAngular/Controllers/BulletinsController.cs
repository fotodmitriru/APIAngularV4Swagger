using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using APIAngular.Database;
using APIAngular.Models;
using System.Data.SqlClient;

namespace APIAngular.Controllers
{
    /// <summary>
    /// Управление объявлениями.
    /// </summary>
    public class BulletinsController : ApiController
    {
        private DBManager db = new DBManager();
        private DBManager dbTemp = new DBManager();

        // GET
        /// <summary>
        /// Возвращает список всех объявлений
        /// </summary>
        /// <param name="page">если параметр > 0, то список объявлений будет разбит на страницы, вернёт старницу под номером page</param>
        /// <param name="countBullsOnPage">задаёт количество объявлений на странице, по умолчанию значение 10</param>
        /// <param name="getOnlyCount">если параметр true, то в качестве ответа будет количество объявлений</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Bulletins/GetListBulletins/{page?}/{countBullsOnPage?}")]
        [ResponseType(typeof(List<Bulletin>))]
        public IHttpActionResult GetListBulletins(string page = "0", string countBullsOnPage = "10", bool getOnlyCount = false)
        {
            int npage;
            int ncountBullsOnPage;
            try
            {
                npage = Convert.ToInt32(page);
                ncountBullsOnPage = Convert.ToInt32(countBullsOnPage);
            }
            catch (FormatException)
            {
                return Ok(db.Bulletins);
            }

            if (db.Bulletins.Any())
            {
                if (getOnlyCount)
                    return Ok(db.Bulletins.Count());

                if (npage != 0)
                    return Ok(db.Bulletins.OrderBy(x => x.BullId).Skip(npage * ncountBullsOnPage - ncountBullsOnPage)
                        .Take(ncountBullsOnPage));
            }

            return Ok(db.Bulletins);
            //return NotFound();
        }

        // GET
        /// <summary>
        /// Возвращает объявление по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(Bulletin))]
        public async Task<IHttpActionResult> GetBulletinById(Guid id)
        {
            Bulletin bulletin = await db.Bulletins.FindAsync(id);
            /*if (bulletin == null)
                return NotFound();*/

            return Ok(bulletin);
        }

        //GET
        /// <summary>
        /// Возвращает список всех объявлений пользователя по его Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getOnlyCount">если параметр true, то в качестве ответа будет количество объявлений</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(List<Bulletin>))]
        public IHttpActionResult GetListBulletinsByUserId(Guid id, bool getOnlyCount = false)
        {
            IEnumerable<Bulletin> bulls = db.Bulletins.Where(bulletin => bulletin.UserId == id);
            if (getOnlyCount)
                return Ok(bulls.Count());
            //if (bulls.Any())
            return Ok(bulls);

            //return NotFound();
        }

        //POST
        /// <summary>
        /// Возвращает список объявлений по заданному фильтру
        /// </summary>
        /// <param name="filterBulletins">
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(List<ResultFilterBulletins>))]
        public IHttpActionResult GetListBulletinsByFilter(FilterBulletins filterBulletins)
        {
            if (filterBulletins == null)
            {
                return Content(HttpStatusCode.NotFound, "Должен быть указан хотя бы один входной параметр!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string sql = "WITH resultTable AS (SELECT row_number() OVER (ORDER BY " +
                         ((filterBulletins.FieldForSort.ToLower() == ("UserId").ToLower()) ? "u.UserId" : filterBulletins.FieldForSort) +
                         ((filterBulletins.IsDesc) ? " desc" : " asc") +
                         ") as rowNumber, b.BullId, b.BullCreateDateTime, b.BullEditDateTime, b.BullTxt, b.BullRate, u.UserId, u.UserName " +
                         "FROM Bulletins AS b LEFT JOIN Users AS u ON (b.UserId = u.UserId) Where (BullId is not null)";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            string sqlCondition = "";
            string sqlConditionOfCount = "";

            if (filterBulletins.BullCreateDateTimeBegin != Convert.ToDateTime("01.01.1900") &
                filterBulletins.BullCreateDateTimeEnd != Convert.ToDateTime("01.01.1900"))
            {
                sqlCondition += " and (BullCreateDateTime between @pBullCreateDateTimeBegin and @pBullCreateDateTimeEnd)";
                sqlParameters.Add(
                    new SqlParameter("@pBullCreateDateTimeBegin", filterBulletins.BullCreateDateTimeBegin));
                sqlParameters.Add(new SqlParameter("@pBullCreateDateTimeEnd", filterBulletins.BullCreateDateTimeEnd));
            }

            if (filterBulletins.BullRate != -1)
            {
                sqlCondition += " and (BullRate = @pBullRate)";
                sqlParameters.Add(new SqlParameter("@pBullRate", filterBulletins.BullRate));
            }

            if (filterBulletins.BullTxt != null)
            {
                sqlCondition += " and ((BullTxt Like @pBullTxt)";
                sqlCondition += " or (BullCreateDateTime Like @pBullTxt)";
                sqlCondition += " or (BullEditDateTime Like @pBullTxt)";
                sqlCondition += " or (UserName Like @pBullTxt))";
                sqlParameters.Add(new SqlParameter("@pBullTxt", "%" + filterBulletins.BullTxt + "%"));
            }

            if (filterBulletins.UserId != Guid.Empty)
            {
                sqlCondition += " and (UserId = @pUserId)";
                sqlParameters.Add(new SqlParameter("@pUserId", filterBulletins.UserId));
            }

            sqlConditionOfCount = sqlCondition;
            string sqlConditionPage = "";
            if (filterBulletins.Page != 0 & filterBulletins.CountBulletinsOnPage != 0)
            {
                sqlConditionPage += " WHERE (rowNumber BETWEEN @pPage AND @pCountBulletinsOnPage)";
                sqlParameters.Add(new SqlParameter("@pPage",
                    filterBulletins.Page * filterBulletins.CountBulletinsOnPage - filterBulletins.CountBulletinsOnPage +
                    1));
                sqlParameters.Add(new SqlParameter("@pCountBulletinsOnPage",
                    filterBulletins.Page * filterBulletins.CountBulletinsOnPage));
            }

            //string sqlSorted = " ORDER BY " + filterBulletins.FieldForSort + " " + ((filterBulletins.IsDesc) ? "desc" : "asc");

            sql += sqlCondition + ") SELECT * FROM resultTable" + sqlConditionPage;

            List<ResultFilterBulletins> resultFilterBulletins = null;
            try
            {
                resultFilterBulletins = db.Database
                    .SqlQuery<ResultFilterBulletins>(sql, sqlParameters.ToArray<SqlParameter>()).ToList();
            }
            catch (SqlException)
            {
                return BadRequest("Ошибка запроса, проверьте входные параметры !");
            }

            int bulletinsCount = GetCountBullsWithCondition(sqlConditionOfCount, sqlParameters);
            int pagesCount = 0;
            if (bulletinsCount > 0)
            {
                pagesCount = (bulletinsCount > filterBulletins.CountBulletinsOnPage)
                             ? bulletinsCount / filterBulletins.CountBulletinsOnPage
                                + ((bulletinsCount % filterBulletins.CountBulletinsOnPage > 0) ? 1 : 0)
                             : 1;
            }

            ResultContainerOfBulletins resultContBulls = new ResultContainerOfBulletins
            {
                CountPages = pagesCount,
                CountBulls = bulletinsCount
            };

            resultContBulls.ListBulls = (object) resultFilterBulletins;

            return Ok(resultContBulls);
        }

        // POST
        /// <summary>
        /// Добавляет новое объявление
        /// </summary>
        /// <param name="newBulletin"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(Bulletin))]
        public async Task<IHttpActionResult> AddBulletin(NewBulletin newBulletin)
        {
            if (newBulletin == null)
            {
                return Content(HttpStatusCode.NotFound, "Входные параметры должны быть заданы!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var user = await db.Users.FirstOrDefaultAsync(usEx => usEx.UserId == newBulletin.UserId);
            var bulletin = new Bulletin()
            {
                BullCreateDateTime = newBulletin.BullCreateDateTime,
                BullEditDateTime = newBulletin.BullEditDateTime,
                BullTxt = newBulletin.BullTxt,
                BullRate = newBulletin.BullRate,
                UserId = newBulletin.UserId
                //UserRelation = user
            };

            db.Bulletins.Add(bulletin);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = bulletin.BullId }, bulletin);

        }

        // POST
        /// <summary>
        /// Редактирует объявление по параметру bullId 
        /// </summary>
        /// <param name="editBulletin"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EditBulletin(EditBulletin editBulletin)
        {
            if (editBulletin == null)
            {
                return Content(HttpStatusCode.NotFound, "Входные параметры должны быть заданы!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bulletin = new Bulletin()
            {
                BullId = editBulletin.BullId,
                BullCreateDateTime = GetBulletinDateTimeCreate(editBulletin.BullId),
                BullEditDateTime = editBulletin.BullEditDateTime,
                BullTxt = editBulletin.BullTxt,
                BullRate = editBulletin.BullRate,
                UserId = editBulletin.UserId
            };

            db.Entry(bulletin).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BulletinExists(bulletin.BullId))
                {
                    return Content(HttpStatusCode.NotFound, $"Объявление с Id = {bulletin.BullId} не найдено!");
                }
            }

            return Ok($"Данные объявления с Id = {bulletin.BullId} успешно обновлены!");
        }

        // POST
        /// <summary>
        /// Удаляет объявление по bullId
        /// </summary>
        /// <param name="deleteBulletin"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(Bulletin))]
        public async Task<IHttpActionResult> DeleteBulletin(DeleteBulletin deleteBulletin)
        {
            if (deleteBulletin == null)
            {
                return Content(HttpStatusCode.NotFound, "Входные параметры должны быть заданы!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid bullId = deleteBulletin.BullId;
            var bulletin = await db.Bulletins.FindAsync(bullId);
            if (bulletin == null)
            {
                return Content(HttpStatusCode.NotFound, $"Объявление с Id = {bullId} не найдено!");
            }

            db.Bulletins.Remove(bulletin);
            await db.SaveChangesAsync();

            return Ok($"Объявление c Id = {bullId} успешно удалёно!");
        }

        // POST
        /// <summary>
        /// Удаляет объявления по userId
        /// </summary>
        /// <param name="deleteBulletin"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(Bulletin))]
        public async Task<IHttpActionResult> DeleteBulletinsByUserId(DeleteBulletin deleteBulletin)
        {
            if (deleteBulletin == null)
            {
                return Content(HttpStatusCode.NotFound, "Входные параметры должны быть заданы!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid userId = deleteBulletin.UserId;
            IEnumerable<Bulletin> bull = db.Bulletins.Where(b => b.UserId == userId);
            if (!bull.Any())
            {
                return Content(HttpStatusCode.NotFound, $"Объявления с Id = {userId} не найдены!");
            }

            db.Bulletins.RemoveRange(bull);
            await db.SaveChangesAsync();

            return Ok($"Объявления c Id = {userId} успешно удалёно!");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BulletinExists(Guid id) => dbTemp.Bulletins.Count(bulletin => bulletin.BullId == id) > 0;
        
        private DateTime GetBulletinDateTimeCreate(Guid id) => dbTemp.Bulletins.Find(id)?.BullCreateDateTime ?? DateTime.Now;

        /// <summary>
        /// Возвращает количество записей в таблице Bulletins.
        /// </summary>
        /// <param name="sqlCondition">передаются только условия запроса</param>
        /// <param name="sqlParameters">передаются параметры для sqlCondition</param>
        /// <returns></returns>
        private int GetCountBullsWithCondition(string sqlCondition, List<SqlParameter> sqlParameters)
        {
            string sql = "SELECT count(b.BullId) as BullCount FROM Bulletins AS b " +
                         "LEFT JOIN Users AS u ON (b.UserId = u.UserId) WHERE (BullId is not null)";

            List<int> resultCountBulletins = dbTemp.Database
                    .SqlQuery<int>(sql + sqlCondition, sqlParameters.Select(x => ((ICloneable)x).Clone()).ToArray()).ToList();

            return resultCountBulletins.LastOrDefault();
        }
    }
}