namespace DemoApi.Data.Models
{
    public class Action
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
