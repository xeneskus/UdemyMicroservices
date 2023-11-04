namespace FreeCourse.Services.Discount.Models
{
    [Dapper.Contrib.Extensions.Table("discount")] //postrgsql içerisindeki discount tablosuna eşit olsun dedik 
    public class Discount
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public int Rate { get; set; }
        public string Code { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
