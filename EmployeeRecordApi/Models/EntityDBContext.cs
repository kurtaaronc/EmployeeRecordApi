using Microsoft.EntityFrameworkCore;

namespace EmployeeRecordApi.Models
{
    public class EntityDBContext : DbContext
    {
        public EntityDBContext(DbContextOptions option):base(option) { 

        }   
        
        public DbSet<EmployeeModel> Employees { get; set; } 

    }
}
