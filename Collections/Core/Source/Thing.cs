using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yipeee;

namespace Core
{
    public class ThingCategory : Feature
    {
        public string name;
        public object[] things;

        public ThingCategory(string tableName, string name, object[] things) : base(tableName)
        {
            this.tableName = tableName;
            this.name = name;
            this.things = things;
        }
    }

    public class Thing : Feature
    {
        public string id;
        public string name;

        public Thing(string tableName, string name, string id) : base(tableName)
        {
            this.tableName = tableName;
            this.id = id;
            this.name = name;
        }
    }

    public class ThingWithGrafic : Thing
    {
        public string grafic;

        public ThingWithGrafic(string tableName, string name, string id, string grafic) : base(tableName, name, id)
        {
            this.tableName = tableName;
            this.id = id;
            this.name = name;
            this.grafic = grafic;
        }
    }
}
