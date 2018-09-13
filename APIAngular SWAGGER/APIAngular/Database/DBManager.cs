using System.Data.Entity.Migrations;

namespace APIAngular.Database
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class DBManager : DbContext
    {
        // Контекст настроен для использования строки подключения "DBManager" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "APIAngular.Database.DBManager" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "DBManager" 
        // в файле конфигурации приложения.
        public DBManager()
            : base("name=DBManager")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DBManager>());
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DBManager>());
        }

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Bulletin> Bulletins { get; set; }
    }
}