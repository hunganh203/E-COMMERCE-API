namespace Application.Dtos.Menu
{
    public class MenuDto
    {
        public int Id { get; set; }
        public int? PMenuId { get; set; }
        public string Group { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public int? Index { get; set; }
        public bool? ShowHomePage { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool Active { get; set; }

        public MenuDto? PMenu { get; set; }
        public List<MenuDto> SubMenus { get; set; } = new();
    }
}