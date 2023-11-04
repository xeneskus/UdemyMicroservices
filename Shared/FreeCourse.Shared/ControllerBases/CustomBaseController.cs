using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Shared.ControllerBases
{
    public class CustomBaseController : ControllerBase //iki kere tıkladık sharede bu controllerbase i ekledik şu şekil 	<FrameworkReference Include="Microsoft.AspNetCore.App"></FrameworkReference>
    {
        public IActionResult CreateActionResultInstance<T>(Response<T> response)//buradan 404 geldiyse 404 döncek vs
        {
            return new ObjectResult(response) //object result dönme sebebimiz ok olabilir not olabilir her türlü şey var
            {
                StatusCode = response.StatusCode //status kodu bizim response dan gelen status kod olsun
            }; 
        }
    }
}
