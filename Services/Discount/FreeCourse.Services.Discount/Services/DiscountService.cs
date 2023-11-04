using Dapper;
using FreeCourse.Shared.Dtos;
using Npgsql;
using System.Data;

namespace FreeCourse.Services.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;//veri tabanına bağlanmak için

        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql")); //bu sınıf ile appsettingsi aldık
        }

        public async Task<Response<NoContent>> Delete(int id)
        {
            var status = await _dbConnection.ExecuteAsync("delete from discount where id=@Id", new { Id = id });
            return status > 0 ? Response<NoContent>.Success(204) : Response<NoContent>.Fail("Discount not found", 404);
        }

        public async Task<Response<List<Models.Discount>>> GetAll()
        {
            var discounts = await _dbConnection.QueryAsync<Models.Discount>("Select * from discount");
            return Response<List<Models.Discount>>.Success(discounts.ToList(),200);
        }

        public async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
        {
            var discount = await _dbConnection.QueryAsync<Models.Discount>("select * from discount where userid=@UserId and code=@Code", new { UserId = userId, Code = code });
            var hasDiscount = discount.FirstOrDefault(); // ilk buldugunu getir bir tane data gelecegini biliyoruz ondan dedik
            if (hasDiscount==null)
            {
                return Response<Models.Discount>.Fail("Discount not found", 404);
            }
            return Response<Models.Discount>.Success(hasDiscount, 200);
        }

        public async Task<Response<Models.Discount>> GetById(int id)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>("select*from discount where id=@Id", new { Id = id })).SingleOrDefault(); //, sonrası @Id kısmını doldurmak için
            //singleordefault te kbir veri döneceği için 
            if (discount==null)
            {
                return Response<Models.Discount>.Fail("Discount not found", 404);
            }
            return Response<Models.Discount>.Success(discount, 200);
        } 
        

        public async Task<Response<NoContent>> Save(Models.Discount discount)
        {
            var saveStatus = await _dbConnection.ExecuteAsync("INSERT INTO discount (userid,rate,code) VALUES(@UserId,@Rate,@Code)", discount); //bu sefer direkt discounttan verdik
            if (saveStatus>0)
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail("an error accured while adding",500);
        }

        public async Task<Response<NoContent>> Update(Models.Discount discount)
        {
            var status = await _dbConnection.ExecuteAsync("update discount set userid=@UserId,code=@Code,rate@Rate where id=@Id", discount);
            if (status > 0)
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail("Discount not found", 404);
        }
    }
}
