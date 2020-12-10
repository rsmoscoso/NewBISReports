using System;
using System.Runtime.Serialization;

namespace NewBISReports.Models
{
    [DataContract]
    public class NameSearch
    {

        [DataMember(Name = "searchField")]
        public string searchField { get; set; }
        [DataMember(Name = "PersClassIdArray")]
        public string[] PersClassIdArray { get; set; }
        public SEARCHPERSONS SearchType
        {
            get
            {
                SEARCHPERSONS tempEnum;
                Enum.TryParse(SearchTypeString, out tempEnum);
                return tempEnum;
            }
            set
            {
                var enumToConvert = (SEARCHPERSONS)value;
                SearchTypeString = enumToConvert.ToString();
            }
        }
        [DataMember(Name = "SearchTypeString")]
        public string SearchTypeString { get; set; }
    }
}