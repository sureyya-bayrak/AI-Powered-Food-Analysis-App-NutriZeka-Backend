namespace NutriZeka.UI.Dtos.UserDtos
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; } // Eklendi
        public IFormFile ProfileImageFile { get; set; } // Yeni yüklenecek dosya
        public bool IsGlutenFree { get; set; }
        public bool IsLactoseFree { get; set; }
        public bool IsPalmOilFree { get; set; }
    }
}
