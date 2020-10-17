using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestWebApiPumoxGmgH.Models;

namespace RestWebApiPumoxGmgH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly CompanyDBContext _context;

        public CompaniesController(CompanyDBContext context)
        {
            _context = context;
        }
        #region PUT
        // PUT: api/Companies/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(long id, Company company)
        {
            if (id != company.CompanyId)
            {
                return BadRequest();
            }
            if (CheckIfDataIsFull(company))
            {
                _context.Entry(company).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }           
            return NoContent();
        }
        #endregion
        #region POST
        // POST: api/Companies
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<long>> PostCompany(Company company)
        {
            if (CheckIfDataIsFull(company))
            {
                _context.Company.Add(company);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            else
            {
                BadRequest();
            }
            return company.CompanyId;
        }
        // POST: api/Companies/search
        [HttpPost("search")]
        public ActionResult<List<Company>> GetCompanySearch(CompanySearch companySearch)
        {
            List<Company> companySearchResult = new List<Company>();
            try
            {
                companySearchResult = _context.Company
                    .GroupJoin(_context.Employee,
                                Company => Company.CompanyId,
                                Employee => Employee.Idcompany,
                                (x, y) =>
                                new
                                {
                                    Company = x,
                                    Employee = y
                                })
                    .SelectMany(xy => 
                    xy.Employee.DefaultIfEmpty(),
                    (x, y) =>
                    new
                    {
                        x.Company,
                        Employee = y
                    })
                    .Where(e => 
                        CheckIfItemIsNull<string>(companySearch.Keyword) 
                        || (companySearch.Keyword == e.Company.CompanyName 
                        || companySearch.Keyword == e.Employee.FirstName 
                        || companySearch.Keyword == e.Employee.LastName)
                        )
                    .Where(e => 
                        CheckIfItemIsNull<DateTime?>(companySearch.EmployeeDateOfBirthFrom) 
                        || companySearch.EmployeeDateOfBirthFrom <= e.Employee.DateOfBirth
                        )
                    .Where(e => 
                        CheckIfItemIsNull<DateTime?>(companySearch.EmployeeDateOfBirthTo) 
                        || companySearch.EmployeeDateOfBirthTo > e.Employee.DateOfBirth
                        )
                    .Where(e => 
                        CheckIfItemIsNull<List<string>>(companySearch.EmployeeJobTitles) 
                        || companySearch.EmployeeJobTitles.Contains(e.Employee.JobTitle)
                        )
                    .Select(s => 
                        s.Company
                        )
                    .ToList();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
            return companySearchResult;
        }
        #endregion
        #region Delete
        // DELETE: api/Companies/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Company>> DeleteCompany(long id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            else
            {
                var employee = _context.Employee.Where(w => w.Idcompany == id).ToList();
                if(employee.Any())
                {
                    _context.Employee.RemoveRange(employee);
                }
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return company;
        }
        #endregion
        #region Additional methods
        private bool CheckIfDataIsFull(Company company)
        {
            return (company.CompanyName != null
                && company.EstablishmentYear.ToString().Length == 4) ? true : false;

        }
        private bool CompanyExists(long id)
        {
            return _context.Company.Any(e => e.CompanyId == id);
        }
        public bool CheckIfItemIsNull<T>(T keyword)
        {
            return keyword == null;
        }
        #endregion
    }
}
