using System.IO;
using LiteDB;

namespace Shared
{
    public class LiteDbProvider
    {
        private readonly string _connectionPath;
        private bool IsEnsured = false;
        
        public LiteDbProvider(string connectionPath)
        {
            _connectionPath = connectionPath;
        }

        public LiteDbProvider EnsureIndexes()
        {

            if (IsEnsured)
                return this;

            EnsureConnectionPath(_connectionPath);

            using (var db = new LiteDatabase(_connectionPath))
            {
                var films = db.GetCollection<Film>();
                
                films.EnsureIndex(film => film.Title);
                films.EnsureIndex(film => film.Year);
                films.EnsureIndex(film => film.Medias);
                films.EnsureIndex(film => film.ExternalId);
                films.EnsureIndex(film => film.ExternalSource);
                
                var storages = db.GetCollection<Storage>();
                storages.EnsureIndex(x => x.Title, true);
            }

            this.IsEnsured = true;

            return this;
        }

        private void EnsureConnectionPath(string connectionPath)
        {
            var fileInfo = new FileInfo(connectionPath);
            if (!fileInfo.Directory.Exists)
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
        }

        public LiteDatabase GetDatabase()
        {
            EnsureIndexes();
            return new LiteDatabase(_connectionPath);
        }
    }
}