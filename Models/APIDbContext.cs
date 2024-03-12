using Microsoft.EntityFrameworkCore;

namespace DemoApi.Data.Models
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        { }

    }
}