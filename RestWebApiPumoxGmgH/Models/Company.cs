using System;
using System.Collections.Generic;

namespace RestWebApiPumoxGmgH.Models
{
    public partial class Company
    {
        public Company()
        {
            Employee = new HashSet<Employee>();
        }

        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int EstablishmentYear { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
