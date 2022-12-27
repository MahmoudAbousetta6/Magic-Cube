using System;
using System.Collections.Generic;

namespace Scripts
{
    /// <summary>
    /// Contain the info of the magic cube to handle save and load data.
    /// </summary>
    [Serializable]
    public class MagicCubeInfo
    {
        public List<UnitCubeInfo> infos;
        public int cubeSize;
    }
}