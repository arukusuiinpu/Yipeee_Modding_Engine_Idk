using System;
using System.Text;
using System.Threading.Tasks;
using Core.SebastianLague;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Yipeee;
using DynamicExpresso;
using Tomlyn;
using static UnityEngine.GridBrushBase;
using Tomlyn.Model;
using System.Data;
using static UnityEngine.Networking.UnityWebRequest;

namespace Core
{
    public class RhombicCoordinates
    {
        public Vector2 vec2;
        public RhombicCoordinates(Vector2 uv)
        {
            Vector2 rhombic = uv;
            rhombic.x /= (1f - Mathf.Abs(rhombic.y));
            vec2 = rhombic;
        }
        public RhombicCoordinates(float u, float v)
        {
            Vector2 rhombic = new Vector2(u, v);
            rhombic.x /= (1f - Mathf.Abs(rhombic.y));
            vec2 = rhombic;
        }

        public Vector2 ToVector2()
        {
            return vec2;
        }
    }
    public class Rhombic3Coordinates
    {
        public Vector3 vec3;
        public Rhombic3Coordinates(Vector2 uv, float z)
        {
            Vector3 rhombic = new Vector3(uv.x, uv.y, z);
            rhombic.x /= (1f - Mathf.Abs(rhombic.y));
            vec3 = rhombic;
        }
        public Rhombic3Coordinates(float u, float v, float z)
        {
            Vector3 rhombic = new Vector3(u, v, z);
            rhombic.x /= (1f - Mathf.Abs(rhombic.y));
            vec3 = rhombic;
        }

        public Vector3 ToVector3()
        {
            return vec3;
        }
    }
    public class ObjectLayer
    {
        public GameObject self;
        public Tilemap tilemap;

        public int index;

        public ObjectLayer(int index)
        {
            this.index = index;
        }

        public int GetIndex()
        {
            return index;
        }
    }
    public class Chunk
    {
        public GameObject self;

        public Vector2 worldPosition;
        public ObjectLayer[] layers = [new ObjectLayer(0)];

        public Chunk(Vector2 worldPosition)
        {
            //worldPosition.x = Mathf.Floor(worldPosition.x / size.x) * size.x;
            //worldPosition.y = Mathf.Floor(worldPosition.y / size.y) * size.y;

            this.worldPosition = worldPosition;
            self = new GameObject($"Chunk {worldPosition.ToString()}");
            self.transform.SetParent(GameObject.Find("Planet").transform);
            self.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);
            //self.transform.LookAt(GameObject.Find("Planet").transform);
            self.transform.rotation = Quaternion.Euler(self.transform.rotation.eulerAngles.x - 90f, self.transform.rotation.eulerAngles.y, self.transform.rotation.eulerAngles.z);
            //self.transform.rotation = Quaternion.Euler(0f, self.transform.eulerAngles.y, self.transform.eulerAngles.z);

            foreach (ObjectLayer layer in layers)
            {
                layer.self = new GameObject($"Layer");
                layer.self.transform.SetParent(self.transform);
                layer.self.transform.position = new Vector3(worldPosition.x, worldPosition.y, layer.index);
                //layer.self.transform.LookAt(GameObject.Find("Planet").transform);
                layer.self.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        
        public GameObject SpawnObject(GameObject obj, ObjectLayer layer, Vector2 position)
        {
            //position.x = Mathf.Clamp(position.x, 0, size.x);
            //position.y = Mathf.Clamp(position.y, 0, size.y);
        
            obj.transform.SetParent(layer.self.transform);
            obj.transform.localPosition = position;
        
            return obj;
        }
    }
    public class TerrainRule
    {
        public string? Parameter { get; set; }
        public string? Operation { get; set; }
        public float? Value { get; set; }
    }
    public class WorldSettings : Feature
    {
        public string name;
        public object[] position;
        public float scale;
        public int seed;
        public object[] height;
        public object[] moisture;
        public object[] variety;
        public string terrainCategory;

        public WorldSettings(string tableName, string name, object[] position, float scale, int seed, object[] height, object[] moisture, object[] variety, string terrainCategory) : base(tableName)
        {
            this.tableName = tableName;
            this.name = name;
            this.position = position;
            this.scale = scale;
            this.seed = seed;
            this.height = height;
            this.moisture = moisture;
            this.variety = variety;
            this.terrainCategory = terrainCategory;
        }
    }
    public class NoiseTypes
    {
        public NoiseSettings[]? height;
        public NoiseSettings[]? moisture;
        public NoiseSettings[]? variety;

        public NoiseTypes(NoiseSettings[]? height, NoiseSettings[]? moisture, NoiseSettings[]? variety)
        {
            this.height = height;
            this.moisture = moisture;
            this.variety = variety;
        }
    }
    public class NoiseSettings : Feature
    {
        public string function;
        public int seed;
        public float noiseScale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public float lerpMin;
        public float lerpMax;
        public float heightMultiply;
        public float heightOffset;

        public NoiseSettings(string tableName, string function, int seed, float noiseScale, int octaves, float persistance, float lacunarity, float lerpMin, float lerpMax, float heightMultiply = 1f, float heightOffset = 0f) : base(tableName)
        {
            this.tableName = tableName;
            this.function = function;
            this.seed = seed;
            this.noiseScale = noiseScale;
            this.octaves = octaves;
            this.persistance = persistance;
            this.lacunarity = lacunarity;
            this.lerpMin = lerpMin;
            this.lerpMax = lerpMax;
            this.heightMultiply = heightMultiply;
            this.heightOffset = heightOffset;
        }
    }
    public class World
    {
        public Vector3 location;
        public float size;
        public int seed;
        public NoiseTypes settings;
        public ThingWithGrafic[] terrains;
        public TerrainRule[][] rules;

        public World(Vector3 location, float size, int seed, NoiseTypes settings, ThingWithGrafic[] terrains, TerrainRule[][] rules)
        {
            this.location = location;
            this.size = size;
            this.seed = seed;
            this.settings = settings;
            this.terrains = terrains;
            this.rules = rules;
        }
    }
    public static class WorldGen
    {
        // WorldSize in X is 20,000 , where in coordinates X -20,000 = 20,000 and are the same vertical line on the sphere
        // WorldSize in Y is 10,000 , where in coordinates Y -10,000 != 10,000 and are the lowest and the highest points on the sphere respectively
        // WorldSize in Z is not limited, but instead ranges from 0 to inf and gets rounded to the previous whole number after being divided by worldSize.z (1,000)
        public static Vector3 worldSize = new Vector3(2000f, 1000f, 1000f);

        public static float sphereRadius = 100f;
        public static float renderDistance = 160f;

        public static Vector2Int chunkSize = new Vector2Int(32, 32);
        public static Vector2[] chunkPoses = Array.Empty<Vector2>();
        public static Chunk[] chunks = Array.Empty<Chunk>();

        public static int seed = 0;

        public static World mainWorld;

        private static GameObject defaultTile = null;

        public static void Generate()
        {
            Camera.main.gameObject.AddComponent<CameraController>();

            ThingWithGrafic[] terrains = [];
            TerrainRule[][] rules = [];
            foreach (string featureName in Help.GetFeatureNames())
            {
                WorldSettings? potentualWorldSettings = Help.GetFeature(featureName, typeof(WorldSettings)) as WorldSettings;
                if (potentualWorldSettings != null)
                {
                    ThingCategory? potentualCategory = Help.GetFeature(potentualWorldSettings.terrainCategory, typeof(ThingCategory)) as ThingCategory;
                    if (potentualCategory != null)
                    {
                        ExtTerrainCategory? potentualExtCategory = Help.GetFeature(potentualWorldSettings.terrainCategory, typeof(ExtTerrainCategory)) as ExtTerrainCategory;
                        if (potentualExtCategory != null)
                        {
                            foreach (string categoryThing in potentualCategory.things)
                            {
                                ThingWithGrafic? potentualThing = Help.GetFeature(categoryThing, typeof(ThingWithGrafic)) as ThingWithGrafic;
                                if (potentualThing != null)
                                {
                                    ExtTerrain? potentualTerrain = Help.GetFeature(categoryThing, typeof(ExtTerrain)) as ExtTerrain;
                                    if (potentualTerrain != null)
                                    {
                                        terrains = Help.Push(terrains, potentualThing);
                                    }
                                }
                            }
                            foreach (TomlArray ruleSet in potentualExtCategory.rules)
                            {
                                TerrainRule[] ruleAr = [];
                                for (int i = 0; i < ruleSet.Count; i++)
                                {
                                    Yipeee.Logger.Log(ruleSet[i].ToSafeString(), ruleSet[i].ToSafeString(), "", Color.cyan);
                                    TomlTable? rule = ruleSet[i] as TomlTable;
                                    if (rule != null) ruleAr = Help.Push(ruleAr, Toml.ToModel<TerrainRule>(Toml.FromModel(rule)));
                                }
                                rules = Help.Push(rules, ruleAr);
                            }
                        }
                    }

                    NoiseSettings[] heightNoises = [];
                    foreach (string noisePath in potentualWorldSettings.height)
                    {
                        NoiseSettings? potentialNoise = Help.GetFeature(noisePath, typeof(NoiseSettings)) as NoiseSettings;
                        if (potentialNoise != null) heightNoises = Help.Push(heightNoises, potentialNoise);
                    }
                    NoiseSettings[] moistureNoises = [];
                    foreach (string noisePath in potentualWorldSettings.moisture)
                    {
                        NoiseSettings? potentialNoise = Help.GetFeature(noisePath, typeof(NoiseSettings)) as NoiseSettings;
                        if (potentialNoise != null) moistureNoises = Help.Push(moistureNoises, potentialNoise);
                    }
                    NoiseSettings[] varietyNoises = [];
                    foreach (string noisePath in potentualWorldSettings.variety)
                    {
                        NoiseSettings? potentialNoise = Help.GetFeature(noisePath, typeof(NoiseSettings)) as NoiseSettings;
                        if (potentialNoise != null) varietyNoises = Help.Push(varietyNoises, potentialNoise);
                    }

                    NoiseTypes noises = new(heightNoises, moistureNoises, varietyNoises);

                    float x = float.Parse(potentualWorldSettings.position[0].ToSafeString());
                    float y = float.Parse(potentualWorldSettings.position[1].ToSafeString());
                    float z = float.Parse(potentualWorldSettings.position[2].ToSafeString());

                    mainWorld = GenerateWorld(new Vector3(x, y, z), potentualWorldSettings.scale, potentualWorldSettings.seed, noises, terrains, rules);
                    terrains = [];
                    rules = [];
                }
            }
        }

        public static void ReGenerate()
        {
            GameObject.Destroy(GameObject.Find("Planet"));

            Generate();
        }

        public static World GenerateWorld(Vector3 location, float size, int seed, NoiseTypes settings, ThingWithGrafic[] terrains, TerrainRule[][] rules)
        {
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject sphere = new GameObject("Planet");
            sphere.transform.localScale = Vector3.one * 2f * sphereRadius;
            sphere.name = "Planet";
            //sphere.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Mat", typeof(Material)) as Material;
            //sphere.GetComponent<MeshRenderer>().material.color = Color.white;
            //sphere.GetComponent<MeshRenderer>().material.mainTexture = Util.LoadTexture(Help.GetCollectionsPath() + "\\Core\\Resources\\Darkness.png");

            float xyDist = 1f;
            float scale = 1f;
            xyDist *= scale;
            float xDist = xyDist * chunkSize.y;
            float yDist = xyDist * chunkSize.x;
            Vector3 localWorldSize = new Vector3(worldSize.x * size, worldSize.y * size, worldSize.z);
            for (float y = -localWorldSize.y; y < localWorldSize.y; y += yDist)
            {
                //CreateDebugPlane(WorldToSphereCoordinates(new Vector3(y, y, 0), sphereRadius), sphere.transform);
                for (float x = -localWorldSize.x; x < localWorldSize.x; x += xDist)
                {
                    if (Mathf.Pow(x + 0f, 2f) + Mathf.Pow(y + 0f, 2f) <= Mathf.Pow(renderDistance, 2f))
                    {
                        Chunk chunk = SpawnChunk(new Vector2(x, y), scale, seed, settings, terrains, rules);
                        chunk.self.transform.position += location;
                    }
                }
            }

            //Chunk chunk = SpawnChunk(new Vector2(0, 0), scale, settings, terrains, rules);
            //chunk.self.transform.position += location;

            //int subdivisions = 8;  // Number of subdivisions
            //int totalTriangles = 20;  // Total number of small triangles
            //for (int i = 0; i < subdivisions; i++)
            //    totalTriangles *= 4;
            //Vector3[] centroids = new Vector3[totalTriangles];  // Array to store centroids
            //
            //// Generate the geodesic grid
            //GeodesicGrid.GenerateGeodesicGrid(subdivisions, centroids);
            //
            //// Output the centroids
            //for (int i = 0; i < centroids.Length; i++)
            //{
            //    if (Vector3.Distance(centroids[i] * sphereRadius, WorldToSphereCoordinates(new Vector3(0, 0, 0), sphereRadius, true)) <= renderDistance
            //     || Vector3.Distance(centroids[i] * sphereRadius, WorldToSphereCoordinates(new Vector3(0, 2500, 0), sphereRadius, true)) <= renderDistance
            //     || Vector3.Distance(centroids[i] * sphereRadius, WorldToSphereCoordinates(new Vector3(0, 5000, 0), sphereRadius, true)) <= renderDistance
            //     || Vector3.Distance(centroids[i] * sphereRadius, WorldToSphereCoordinates(new Vector3(0, 7500, 0), sphereRadius, true)) <= renderDistance
            //     || Vector3.Distance(centroids[i] * sphereRadius, WorldToSphereCoordinates(new Vector3(0, 9990, 0), sphereRadius, true)) <= renderDistance)
            //        CreateDebugPlane(centroids[i] * sphereRadius, 0.01f);
            //}

            return new World(location, size, seed, settings, terrains, rules);
        }
        public static Chunk RegisterChunk(Vector2 position, Chunk chunk)
        {
            chunkPoses = Help.Push(chunkPoses, position);
            chunks = Help.Push(chunks, chunk);

            return chunk;
        }
        public static Chunk? GetChunk(Vector2 position)
        {
            for (int i = 0; i < chunkPoses.Length; i++)
            {
                if (chunkPoses[i] == position) return chunks[i];
            }
            return null;
        }
        public static Chunk SpawnChunk(Vector2 position, float scale, int seed, NoiseTypes noiseTypes, ThingWithGrafic[] terrains, TerrainRule[][] rules)
        {
            float[,] height = new float[chunkSize.x, chunkSize.y];
            float[,] moisture = new float[chunkSize.x, chunkSize.y];
            float[,] variety = new float[chunkSize.x, chunkSize.y];

            void ProcessNoiseMap(float[,] map, NoiseSettings[]? settings)
            {
                if (settings != null)
                {
                    foreach (NoiseSettings noiseSettings in settings)
                    {
                        float[,] perlin = Noise.GenerateNoiseMap(chunkSize.x, chunkSize.y, noiseSettings.seed + seed, noiseSettings.noiseScale, noiseSettings.octaves, noiseSettings.persistance, noiseSettings.lacunarity, position, noiseSettings.lerpMin, noiseSettings.lerpMax);
                        for (int y = 0; y < chunkSize.x; y++)
                        {
                            for (int x = 0; x < chunkSize.x; x++)
                            {
                                float height = perlin[x, y] * noiseSettings.heightMultiply + noiseSettings.heightOffset;
                                if (noiseSettings.function == "Add")
                                    map[x, y] += height / settings.Length;
                                else if (noiseSettings.function == "Multiply")
                                    map[x, y] *= height;
                            }
                        }
                    }
                }
            }

            ProcessNoiseMap(height, noiseTypes.height);
            ProcessNoiseMap(moisture, noiseTypes.moisture);
            ProcessNoiseMap(variety, noiseTypes.variety);

            Chunk chunk = new Chunk(position);
            RegisterChunk(position, chunk);
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int x = 0; x < chunkSize.x; x++)
                {
                    float fx = (x - chunkSize.y / 2) * scale;
                    float fy = (y - chunkSize.x / 2) * scale;

                    Texture2D tex = Texture2D.blackTexture;
                    for (int i = 0; i < rules.Length; i++)
                    {
                        if (rules[i] != null)
                        {
                            bool finalResult = true;

                            foreach (TerrainRule rule in rules[i])
                            {
                                float parameter = rule.Parameter switch
                                {
                                    "height" => height[x, y],
                                    "moisture" => moisture[x, y],
                                    "variety" => variety[x, y],
                                    _ => 0
                                };

                                bool result = rule.Operation switch
                                {
                                    "<=" => parameter <= rule.Value,
                                    ">=" => parameter >= rule.Value,
                                    "<" => parameter < rule.Value,
                                    ">" => parameter > rule.Value,
                                    "==" => parameter == rule.Value,
                                    _ => false
                                };

                                if (!result)
                                {
                                    finalResult = false;
                                    break;
                                }
                            }

                            //Yipeee.Logger.Log(result.ToString(), result.ToString(), "", Color.magenta);

                            if (finalResult && i < terrains.Length)
                            {
                                tex = terrains[i].grafic.placementLoading
                                    ? terrains[i].grafic.LoadTexture()
                                    : terrains[i].grafic.grafic;
                                break;
                            }
                        }
                    }

                    GameObject tile = CreateTile(chunk.self, new Vector2(position.x + fx, position.y + fy), scale, tex);
                    tile.transform.localPosition = new Vector3(tile.transform.localPosition.x, 0f, tile.transform.localPosition.z);

                    //GameObject tile = chunk.SpawnObject(CreateTile(scale), chunk.layers[0], new Vector2(x, y));
                }
            }

            //float perlinScale = scale * 25f / 24f;
            //GameObject perlinVisualised = new GameObject("Perlin");
            //Texture2D perlinTexture = TextureGenerator.TextureFromHeightMap(height);
            //perlinVisualised.AddComponent<SpriteRenderer>().sprite = Sprite.Create(perlinTexture, new Rect(0f, 0f, perlinTexture.width, perlinTexture.height), new Vector2(0f, 0f), 100f);
            //perlinVisualised.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            //perlinVisualised.transform.localScale = new Vector3(chunkSize.x, chunkSize.y) * 4f * perlinScale;
            //perlinVisualised.transform.position = new Vector3(position.x - chunkSize.x * perlinScale / 2f, position.y - chunkSize.y * perlinScale / 2f);
            //perlinVisualised.transform.SetParent(chunk.self.transform);

            return chunk;
        }
        public static float Average(float[,] area, Vector2Int limX, Vector2Int limY)
        {
            float summ = 0f;
            int i = 0;
            for (int y = 0; y < area.GetLength(0); y++)
            {
                for (int x = 0; x < area.GetLength(1); x++)
                {
                    if (y >= limY[0] && y <= limY[1])
                        if (x >= limX[0] && x <= limX[1])
                            summ += area[x, y]; i++;
                }
            }
            return summ / i;
        }
        public static GameObject CreateTile(GameObject chunk, Vector2 position, float scale, Texture2D tileTexture)
        {
            if (defaultTile == null)
            {
                defaultTile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                defaultTile.name = "Tile";
                defaultTile.transform.localScale = Vector3.one * 0.1f;
                defaultTile.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Mat", typeof(Material)) as Material;

                if (defaultTile.GetComponent<MeshRenderer>().material != null)
                {
                    defaultTile.GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }

            GameObject tile = GameObject.Instantiate(defaultTile, chunk.transform);
            tile.transform.position = new Vector3(position.x, position.y);
            tile.transform.localScale *= scale;
            tile.GetComponent<MeshRenderer>().material.mainTexture = tileTexture;

            return tile;
        }
        private static GameObject CreateDebugPlane(Vector3 coordinates, float scale)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = coordinates;
            plane.transform.localScale = Vector3.one * 0.25f * scale;
            plane.transform.SetParent(GameObject.Find("Planet").transform);
            plane.transform.LookAt(Vector3.zero);
            plane.transform.rotation = Quaternion.Euler(plane.transform.rotation.eulerAngles.x - 90f, plane.transform.rotation.eulerAngles.y, plane.transform.rotation.eulerAngles.z);
            plane.name = "Plane";
            plane.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Mat", typeof(Material)) as Material; ;
            plane.GetComponent<MeshRenderer>().material.color = Color.white;
            plane.GetComponent<MeshRenderer>().material.mainTexture = Util.LoadTexture(Help.GetCollectionsPath() + "\\Core\\Resources\\Point.png");
            return plane;
        }
        private static GameObject CreateDebugPlane()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = Vector3.one * 0.25f;
            plane.name = "Plane";
            plane.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Mat", typeof(Material)) as Material; ;
            plane.GetComponent<MeshRenderer>().material.color = Color.white;
            plane.GetComponent<MeshRenderer>().material.mainTexture = Util.LoadTexture(Help.GetCollectionsPath() + "\\Core\\Resources\\Point.png");
            return plane;
        }

        private static GameObject CreateDebugSphere(Vector3 coordinates)
        {
            GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere1.transform.position = coordinates;
            sphere1.transform.localScale = Vector3.one * 20f;
            sphere1.name = "Sphere";
            sphere1.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Mat", typeof(Material)) as Material; ;
            sphere1.GetComponent<MeshRenderer>().material.color = Color.white;
            sphere1.GetComponent<MeshRenderer>().material.mainTexture = Util.LoadTexture(Help.GetCollectionsPath() + "\\Core\\Resources\\Alula.png");
            return sphere1;
        }

        public static Vector3 WorldToSphereCoordinates(Vector3 worldCoordinates, float sphereRadius, bool convertToRhombic = false)
        {
            Vector3 uv3 = WorldToUVCoordinates(worldCoordinates);
            Vector2 rhombic = !convertToRhombic ? new Vector2(uv3.x, uv3.y) : new RhombicCoordinates(uv3.x, uv3.y).vec2;
            return RhombicToSphereCoordinates(rhombic, sphereRadius, uv3.z);
        }
        public static Vector3 WorldToSphereCoordinates(Rhombic3Coordinates rhombicCoordinates, float sphereRadius)
        {
            Vector3 uv3 = WorldToUVCoordinates(rhombicCoordinates.vec3);
            Vector2 rhombic = new Vector2(uv3.x, uv3.y);
            return RhombicToSphereCoordinates(rhombic, sphereRadius, uv3.z);
        }

        public static Vector3 WorldToUVCoordinates(Vector3 worldCoordinates)
        {
            Vector2 uv = Vector2.zero;
            uv.y = Mathf.Clamp(worldCoordinates.y, -worldSize.y, worldSize.y) / worldSize.y;

            uv.x = Mathf.Clamp(worldCoordinates.x, -worldSize.x, worldSize.x) / worldSize.x;

            return new Vector3(uv.x, uv.y, Mathf.Floor(worldCoordinates.z / worldSize.z));
        }

        public static Vector3 RhombicToSphereCoordinates(Vector2 UV, float radius, float z = 0f)
        {
            float pi = Mathf.PI;
            float u = UV.x;
            float v = UV.y;

            //v = 2 / pi * Mathf.Asin(v);

            z = Mathf.Max(z, 0f);

            Vector3 coordinates = Vector3.zero;
            coordinates.x = Mathf.Cos(pi * u) * Mathf.Cos(pi * v / 2f) * (radius + z * radius / 10f);
            coordinates.y = Mathf.Sin(pi * v / 2f) * (radius + z * radius / 10f);
            //coordinates.y = v * (radius + z * radius / 10f);
            coordinates.z = Mathf.Sin(pi * u) * Mathf.Cos(pi * v / 2f) * (radius + z * radius / 10f);

            return coordinates;
        }
    }
}
