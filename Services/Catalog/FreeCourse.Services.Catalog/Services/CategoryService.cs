using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;//hangi collectionla baglantı kurucam category
        private readonly IMapper _mapper;//ayrıca datayı dönüştürmemiz gerekecek

        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);//bana bir veritabanı ver
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);//database üzerinden bana bi collection ver ismi de databaseden gelen
            
            _mapper = mapper;
        }
        public async Task<Response<List<CategoryDto>>> GetAllAsync()//içerisinde list category alacak - buradaki generic category responsedeki t dataya karşılık geliyor
        {
            var categories = await _categoryCollection.Find(category => true).ToListAsync();//true ile beraber bana tüm dataları ver dedik
            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), 200);
        }
        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);//categorye dönüştürdük
            await _categoryCollection.InsertOneAsync(category);
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }
        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            var category = await _categoryCollection.Find<Category>(x => x.Id == id).FirstOrDefaultAsync();//kcategory üzeerinden arama yapıyorum eger x id si id eşitse ilk karşılaştıgını getir
            if (category == null)
            {
                return Response<CategoryDto>.Fail("Category not found", 404);

            }
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);

        }
    }
}
