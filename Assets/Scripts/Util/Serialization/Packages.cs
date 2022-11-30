using System;

namespace Serialization
{
    [Serializable]
    struct TwoDimensionalPackage<T>
    {
        public int Index0;
        public int Index1;
        public T Element;
        public TwoDimensionalPackage(int idx0, int idx1, T element)
        {
            Index0 = idx0;
            Index1 = idx1;
            Element = element;
        }
    }

    [Serializable]
    struct ThreeDimensionalPackage<T>
    {
        public int Index0;
        public int Index1;
        public int Index2;
        public T Element;
        public ThreeDimensionalPackage(int idx0, int idx1, int idx2, T element)
        {
            Index0 = idx0;
            Index1 = idx1;
            Index2 = idx2;
            Element = element;
        }
    }
}
