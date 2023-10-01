﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRecordApi.Models;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace EmployeeRecordApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {
        private readonly EntityDBContext _context;

        public EmployeeApiController(EntityDBContext context)
        {
            _context = context;
        }

        [Authorize]
        // Get All Employees
        [HttpGet("/GetAllEmployees")]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployees()
        {
          if (_context.Employees == null)
          {
                Log.Information("Please Create an Employee");
                return NotFound();
          }
          var employee = await _context.Employees.Where(e => e.RecordDate != DateTime.Parse("2100-01-01")).ToListAsync();
          Log.Information("Employee result => {@employeeModel}", employee);
          return employee;
        }

        [Authorize]
        // Get Employee by Id
        [HttpGet("/GetById/{id}")]
        public async Task<ActionResult<EmployeeModel>> GetEmployeeModel(int id)
        {
            if (_context.Employees == null)
            {
                Log.Information("Please Create an Employee");
                return new NotFoundObjectResult("Please Create an Employee");
            }
            var employeeModel = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNumber == id && e.RecordDate != DateTime.Parse("2100-01-01"));

            if (employeeModel == null)
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }
            Log.Information("Employee result => {@employeeModel}", employeeModel);
            return employeeModel;
        }

        [Authorize]
        // Get Employee by FirstName
        [HttpGet("/GetByFirstName/{firstName}")]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployeeModelByName(string name)
        {
            if (_context.Employees == null)
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }
            var employees = await _context.Employees.Where(e => e.FirstName == name && e.RecordDate != DateTime.Parse("2100-01-01")).ToListAsync();

            if (employees == null)
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }
            Log.Information("Employee result => {@employeeModel}", employees.ToList());
            return employees.ToList();
        }

        [Authorize]
        // Get Employee by LastName
        [HttpGet("/GetByLastName/{lastName}")]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployeeModelByLastName(string name)
        {
            if (_context.Employees == null)
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }
            var employees = await _context.Employees.Where(e => e.LastName == name && e.RecordDate != DateTime.Parse("2100-01-01")).ToListAsync();

            if (employees == null)
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }

            Log.Information("Employee result => {@employeeModel}", employees.ToList());
            return employees.ToList();
        }

        [Authorize]
        // Get Employee by TemperatureRange
        [HttpPost("/GetByTemperatureRange")]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployeeModelByTemperatureRange([FromBody] TemperatureRangeModel temperatureRange)
        {
            if (_context.Employees == null)
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }

            var employees = await _context.Employees
                .Where(e => e.Temperature >= temperatureRange.minTemperature && e.Temperature <= temperatureRange.maxTemperature && e.RecordDate != DateTime.Parse("2100-01-01"))
                .ToListAsync();

            if (employees == null || !employees.Any())
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }
            Log.Information("Employee result => {@employeeModel}", employees.ToList());
            return employees.ToList();
        }

        [Authorize]
        // Get Employee by DateRange
        [HttpPost("/GetByDateRange")]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployeeModelByDateRange([FromBody] DateRangeModel dateRange)
        {
            if (_context.Employees == null)
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }

            var employees = await _context.Employees
                .Where(e => e.RecordDate >= dateRange.startDate && e.RecordDate <= dateRange.endDate && e.RecordDate !=  DateTime.Parse("2100-01-01"))
                .ToListAsync();

            if (employees == null || !employees.Any())
            {
                Log.Information("Employee Not Found");
                return new NotFoundObjectResult("Employee Not Found");
            }
            Log.Information("Employee result => {@employeeModel}", employees.ToList());
            return employees.ToList();
        }

        [Authorize]
        //Update Employee
        [HttpPut("/UpdateEmployee")]
        public async Task<IActionResult> PutEmployeeModel([FromBody] EmployeeModel employeeModel)
        {
            try
            {
                var existingEmployee = await _context.Employees.FindAsync(employeeModel.EmployeeNumber);

                if (existingEmployee == null)
                {
                    Log.Information("Please provide Employee Number in the body");
                    return NotFound("Please provide Employee Number in the body");
                }

                _context.Entry(existingEmployee).CurrentValues.SetValues(employeeModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex) { 

                Log.Information("Please provide Employee Number in the body");

                if (!EmployeeModelExists(employeeModel.EmployeeNumber))
                {
                    return NotFound("Employee not found.");
                }
                else
                {
                    return StatusCode(500, "Concurrency conflict occurred. Please try again.");
                }
            }

            return NoContent();
        }

        [Authorize]
        //Create Employee
        [HttpPost("/AddEmployee")]
        public async Task<ActionResult<EmployeeModel>> PostEmployeeModel([FromBody] EmployeeModel employeeModel)
        {
          if (_context.Employees == null)
          {
              return Problem("Entity set 'EntityDBContext.Employees'  is null.");
          }
            _context.Employees.Add(employeeModel);
            await _context.SaveChangesAsync();

            Log.Information("Successfully Created New Employee");
            return CreatedAtAction("GetEmployeeModel", new { id = employeeModel.EmployeeNumber }, employeeModel);
        }

        [Authorize]
        //Delete Employee
        [HttpDelete("/DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployeeModel(int id)
        {
            if (_context.Employees == null)
            {
                Log.Information("Employee Not Found");
                return NotFound();
            }

            var employeeModel = await _context.Employees.FindAsync(id);
            if (employeeModel == null)
            {
                Log.Information("Employee Not Found");
                return NotFound();
            }

            // Soft delete by setting the deletion date to 2100-01-01
            employeeModel.RecordDate = DateTime.Parse("2100-01-01");

            // Mark the entity as modified, so Entity Framework will update the DeletionDate property
            _context.Entry(employeeModel).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            Log.Information("Successfully Deleted Employee with EmployeeId: ", + id);
            return NoContent();
        }

        private bool EmployeeModelExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeNumber == id)).GetValueOrDefault();
        }
    }
}