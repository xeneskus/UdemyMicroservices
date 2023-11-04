namespace FreeCourse.Services.Catalog.Dtos
{
    public class CategoryDto
    {
        public string? Id { get; set; } //id string olmalı çünkü mongodb otomatik üretecek
        public string Name { get; set; }
    }
}
