using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yipeee;

namespace Core
{
    public class ExtTerrain : Feature
    {
        public ExtTerrain(string tableName) : base(tableName)
        {
            this.tableName = tableName;
        }
    }

    public class ExtTerrainCategory : Feature
    {
        public object[] rules;
        public ExtTerrainCategory(string tableName, object[] rules) : base(tableName)
        {
            this.tableName = tableName;
            this.rules = rules;
        }
    }
}
