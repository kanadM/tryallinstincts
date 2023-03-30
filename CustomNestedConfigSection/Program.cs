using System;
using System.Configuration;
using CustomNestedConfigSections.NestedConfig;

namespace CustomNestedConfigSections
{
    class Program
    {
        static void Main(string[] args)
        {
            var employee1 = GetEmployee("1");
            Console.WriteLine("\n\nDetails of employee with id : 1");
            Console.WriteLine("Personal Info: SSN - {0} Height - {1}inch Weight - {2}Kg", 
            employee1.PersonalInfo.SSN, employee1.PersonalInfo.Height,employee1.PersonalInfo.Weight);
            Console.WriteLine("Home Address : {0} {1} {2}", employee1.HomeAddress.City, 
            employee1.HomeAddress.State, employee1.HomeAddress.PinCode);
            Console.WriteLine("Office Address : {0} {1} {2}", employee1.OfficeAddress.City, 
            employee1.OfficeAddress.State, employee1.OfficeAddress.PinCode);

            Console.ReadKey();
        }

        public static Employee GetEmployee(string employeeId)
        {
            var employeesConfigSection = ConfigurationManager.GetSection
            ("employeeCollectionSection") as EmployeesConfigSection;

            if (employeesConfigSection == null || 
            employeesConfigSection.Members == null || employeesConfigSection.Members.Count < 1)
                return null;

            return employeesConfigSection.Members[employeeId];
        }
    }
}