using System;

namespace Scripts
{
    /// <summary>
    /// This extension handles the iteration of going next or back for an enum.
    /// </summary>
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
        {
            return DoStep(src, 1);
        }

        private static T DoStep<T>(T src, int index) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            var Arr = (T[])Enum.GetValues(src.GetType());
            var j = Array.IndexOf<T>(Arr, src) + index;
            j = (j + Arr.Length) % Arr.Length;
            return Arr[j];
        }

        public static T Pre<T>(this T src) where T : struct
        {
            return DoStep(src, -1);
        }
    }
}