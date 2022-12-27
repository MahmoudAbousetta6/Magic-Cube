namespace Scripts
{
    /// <summary>
    /// Contain the info of each cube face to handle save and load data.
    /// </summary>
    [System.Serializable]
    public class CubeFaceInfo
    {
        public bool InPlay { get; set; }
        public ECubeDirection Direction { get; set; }
        public ECubeColors Color { get; set; }
    }
}