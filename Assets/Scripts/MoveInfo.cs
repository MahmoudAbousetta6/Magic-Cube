namespace Scripts
{
    /// <summary>
    /// Hold the movement data of the cube such as angle, index of the selected part and target axis.
    /// </summary>
    [System.Serializable]
    public class MoveInfo
    {
        public float Angle { get; set; }
        public int Index { get; set; }
        public EAxis Axis { get; set; }

        public MoveInfo ReverseMove()
        {
            return new MoveInfo
            {
                Angle = -Angle,
                Axis = Axis,
                Index = Index
            };
        }
    }
}