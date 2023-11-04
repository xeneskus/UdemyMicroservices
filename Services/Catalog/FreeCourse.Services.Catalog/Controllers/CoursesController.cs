using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : CustomBaseController

    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string id)
        {
            var response = await _courseService.GetAllAsync(); //git course service den getbyid async al idyi ver
          
            
            return CreateActionResultInstance(response);
       
        }
        //courses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _courseService.GetByIdAsync(id); //git course service den getbyid async al idyi ver
          
            
            return CreateActionResultInstance(response);//bu temiz yolu kendi otomatik yapacak
            //if (response.StatusCode==404)//id nullda gelebilir
            //{
            //    return NotFound(response.Errors);  //her seferinde böyle kontrol mu yapıcaz yani
            //}
        }


    
        [HttpGet]
        [Route("/api/[controller]/GetAllByUserId/{userId}")]//api/courses/getallbyuserid/4
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var response = await _courseService.GetAllByUserIdAsync(userId); 
            
            return CreateActionResultInstance(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateDto courseCreateDto)
        {
            var response = await _courseService.CreateAsync(courseCreateDto);

            return CreateActionResultInstance(response);

        }

        [HttpPut]
        public async Task<IActionResult> Update(CourseUpdateDto courseUpdateDto)
        {
            var response = await _courseService.UpdateAsync(courseUpdateDto); 
            return CreateActionResultInstance(response);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _courseService.DeleteAsync(id);


            return CreateActionResultInstance(response);
        }


    }
}
