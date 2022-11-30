using System;
using System.Collections.Generic;
using System.Text;
using World;

namespace Serialization
{
    internal static class Serializers
    {
        public static void SerializeTwoDimensional<T>(T[,] source, out List<TwoDimensionalPackage<T>> output)
        {
            // Convert our unserializable array into a serializable list
            output = new List<TwoDimensionalPackage<T>>();
            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    output.Add(new TwoDimensionalPackage<T>(i, j, source[i, j]));
                }
            }
        }
        public static void DeserializeTwoDimensional<T>(List<TwoDimensionalPackage<T>> source, int dimension1, int dimension2, out T[,] output)
        {
            // Convert the serializable list into our unserializable array
            output = new T[dimension1, dimension2];
            foreach (var package in source)
            {
                output[package.Index0, package.Index1] = package.Element;
            }
        }

        public static void SerializeThreeDimensional<T>(T[,,] source, out List<ThreeDimensionalPackage<T>> output)
        {
            // Convert our unserializable array into a serializable list
            output = new List<ThreeDimensionalPackage<T>>();
            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    for (int k = 0; k < source.GetLength(1); k++)
                    {
                        output.Add(new ThreeDimensionalPackage<T>(i, j, k, source[i, j, k]));
                    }
                }
            }
        }
        public static void DeserializeThreeDimensional<T>(List<ThreeDimensionalPackage<T>> source, int dimension1, int dimension2, int dimension3, out T[,,] output)
        {
            // Convert the serializable list into our unserializable array
            output = new T[dimension1, dimension2, dimension3];
            foreach (var package in source)
            {
                output[package.Index0, package.Index1, package.Index2] = package.Element;
            }
        }

        public static void BlockSerializer(Block block, ref StringBuilder stringBuilder)
        {
            stringBuilder.AppendJoin(',',
                block.x,
                block.y,
                block.z,
                block.worldPosition.x,
                block.worldPosition.y,
                block.worldPosition.z,
                (int)block.type,
                block.passable ? 't' : 'f',
                block.buildable ? 't' : 'f',
                '\n');
        }
        public static void DeserializeBlock(string blockString, out Block block)
        {
            var sub_strings = blockString.Split(',');
            block = new()
            {
                x = int.Parse(sub_strings[0]),
                y = int.Parse(sub_strings[1]),
                z = int.Parse(sub_strings[2]),
                worldPosition = new()
                {
                    x = int.Parse(sub_strings[3]),
                    y = int.Parse(sub_strings[4]),
                    z = int.Parse(sub_strings[5]),
                },
                type = (BlockType)Enum.Parse(typeof(BlockType), sub_strings[6]),
                passable = sub_strings[7] == "t" ? true : false,
                buildable = sub_strings[8] == "t" ? true : false,
            };
        }
    }
}
