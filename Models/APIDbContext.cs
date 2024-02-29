using Microsoft.EntityFrameworkCore;

namespace DemoApi.Models
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        { }

    }
}