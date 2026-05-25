namespace NutriZeka.UI.Dtos.UserDtos
{
    public class ResultUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string PhoneNumber { get; set; } // Yeni eklenen alan
        public int ProfileCompletionRate { get; set; }
        public bool IsGoogleUser { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsLactoseFree { get; set; }
        public bool IsPalmOilFree { get; set; }
    }
}
