namespace FreeCourse.Services.Basket.Dtos
{
    public class BasketDto
    {
        public string UserId { get; set; } //sepet kime ait
        public string DiscountCode { get; set; }//indirim kodu
        public List<BasketItemDto> basketItemms { get; set; }

        public decimal TotalPrice
        {
            get => basketItemms.Sum(x => x.Price * x.Quantity); // sum içerideki propları çarpar ve tek değer döner decimal
        }
    }
}
