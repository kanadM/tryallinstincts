using System.Configuration;

namespace CustomNestedConfigSections.NestedConfig
{
    public class Employee : ConfigurationElement
    {
        [ConfigurationProperty("id", IsKey = true)]
        public string Id { get { return (string) this["id"]; } }

        [ConfigurationProperty("personalInfo")]
        public PersonalInfo PersonalInfo
        {
            get { return (PersonalInfo)this["personalInfo"]; }
        }

        [ConfigurationProperty("homeAddress")]
        public Address HomeAddress { get { return (Address)this["homeAddress"]; } }

        [ConfigurationProperty("officeAddress")]
        public Address OfficeAddress { get { return (Address)this["officeAddress"]; } }
    }
}