﻿using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MassTransit;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;//hangi collectionla baglantı kurucam category
        private readonly IMongoCollection<Category> _categoryCollection;//hangi collectionla baglantı kurucam category
        private readonly IMapper _mapper;//ayrıca datayı dönüştürmemiz gerekecek
        private readonly IPublishEndpoint _publishEndPoint;
        public CourseService(IDatabaseSettings databaseSettings, IMapper mapper, IPublishEndpoint publishEndPoint)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);//bana bir veritabanı ver
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);//database üzerinden bana bi collection ver ismi de databaseden gelen
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);//database üzerinden bana bi collection ver ismi de databaseden gelen

            _mapper = mapper;
            _publishEndPoint = publishEndPoint;
        }

        public async Task<Shared.Dtos.Response<List<CourseDto>>> GetAllAsync()
        {
            //mongodbde ilişkisel bir işlem olmadığından includa join gibi işlemler yok lazy loading vs
            var courses = await _courseCollection.Find(course => true).ToListAsync();



            if (courses.Any())//eğer any değilse yani içerisinde mutlaka bir kayıt varsa geriye true dönecek
            {
                //tüm kurslarda dolan dolandıktan sonra bu kursta category doldur
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();//buradaki kurstaki category id yi alıyoruz bu categorydeki idye bağlı categoriyi getir dedik
                }
            }
            else//yok ise bir kayır
            {
                courses = new List<Course>(); //varsa zaten dolduracvak yoksa boş bir kurs listesi bizim için oluştursun
            }
            return Shared.Dtos.Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Shared.Dtos.Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();
            if (course == null)
            {
                return Shared.Dtos.Response<CourseDto>.Fail("Course not found", 404);


            }
            course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
            return Shared.Dtos.Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }
        public async Task<Shared.Dtos.Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();

            if (courses.Any())//eğer any değilse yani içerisinde mutlaka bir kayıt varsa geriye true dönecek
            {
                //tüm kurslarda dolan dolandıktan sonra bu kursta category doldur
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();//buradaki kurstaki category id yi alıyoruz bu categorydeki idye bağlı categoriyi getir dedik
                }
            }
            else//yok ise bir kayır
            {
                courses = new List<Course>(); //varsa zaten dolduracvak yoksa boş bir kurs listesi bizim için oluştursun
            }
            return Shared.Dtos.Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Shared.Dtos.Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);
            newCourse.CreatedTime = DateTime.Now;
            await _courseCollection.InsertOneAsync(newCourse);
            return Shared.Dtos.Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
        }

        public async Task<Shared.Dtos.Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);
            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);//bul ve değiştir metodu findone adnd replace - idyi bul buldulktan sonra da updateCourse u güncelle
            if (result == null)//yukarıda bulamazsa result boş dönecek
            {
                return Shared.Dtos.Response<NoContent>.Fail("Course not found", 404);
            }

            await _publishEndPoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent {  CourseId = updateCourse.Id, UpdatedName = courseUpdateDto.Name });
            return Shared.Dtos.Response<NoContent>.Success(204);//bodysi olmayan başarılı bir durum kodu

        }

        public async Task<Shared.Dtos.Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);
            if (result.DeletedCount > 0)//bu resultun deletedcountu 0 landıysa silmmiş
            {
                return Shared.Dtos.Response<NoContent>.Success(204);
            }
            else //ama else ise bir şey silemediyse demek ki veritabanında bulamadı
            {
                return Shared.Dtos.Response<NoContent>.Fail("Course not found", 404);
            }
        }
    }
}
