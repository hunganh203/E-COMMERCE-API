namespace Application.Dtos
{
    public class EmailRegistrationDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime Created { get; set; }
    }
}