using MongoDB.Bson;
using MongoDB.Driver;

namespace XXX.Plugin.MongoDB.Service
{
    public interface IMongoDBDemoService
    {
        void InsertOne(string key = "mongo1");
        void DeleteOne(string key = "mongo1");
        void ReplaceOne(string key = "mongo1");
        Product Find(string key = "mongo1");

    }

    public class MongoDBDemoService : IMongoDBDemoService
    {
        private readonly IMongoDBMulti _mongoDBMulti;
        public MongoDBDemoService(IMongoDBMulti mongoDBMulti)
        {
            _mongoDBMulti = mongoDBMulti;
        }

        /// <summary>
        /// 通过数据库别名key新增到指定数据库
        /// </summary>
        public void InsertOne(string key = "mongo1")
        {
            var mongodb = _mongoDBMulti[key];

            mongodb.GetCollection<Product>().InsertOne(new Product
            {
                Name = "name",
                Category = "Category",
                Description = "Description",
                ImageFile = "ImageFile",
                Price = 12121212,
                Summary = "Summary",
            });

        }

        /// <summary>
        /// 删除
        /// </summary>
        public void DeleteOne(string key = "mongo1")
        {
            var mongodb = _mongoDBMulti.Get(key);
            //1、
            //var filter = Builders<Product>.Filter;
            //mongodb.GetCollection<Product>().DeleteOne(filter.Eq("Name", "11"));
            //2、
            mongodb.GetCollection<Product>().DeleteOne(s => s.Name == "11");
        }

        /// <summary>
        /// 更新整个对象
        /// </summary>
        public void ReplaceOne(string key = "mongo1")
        {
            var mongodb = _mongoDBMulti[key];
            var filter = Builders<BsonDocument>.Filter;
            mongodb.GetCollection<Product>().ReplaceOne(s => s.Category == "", new Product
            {

            });
        }

        /// <summary>
        /// 查询
        /// </summary>
        public Product Find(string key = "mongo1")
        {
            var mongodb = _mongoDBMulti[key];
            var filter = Builders<BsonDocument>.Filter;
            var product = mongodb.GetCollection<Product>().Find(s => s.Category == "").FirstOrDefault();

            return product;
        }
    }
}
