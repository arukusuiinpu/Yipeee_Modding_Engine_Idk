using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;
using Yipeee;

namespace Core
{
    public static class Help
    {
        public static string collectionsPath = Application.dataPath.Replace("/Yipeee_Data", "") + "/Collections";
        public static string GetCollectionsPath()
        {
            return collectionsPath;
        }

        public static Feature GetFeature(string culminativeName, Type featureType, string collectionName = "")
        {
            foreach (Collection collection in Yipeee.Yipeee.arrayCollections)
            {
                for (int i = 0; i < collection.featureNames.Length; i++)
                {
                    if ((collection.id == collectionName || collectionName == "") && collection.featureNames[i] == culminativeName)
                    {
                        foreach (var localFeature in collection.features[i].features)
                        {
                            if (localFeature.GetType() == featureType) return (Feature)localFeature;
                        }
                    }
                }
            }
            return null;
        }
        public static Feature[] GetFeatures(string culminativeName, Type featureRef, string collectionName = "")
        {
            Feature[] features = null;
            foreach (Collection collection in Yipeee.Yipeee.arrayCollections)
            {
                for (int i = 0; i < collection.featureNames.Length; i++)
                {
                    if ((collection.id == collectionName || collectionName == "") && collection.featureNames[i] == culminativeName)
                    {
                        foreach (var localFeature in collection.features[i].features)
                        {
                            features = Push(features, (Feature)localFeature);
                        }
                    }
                }
            }
            return features;
        }
        //public static Feature HasFeature(string culminativeName, string featureName = "")
        //{
        //    foreach (Collection collection in Yipeee.Yipeee.arrayCollections)
        //    {
        //        for (int i = 0; i < collection.featureNames.Length; i++)
        //        {
        //            if ((collection.id == collectionName || collectionName == "") && collection.featureNames[i] == culminativeName)
        //            {
        //                foreach (var localFeature in collection.features[i].features)
        //                {
        //                    return (Feature)localFeature;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
        public static Feature GetFeatureByTableName(string culminativeName, string tableName)
        {
            foreach (Collection collection in Yipeee.Yipeee.arrayCollections)
            {
                for (int i = 0; i < collection.featureNames.Length; i++)
                {
                    if (collection.featureNames[i] == culminativeName)
                    {
                        foreach (var localFeature in collection.features[i].features)
                        {
                            if (((Feature)localFeature).tableName == tableName)
                            {
                                return (Feature)localFeature;
                            }
                        }
                    }
                }
            }
            return null;
        }
        public static string[] GetFeatureNames(string collectionName = "")
        {
            string[] featureNames = [];
            foreach (Collection collection in Yipeee.Yipeee.arrayCollections)
            {
                if (collection.id == collectionName || collectionName == "")
                {
                    for (int i = 0; i < collection.featureNames.Length; i++)
                    {
                        featureNames = Push(featureNames, collection.featureNames[i]);
                    }
                }
            }
            return featureNames;
        }

        public static string[] Push(string[] originalArray, string value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static bool[] Push(bool[] originalArray, bool value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static NoiseSettings[] Push(NoiseSettings[] originalArray, NoiseSettings value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static NoiseSettings[][] Push(NoiseSettings[][] originalArray, NoiseSettings[] value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static Feature[] Push(Feature[] originalArray, Feature value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static ThingWithGrafic[] Push(ThingWithGrafic[] originalArray, ThingWithGrafic value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static TerrainRule[] Push(TerrainRule[] originalArray, TerrainRule value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static TerrainRule[][] Push(TerrainRule[][] originalArray, TerrainRule[] value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static Texture2D[] Push(Texture2D[] originalArray, Texture2D value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static Vector2[] Push(Vector2[] originalArray, Vector2 value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
        public static Chunk[] Push(Chunk[] originalArray, Chunk value)
        {
            Array.Resize(ref originalArray, originalArray.Length + 1);
            originalArray[originalArray.GetUpperBound(0)] = value;
            return originalArray;
        }
    }

    public class Core : CollectionClass
    {
        private static string[] texturePaths = [];
        private static Texture2D[] textures = [];

        public static Texture2D GetTexture(string texturePath)
        {
            for (int i = 0; i < texturePaths.Length; i++)
            {
                if (texturePaths[i] == texturePath) return textures[i];
            }
            return Texture2D.blackTexture;
        }

        public override void Initialize()
        {
            
        }

        public override void PostInitialize()
        {
            foreach (string featureName in Help.GetFeatureNames())
            {
                ThingWithGrafic? potentualThing = Help.GetFeature(featureName, typeof(ThingWithGrafic)) as ThingWithGrafic;
                if (potentualThing != null)
                {
                    if (potentualThing.grafic is TileGrafic)
                    {
                        TileGrafic? tileGrafic = potentualThing.grafic as TileGrafic;
                        Texture2D texture2D = Util.LoadTexture(Help.GetCollectionsPath() + tileGrafic.graficPath);
                        for (int y = 0; y < tileGrafic.tilesY; y++)
                        {
                            for (int x = 0; x < tileGrafic.tilesX; x++)
                            {
                                Color[] c = texture2D.GetPixels(x * tileGrafic.tileSize, y * tileGrafic.tileSize, tileGrafic.tileSize, tileGrafic.tileSize);
                                Texture2D croppedTexture = new Texture2D(tileGrafic.tileSize, tileGrafic.tileSize);

                                croppedTexture.SetPixels(c);
                                croppedTexture.Apply();

                                texturePaths = Help.Push(texturePaths, tileGrafic.graficPath + x.ToString() + ";" + y.ToString());
                                textures = Help.Push(textures, croppedTexture);
                            }
                        }
                    }
                    else
                    {
                        texturePaths = Help.Push(texturePaths, potentualThing.grafic.graficPath);
                        textures = Help.Push(textures, Util.LoadTexture(Help.GetCollectionsPath() + potentualThing.grafic.graficPath));
                    }
                }
            }
            foreach (string featureName in Help.GetFeatureNames())
            {
                ThingWithGrafic? potentualThing = Help.GetFeature(featureName, typeof(ThingWithGrafic)) as ThingWithGrafic;
                if (potentualThing != null && !potentualThing.grafic.placementLoading)
                {
                    potentualThing.grafic.LoadTexture();
                }
            }
            WorldGen.ReGenerate();
        }
    }
}
