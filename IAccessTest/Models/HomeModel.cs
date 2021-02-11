using System;
using System.Collections.Generic;

namespace IAccessTest.Models
{
    public class HomeModel
    {
        public string Keywords { get; set; }
        public int PageNumber { get; set; }
        public List<DataModel> DataModelList { get; set; }
    }
}