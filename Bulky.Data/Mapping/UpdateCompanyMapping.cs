using Bulky.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Data.Mapping
{
    public static class UpdateCompanyMapping
    {
        public static void ToEntity(this Company company,Company company1)
        {
            company1.Name=company.Name;
            company1.City=company.City;
            company1.State=company.State;
            company1.StreetAddress=company.StreetAddress;
            company1.PhoneNumber=company.PhoneNumber;
            company1.PostalCode=company.PostalCode;
        }
    }
}
