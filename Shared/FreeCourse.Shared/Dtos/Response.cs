using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FreeCourse.Shared.Dtos
{
    public class Response<T>
    {
        public T Data { get; set; }//başarılı oldugunda gidecek olan bir datam olsun
        [JsonIgnore]
        public int StatusCode { get; set; }
        public bool IsSuccessful { get; set; } //başarılı mı degil mi
        public List<string> Errors { get; set; } //başarılı t data hata olunca burası
        public static Response<T> Success(T data, int statusCode)//başarılı da 200 var 201 var hangisi yani
        {
            return new Response<T> { StatusCode = statusCode, Data = data };
        }
        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default(T), StatusCode = statusCode, IsSuccessful = true };
        }

        public static Response<T> Fail(List<string> errors, int statusCode)
        {
            return new Response<T>
            {
                Errors = errors,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }

        public static Response<T> Fail(string error, int statusCode)
        {
            return new Response<T> { Errors = new List<string>() { error }, StatusCode = statusCode, IsSuccessful = false };
        }
    }
}
