using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomlyn;
using Tomlyn.Model;
using UnityEngine;
using UnityEngine.Tilemaps;
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

    public class Grafic
    {
        public Texture2D grafic;
        public string graficPath;

        public bool placementLoading;

        public Grafic(string grafic)
        {
            this.graficPath = grafic;
            this.placementLoading = false;
        }

        public virtual Texture2D LoadTexture()
        {
            this.grafic = Core.GetTexture(graficPath);
            return this.grafic;
        }
    }

    public class TileGrafic : Grafic
    {
        public int tileSize;
        public int tilesX;
        public int tilesY;

        public TileGrafic(string grafic, int tileSize, int tilesX, int tilesY) : base(grafic)
        {
            this.tileSize = tileSize;
            this.tilesX = tilesX;
            this.tilesY = tilesY;

            this.graficPath = grafic;
            this.placementLoading = true;
        }

        public override Texture2D LoadTexture()
        {
            int x = UnityEngine.Random.Range(0, tilesX);
            int y = UnityEngine.Random.Range(0, tilesY);
            this.grafic = Core.GetTexture(graficPath + x.ToString() + ";" + y.ToString());

            return this.grafic;
        }
    }

    public class ThingWithGrafic : Thing
    {
        public Grafic? grafic;

        public ThingWithGrafic(string tableName, string name, string id, object grafic) : base(tableName, name, id)
        {
            this.tableName = tableName;
            this.id = id;
            this.name = name;

            TomlTable? toml = grafic as TomlTable;
            if (toml != null)
            {
                if (toml["type"].ToString() == "RandomTile")
                {
                    //this.grafic = new Grafic(toml["grafic"].ToString());
                    this.grafic = new TileGrafic(toml["grafic"].ToString(), int.Parse(toml["tileSize"].ToString()), int.Parse(toml["tilesX"].ToString()), int.Parse(toml["tilesY"].ToString()));
                }
            }
            else if (grafic as string != null)
            {
                this.grafic = new Grafic((string)grafic);
            }
        }
    }
}
