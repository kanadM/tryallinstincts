using System.Configuration;

namespace CustomNestedConfigSections.NestedConfig
{
    public class Address : ConfigurationElement 
    {
        [ConfigurationProperty("pin")]
        public string PinCode { get { return (string) this["pin"]; } }

        [ConfigurationProperty("city")]
        public string City { get { return (string)this["city"]; } }

        [ConfigurationProperty("state")]
        public string State { get { return (string)this["state"]; } }
    }
}