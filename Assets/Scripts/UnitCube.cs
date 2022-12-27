using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts
{
   /// <summary>
   /// Hold the unit cube data such as playable faces, colors and directions.
   /// Build the magic cube.
   /// </summary>
   public class UnitCube : MonoBehaviour 
   {
      [SerializeField] private List<CubeMatInfo> infos;
      [SerializeField] private CubeMatInfo defaultInfo; 
      [SerializeField] private CubeFace[] cubeFaces;

      private void Awake()
      {
         foreach (var t in cubeFaces)
         {
            t.Info = new CubeFaceInfo
            {
               Direction = (ECubeDirection)Enum.Parse(typeof(ECubeDirection), t.name, true)
            };
         }
      }
      
      public UnitCubeInfo GetUnitCubeFaces()
      {
         var cubeInfo = new UnitCubeInfo();
         var infos = new List<CubeFaceInfo>(cubeFaces.Length);
         infos.AddRange(cubeFaces.Select(t => t.Info));
         cubeInfo.infos = infos;
         return cubeInfo;
      }

      public void SetUnitCubeFaces(UnitCubeInfo infos)
      {
         for (var i = 0; i < infos.infos.Count; i++)
            cubeFaces[i].Info = infos.infos[i];
         
         SetupPlayableFace();
         SetLoadedColor();
      }

      public void GetCubeFacesInPlay(List<CubeFace> cubes)
      {
         cubes.AddRange(cubeFaces.Where(t =>  t.Info.InPlay));
      }

      public void ChangeDirectionsAfterYRotationClockwise()
      {
         ChangeDirections(true, new List<ECubeDirection>() { ECubeDirection.Top ,ECubeDirection.Bottom});
      }
      public void ChangeDirectionsAfterYRotationCounterClockwise()
      {
         ChangeDirections(false, new List<ECubeDirection>() { ECubeDirection.Top ,ECubeDirection.Bottom});
      }

      // Shift the direction for each axis.
      // Filter the non usable directions.
      // X axis rotation: top, south, bottom, north.
      // Y axis rotation: North, East, South, West.
      // Z axis rotation: Top, Wast, Bottom, East.
      private void ChangeDirections(bool isClockwise, ICollection<ECubeDirection> filters)
      {
         foreach (var t in cubeFaces)
         {
            if (! t.Info.InPlay || filters.Contains( t.Info.Direction)) continue;
            do
            {
               t.Info.Direction = isClockwise ?  t.Info.Direction.Next() :  t.Info.Direction.Pre();

            } while (filters.Contains( t.Info.Direction));
         }
      }

      public void ChangeDirectionsAfterXRotationClockwise()
      {
         ChangeDirections(true, new List<ECubeDirection>() { ECubeDirection.East ,ECubeDirection.West});
     
      }
      public void ChangeDirectionsAfterXRotationCounterClockwise()
      {
         ChangeDirections(true, new List<ECubeDirection>() { ECubeDirection.East ,ECubeDirection.West});
      }
      public void ChangeDirectionsAfterZRotationClockwise()
      {
         ChangeDirections(true, new List<ECubeDirection>() { ECubeDirection.North ,ECubeDirection.South});
      
      }
      public void ChangeDirectionsAfterZRotationCounterClockwise()
      {
         ChangeDirections(false, new List<ECubeDirection>() { ECubeDirection.North ,ECubeDirection.South});
      }

      public void SetMaterials()
      {
         foreach (var t in cubeFaces)
         {
            var info =  t.Info.InPlay ? infos.FirstOrDefault(i => i.Direction ==  t.Info.Direction) : defaultInfo;
            t.SetMaterial(info.Mat);
            t.Info.Color = info.CubeColor;
         }
      }

      // Disable the hidden playable faces.
      private void SetupPlayableFace(ICollection<ECubeDirection> filters)
      {
         foreach (var t in cubeFaces)
         {
            t.Info.InPlay = true;
            if (filters.Contains( t.Info.Direction)) continue;
            t.Info.InPlay = false;
            Destroy(t.GetComponent<Collider>());
         }
      }

      private void SetLoadedColor()
      {
         foreach (var t in cubeFaces)
         {
            var info = infos.FirstOrDefault(i => i.Direction == t.Info.Direction) ?? defaultInfo;
            t.SetMaterial(info.Mat);
         
         }
      }

      private void SetupPlayableFace()
      {
         foreach (var t in cubeFaces)
         {
            if (!t.Info.InPlay)
               Destroy(t.GetComponent<Collider>());
         }
      }

      // Bottom Row
      public void SetBottomWestSouthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom, ECubeDirection.West, ECubeDirection.South });
     
      }
      public void SetBottomEastSouthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom, ECubeDirection.East, ECubeDirection.South });
      
      }
      public void SetBottomWestNorthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom, ECubeDirection.West, ECubeDirection.North });
     
      }
      public void SetBottomEastNorthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom, ECubeDirection.East, ECubeDirection.North });
      
      }
      public void SetSouthBottomSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom,ECubeDirection.South });
      
      }
      public void SetEastBottomSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom,ECubeDirection.East });
     
      }
      public void SetWestBottomSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom,ECubeDirection.West });
      
      }
      public void SetNorthBottomSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom,ECubeDirection.North });
     
      }
      public void SetBottomMiddle()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Bottom });
     
      }
   
      // Middle Row
      public void SetMiddleWestSouthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.South,ECubeDirection.West });
      
      }
      public void SetSouthMiddleSide()
      {SetupPlayableFace(new List<ECubeDirection>()
         {ECubeDirection.South });
     
      }
      public void SetMiddleEastSouthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.East,ECubeDirection.South });
      
      }
      public void SetWestMiddleSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.West });
      }
      public void SetEastMiddleSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.East });
      }
      public void SetMiddleWestNorthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.North,ECubeDirection.West });
     
      }
      public void SetNorthMiddleSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.North});
    
      }
      public void SetMiddleEastNorthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.North,ECubeDirection.East });
      }

      // Top Row
      public void SetTopWestSouthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top,ECubeDirection.West, ECubeDirection.South});
      }
      public void SetTopEastSouthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top,ECubeDirection.East, ECubeDirection.South});
      
      }
      public void SetTopWestNorthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top,ECubeDirection.West, ECubeDirection.North});
      
      }
      public void SetTopEastNorthCorner()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top,ECubeDirection.East, ECubeDirection.North});
     
      }
      public void SetTopBottomSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top,ECubeDirection.Top, ECubeDirection.South});
     
      }
      public void SetEastTopSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top, ECubeDirection.East});
     
      }
      public void SetWestTopSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top,ECubeDirection.Top, ECubeDirection.West});
      }
      public void SetNorthTopSide()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top,ECubeDirection.North});
     
      }
      public void SetTopMiddle()
      {
         SetupPlayableFace(new List<ECubeDirection>()
            { ECubeDirection.Top});
      }
   }
}

