using System;
using System.Collections.Generic;

namespace Scripts
{
    /// <summary>
    /// Contain the info of the unit cube to handle save and load data.
    /// </summary>
    [Serializable]
    public class UnitCubeInfo
    {
        public List<CubeFaceInfo> infos;
    }
}