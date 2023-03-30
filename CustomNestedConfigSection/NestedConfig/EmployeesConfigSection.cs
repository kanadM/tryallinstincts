using System.Configuration;

namespace CustomNestedConfigSections.NestedConfig
{
    public class EmployeesConfigSection : ConfigurationSection
    {
        //If you replace "employeeCollection" with "" then you do not need "employeeCollection" element as a wrapper node over employee nodes in config file.
        [ConfigurationProperty("employeeCollection", IsDefaultCollection = true, IsKey = false, IsRequired = true)]
        public EmployeeCollection Members
        {
            get
            {
                return base["employeeCollection"] as EmployeeCollection;
            }

            set
            {
                base["employeeCollection"] = value;
            }
        }

    }
}