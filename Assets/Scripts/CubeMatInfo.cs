using UnityEngine;

namespace Scripts
{ 
    /// <summary>
    /// Contain the data of each material and direction.
    /// </summary>
    [System.Serializable]
    public class CubeMatInfo
    {
        [SerializeField] private Material mat;
        [SerializeField] private ECubeColors color;
        [SerializeField] private ECubeDirection direction;

        public Material Mat
        {
            get => mat;
            set => mat = value;
        }

        public ECubeColors CubeColor
        {
            get => color;
            set => color = value;
        }

        public ECubeDirection Direction
        {
            get => direction;
            set => direction = value;
        }
    }
}