using System;
using System.Collections.Generic;

namespace RestWebApiPumoxGmgH.Models
{
    public partial class Employee
    {
        public long EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public long? Idcompany { get; set; }

        public virtual Company IdcompanyNavigation { get; set; }
    }
}
