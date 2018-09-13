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
using System.Reflection;

namespace APIAngular.Controllers
{
    /// <summary>
    /// Управление пользователями.
    /// </summary>
    public class UsersController : ApiController
    {
        private DBManager db = new DBManager();
        private DBManager dbTemp = new DBManager();

        // GET
        /// <summary>
        /// Возвращает список всех пользователей
        /// </summary>
        /// <param name="page">если параметр > 0, то список пользователей будет разбит на страницы, вернёт старницу под номером page</param>
        /// <param name="countUsersOnPage">задаёт количество пользователей на странице, по умолчанию значение 10</param>
        /// <param name="getOnlyCount">если параметр true, то в качестве ответа будет количество пользователей</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Users/GetListUsers/{page?}/{countUsersOnPage?}")]
        [ResponseType(typeof(List<User>))]
        public IHttpActionResult GetListUsers(string page = "0", string countUsersOnPage = "10", bool getOnlyCount = false)
        {
            int npage;
            int ncountUsersOnPage;
            try
            {
                npage = Convert.ToInt32(page);
                ncountUsersOnPage = Convert.ToInt32(countUsersOnPage);
            }
            catch (FormatException)
            {
                return Ok(db.Users);
            }

            if (db.Users.Any())
            {
                if (getOnlyCount)
                    return Ok(db.Users.Count());

                if (npage != 0)
                    return Ok(db.Users.OrderBy(x => x.UserId).Skip(npage * ncountUsersOnPage - ncountUsersOnPage)
                        .Take(ncountUsersOnPage));
            }

            return Ok(db.Users);
            //return NotFound();
        }

        // GET
        /// <summary>
        /// Возвращает пользователя по его Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUserById(Guid id)
        {
            User user = await db.Users.FindAsync(id);
            /*if (user == null)
            {
                return NotFound();
            }*/

            return Ok(user);
        }

        //POST
        /// <summary>
        /// Возвращает список объявлений по заданному фильтру
        /// </summary>
        /// <param name="filterUsers">
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(List<ResultFilterUsers>))]
        public IHttpActionResult GetListUsersByFilter(FilterUsers filterUsers)
        {
            if (filterUsers == null)
            {
                return Content(HttpStatusCode.NotFound, "Должен быть указан хотя бы один входной параметр!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string sql = "WITH resultTable AS (SELECT row_number() OVER (ORDER BY " +
                        ((filterUsers.FieldForSort.ToLower() == ("UserId").ToLower()) ? "u.UserId" : filterUsers.FieldForSort) +
                        ((filterUsers.IsDesc) ? " desc" : " asc") +
                         ") as rowNumber, u.UserId, u.UserName, u.UserMail, u.UserDateTimeRegister, count(b.UserId) as CountBulletins " +
                         "FROM Users AS u LEFT JOIN Bulletins AS b ON (u.UserId = b.UserId) " +
                         "WHERE (u.UserId is not null) ";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            string sqlCondition = "";

            if (filterUsers.UserName != null)
            {
                sqlCondition += " and (UserName LIKE @pUserName)";
                sqlParameters.Add(new SqlParameter("@pUserName", "%" + filterUsers.UserName + "%"));
            }
            if (filterUsers.UserMail != null)
            {
                sqlCondition += " and (UserMail LIKE @pUserMail)";
                sqlParameters.Add(new SqlParameter("@pUserMail", "%" + filterUsers.UserMail + "%"));
            }
            if (filterUsers.UserDateTimeRegisterBegin != Convert.ToDateTime("01.01.1900") &
                filterUsers.UserDateTimeRegisterEnd != Convert.ToDateTime("01.01.1900"))
            {
                sqlCondition += " and (UserDateTimeRegister between @pUserDateTimeRegisterBegin and @pUserDateTimeRegisterEnd)";
                sqlParameters.Add(new SqlParameter("@pUserDateRegisterBegin", filterUsers.UserDateTimeRegisterBegin));
                sqlParameters.Add(new SqlParameter("@pUserDateRegisterEnd", filterUsers.UserDateTimeRegisterEnd));
            }

            string sqlConditionPage = "";
            if (filterUsers.Page != 0 & filterUsers.CountUsersOnPage != 0)
            {
                sqlConditionPage += " WHERE (rowNumber BETWEEN @pPage AND @pCountUsersOnPage)";
                sqlParameters.Add(new SqlParameter("@pPage", filterUsers.Page * filterUsers.CountUsersOnPage - filterUsers.CountUsersOnPage + 1));
                sqlParameters.Add(new SqlParameter("@pCountUsersOnPage", filterUsers.Page * filterUsers.CountUsersOnPage));
            }

            sql += sqlCondition + " GROUP BY u.UserId, u.UserName, u.UserMail, u.UserDateTimeRegister) SELECT * FROM resultTable" + sqlConditionPage;

            List<ResultFilterUsers> resultFilterUsers = null;
            try
            {
                resultFilterUsers = db.Database.SqlQuery<ResultFilterUsers>(sql, sqlParameters.ToArray<SqlParameter>()).ToList();
            }
            catch (SqlException)
            {
                return BadRequest("Ошибка запроса, проверьте входные параметры !");
            }

            int usersCount = GetCountUsersWithCondition(sqlCondition, sqlParameters);
            int pagesCount = 0;
            if (usersCount > 0)
            {
                pagesCount = (usersCount > filterUsers.CountUsersOnPage)
                             ? usersCount / filterUsers.CountUsersOnPage
                                + ((usersCount % filterUsers.CountUsersOnPage > 0) ? 1 : 0)
                             : 1;
            }
            ResultContainerOfUsers resultContUsers = new ResultContainerOfUsers
            {
                CountPages = pagesCount,
                CountUsers = usersCount
            };

            resultContUsers.ListUsers = (object)resultFilterUsers;

            return Ok(resultContUsers);
        }

        // POST
        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> AddUser(NewUser newUser)
        {
            if (newUser == null)
            {
                return Content(HttpStatusCode.NotFound, "Входные параметры должны быть заданы!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User()
            {
                UserName = newUser.UserName,
                UserMail = newUser.UserMail
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
        }

        // POST
        /// <summary>
        /// Редактирование пользователя по userId
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EditUser(NewUser editUser)
        {
            if (editUser == null)
            {
                return Content(HttpStatusCode.NotFound, "Входные параметры должны быть заданы!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User()
            {
                UserId = editUser.UserId,
                UserName = editUser.UserName,
                UserMail = editUser.UserMail
            };

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                {
                    return Content(HttpStatusCode.NotFound, $"Пользователь с Id = {user.UserId} не найден!");
                }
            }

            //return StatusCode(HttpStatusCode.NoContent);
            return Ok($"Данные пользователя с Id = {user.UserId} успешно обновлены!");
        }

        // POST
        /// <summary>
        /// Удаляет пользователя по userId. Удаляются все объявления(Bulletins) пользователя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(User user)
        {
            if (user == null)
            {
                return Content(HttpStatusCode.NotFound, "Входные параметры должны быть заданы!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid userId = user.UserId;
            user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return Content(HttpStatusCode.NotFound, $"Пользователь с Id = {userId} не найден!");
            }

            IEnumerable<Bulletin> bull = db.Bulletins.Where(b => b.UserId == userId);
            bool delBullFlag = bull.Any();

            db.Bulletins.RemoveRange(bull);
            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok($"Пользователь c Id = {userId} успешно удалён!" +
                      ((delBullFlag) ? $" Объявления c UserId = {userId} успешно удалены!" : ""));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(Guid id) => db.Users.Count(user => user.UserId == id) > 0;

        /// <summary>
        /// Возвращает количество записей в таблице Users.
        /// </summary>
        /// <param name="sqlCondition">передаются только условия запроса</param>
        /// <param name="sqlParameters">передаются параметры для sqlCondition</param>
        /// <returns></returns>
        private int GetCountUsersWithCondition(string sqlCondition, List<SqlParameter> sqlParameters)
        {
            string sql = "SELECT COUNT(UserId) FROM Users " +
                         "WHERE (UserId is not null) ";

            List<int> resultCountUsers = dbTemp.Database
                    .SqlQuery<int>(sql + sqlCondition, sqlParameters.Select(x => ((ICloneable)x).Clone()).ToArray()).ToList();

            return resultCountUsers.LastOrDefault();
        }
    }
}