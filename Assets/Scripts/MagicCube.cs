using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts
{
   /// <summary>
   /// Handle and keep track for the unit cubes.
   /// Check winning condition.
   /// </summary>
   public class MagicCube : MonoBehaviour
   { 
      [SerializeField] private UnitCube unitCubePrefab;
      [SerializeField] private float rotationDuration;
      
      private UnitCube[,,] unitCube;
      private bool currentlyRotating;

      private void Awake()
      {
         currentlyRotating = false;
      }
      
      // For saving data in Json.
      public MagicCubeInfo GetMagicCubeInfo()
      {
         var cubeInfo = new MagicCubeInfo
         {
            infos = new List<UnitCubeInfo>(PlayerSettings.CubeSize * PlayerSettings.CubeSize * PlayerSettings.CubeSize)
         };

         for (var z = 0; z < PlayerSettings.CubeSize; z++)
         for (var y = 0; y < PlayerSettings.CubeSize; y++)
         for (var x = 0; x < PlayerSettings.CubeSize; x++)
            cubeInfo.infos.Add(unitCube[x, y, z].GetUnitCubeFaces());
         return cubeInfo;
      }

      // For saving data in Json.
      public void SetMagicCubeInfo(MagicCubeInfo cubeInfo)
      {
         for (var z = 0; z < PlayerSettings.CubeSize; z++)
         for (var y = 0; y < PlayerSettings.CubeSize; y++)
         for (var x = 0; x < PlayerSettings.CubeSize; x++)
            unitCube[x, y, z]
               .SetUnitCubeFaces(cubeInfo.infos[(x * PlayerSettings.CubeSize + y) * PlayerSettings.CubeSize + z]);
      }

      
      public void GenerateCube()
      {
         unitCube = new UnitCube[PlayerSettings.CubeSize, PlayerSettings.CubeSize, PlayerSettings.CubeSize];
         CreateCube();
      }

      private void CreateCube()
      {
         for (var z = 0; z < PlayerSettings.CubeSize; z++)
         for (var y = 0; y < PlayerSettings.CubeSize; y++)
         for (var x = 0; x < PlayerSettings.CubeSize; x++)
         {
            var newSmallCube = Instantiate(unitCubePrefab);
            unitCube[x, y, z] = newSmallCube;
            newSmallCube.transform.parent = transform;
            newSmallCube.transform.localPosition = new Vector3(x - PlayerSettings.CubeSize * 0.5f + 0.5f,
               y - PlayerSettings.CubeSize * 0.5f + 0.5f, z - PlayerSettings.CubeSize * 0.5f + 0.5f);

            if (PlayerSettings.IsLoaded) continue;
            DetectActiveCubeFaces(newSmallCube, x, y, z);
            unitCube[x, y, z].SetMaterials();
         }
      }

      // Winning condition check.
      private bool CheckIsWin()
      {
         var unitCubeFace = new List<CubeFace>();
         for (var z = 0; z < PlayerSettings.CubeSize; z++)
         for (var y = 0; y < PlayerSettings.CubeSize; y++)
         for (var x = 0; x < PlayerSettings.CubeSize; x++)
            unitCube[x, y, z].GetCubeFacesInPlay(unitCubeFace);

         var southColor = unitCubeFace.FirstOrDefault(i => i.Info.Direction == ECubeDirection.South)?.Info.Color ??
                          ECubeColors.Black;
         var northColor = unitCubeFace.FirstOrDefault(i => i.Info.Direction == ECubeDirection.North)?.Info.Color ??
                          ECubeColors.Black;
         var eastColor = unitCubeFace.FirstOrDefault(i => i.Info.Direction == ECubeDirection.East)?.Info.Color ??
                         ECubeColors.Black;
         var westColor = unitCubeFace.FirstOrDefault(i => i.Info.Direction == ECubeDirection.West)?.Info.Color ??
                         ECubeColors.Black;
         var topColor = unitCubeFace.FirstOrDefault(i => i.Info.Direction == ECubeDirection.Top)?.Info.Color ??
                        ECubeColors.Black;
         var bottomColor = unitCubeFace.FirstOrDefault(i => i.Info.Direction == ECubeDirection.Bottom)?.Info.Color ??
                           ECubeColors.Black;
         
         return unitCubeFace.All(unitCubeFaces =>
            (unitCubeFaces.Info.Direction != ECubeDirection.South || unitCubeFaces.Info.Color == southColor)
            && (unitCubeFaces.Info.Direction != ECubeDirection.North || unitCubeFaces.Info.Color == northColor)
            && (unitCubeFaces.Info.Direction != ECubeDirection.East || unitCubeFaces.Info.Color == eastColor)
            && (unitCubeFaces.Info.Direction != ECubeDirection.West || unitCubeFaces.Info.Color == westColor)
            && (unitCubeFaces.Info.Direction != ECubeDirection.Top || unitCubeFaces.Info.Color == topColor)
            && (unitCubeFaces.Info.Direction != ECubeDirection.Bottom || unitCubeFaces.Info.Color == bottomColor));
      }

      // Cube animation in the beginning of the game.
      public IEnumerator ShuffleCube(int shuffleTimes, float shuffleRotationTime)
      {
         PlayerSettings.IsShuffling = true;
         var oldRotationTime = rotationDuration;
         rotationDuration = shuffleRotationTime;

         for (var i = 0; i < shuffleTimes; i++)
         {
            PlayerSettings.FaceRotation = true;
            var rotationType = Random.Range(0, 3);
            var rotationIndex = Random.Range(0, PlayerSettings.CubeSize);
            var rotationAngle = Random.Range(-1, 1) < 0 ? -90 : 90;
            switch (rotationType)
            {
               case 0:
                  yield return StartCoroutine(RotateAlongX(rotationAngle, rotationIndex));
                  break;
               case 1:
                  yield return StartCoroutine(RotateAlongY(rotationAngle, rotationIndex));
                  break;
               case 2:
                  yield return StartCoroutine(RotateAlongZ(rotationAngle, rotationIndex));
                  break;
            }
            GameManager.Instance.PlayShuffleAudio();
         }

         rotationDuration = oldRotationTime;
         PlayerSettings.IsShuffling = false;
      }

      // Generate mask for certain axis.
      private Vector3Int MaskVector3Value(Vector3Int target, Vector3Int mask, int value)
      {
         var result = new Vector3Int(value, value, value);
         target -= result;
         target = Vector3Int.Scale(target, mask);
         target += result;
         return target;
      }

      // Get axis mask.
      private Vector3Int CreateMask(EAxis rotationAxis)
      {
         return rotationAxis switch
         {
            EAxis.X => new Vector3Int(0, 1, 1),
            EAxis.Y => new Vector3Int(1, 0, 1),
            EAxis.Z => new Vector3Int(1, 1, 0),
            _ => Vector3Int.one
         };
      }

      // Get reverse axis mask.
      private Vector3Int CreateMaskRev(EAxis rotationAxis)
      {
         return rotationAxis switch
         {
            EAxis.X => new Vector3Int(1, 0, 0),
            EAxis.Y => new Vector3Int(0, 1, 0),
            EAxis.Z => new Vector3Int(0, 0, 1),
            _ => Vector3Int.zero
         };
      }

      // Rotate animation.
      private IEnumerator RotateFace(float angle, int rotationIndex, EAxis rotationAxis)
      {
         yield return null;
         if (currentlyRotating || PlayerSettings.SettingsOn || PlayerSettings.IsGameEnd ||
             !PlayerSettings.FaceRotation) yield break;
         currentlyRotating = true;
         var newRotation = new GameObject
         {
            transform =
            {
               position = new Vector3(0f, 0f, 0f)
            }
         };

         float elapsedTime = 0;
         var mask = CreateMask(rotationAxis);
         for (var x = 0; x < PlayerSettings.CubeSize; x++)
         for (var y = 0; y < PlayerSettings.CubeSize; y++)
         for (var z = 0; z < PlayerSettings.CubeSize; z++)
         {
            var index = MaskVector3Value(new Vector3Int(x, y, z), mask, rotationIndex);
            unitCube[index.x, index.y, index.z].transform.parent = newRotation.transform;
         }



         var angMask = CreateMaskRev(rotationAxis);
         var quaternion = Quaternion.Euler(((Vector3)angMask) * angle);
         while (elapsedTime < rotationDuration)
         {
            newRotation.transform.rotation =
               Quaternion.Lerp(newRotation.transform.rotation, quaternion, (elapsedTime / rotationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
         }

         newRotation.transform.rotation = quaternion;
         for (var x = 0; x < PlayerSettings.CubeSize; x++)
         for (var y = 0; y < PlayerSettings.CubeSize; y++)
         for (var z = 0; z < PlayerSettings.CubeSize; z++)
         {
            var index = MaskVector3Value(new Vector3Int(x, y, z), mask, rotationIndex);
            unitCube[index.x, index.y, index.z].transform.parent = transform;
         }
         
         unitCube = CalculatePositionAfterRotation();
         ChangeCubeFacesDirection(angle, rotationIndex, rotationAxis);

         Destroy(newRotation);
         currentlyRotating = false;

         if (!PlayerSettings.IsShuffling && CheckIsWin())
            GameManager.Instance.GameEnd();

         PlayerSettings.FaceRotation = false;
         yield return new WaitForSeconds(0.1f);
      }

      public IEnumerator RotateAlongY(float angle, int rotationIndex)
      {
         yield return RotateFace(angle, rotationIndex, EAxis.Y);
      }

      public IEnumerator RotateAlongX(float angle, int rotationIndex)
      {
         yield return RotateFace(angle, rotationIndex, EAxis.X);
      }

      public IEnumerator RotateAlongZ(float angle, int rotationIndex)
      {
         yield return RotateFace(angle, rotationIndex, EAxis.Z);
      }

      // Calculate and reassign for each unit cube it's own position and direction after rotation.
      private UnitCube[,,] CalculatePositionAfterRotation()
      {
         var multiFactor = PlayerSettings.CubeSize / 2f - 0.5f;
         var tempCubeUnits = new UnitCube[PlayerSettings.CubeSize, PlayerSettings.CubeSize, PlayerSettings.CubeSize];
         var dic = new Dictionary<Vector3, Vector3Int>();

         for (var x2 = 0; x2 < PlayerSettings.CubeSize; x2++)
         for (var y2 = 0; y2 < PlayerSettings.CubeSize; y2++)
         for (var z2 = 0; z2 < PlayerSettings.CubeSize; z2++)
         {
            var vec = unitCube[x2, y2, z2].transform.position * 10;
            vec.x = Mathf.Round(vec.x) / 10;
            vec.y = Mathf.Round(vec.y) / 10;
            vec.z = Mathf.Round(vec.z) / 10;
            dic.Add(vec, new Vector3Int(x2, y2, z2));
         }

         for (var x = 0; x < PlayerSettings.CubeSize; x++)
         for (var y = 0; y < PlayerSettings.CubeSize; y++)
         for (var z = 0; z < PlayerSettings.CubeSize; z++)
         {
            var index = new Vector3(-multiFactor + x, -multiFactor + y, -multiFactor + z);
            if (dic.TryGetValue(index, out var value))
            {
               tempCubeUnits[x, y, z] = unitCube[value.x, value.y, value.z];
            }
         }

         return tempCubeUnits;
      }
      
      private void ChangeCubeFacesDirection(float angle, int rotationIndex, EAxis rotationAlong)
      {
         switch (rotationAlong)
         {
            // Rotate along X
            case EAxis.X:
            {
               for (var y = 0; y < PlayerSettings.CubeSize; y++)
               for (var z = 0; z < PlayerSettings.CubeSize; z++)
                  if (angle > 0)
                     unitCube[rotationIndex, y, z].ChangeDirectionsAfterXRotationClockwise();
                  else
                     unitCube[rotationIndex, y, z].ChangeDirectionsAfterXRotationCounterClockwise();
            }
               break;
            
         case EAxis.Y:
            {
               for (var x = 0; x < PlayerSettings.CubeSize; x++)
               for (var z = 0; z < PlayerSettings.CubeSize; z++)
                  if (angle > 0)
                     unitCube[x, rotationIndex, z].ChangeDirectionsAfterYRotationClockwise();
                  else
                     unitCube[x, rotationIndex, z].ChangeDirectionsAfterYRotationCounterClockwise();
               break;
            }

            case EAxis.Z:
            {
               for (var x = 0; x < PlayerSettings.CubeSize; x++)
               for (var y = 0; y < PlayerSettings.CubeSize; y++)
                  if (angle > 0)
                     unitCube[x, y, rotationIndex].ChangeDirectionsAfterZRotationClockwise();
                  else
                     unitCube[x, y, rotationIndex].ChangeDirectionsAfterZRotationCounterClockwise();
               break;
            }
            case EAxis.None:
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(rotationAlong), rotationAlong, null);
         }
      }

      private void DetectActiveCubeFaces(UnitCube cube, int x, int y, int z)
      {
         switch (x)
         {
            case 0 when y == 0 && z == 0:
               cube.SetBottomWestSouthCorner();
               break;
            case > 0 when x < PlayerSettings.CubeSize - 1 && y == 0 && z == 0:
               cube.SetSouthBottomSide();
               break;
            case 0 when y == 0 && z > 0 && z < PlayerSettings.CubeSize - 1:
               cube.SetWestBottomSide();
               break;
            case > 0 when x < PlayerSettings.CubeSize - 1 && y == 0 && z > 0 && z < PlayerSettings.CubeSize - 1:
               cube.SetBottomMiddle();
               break;
            case 0 when y == 0 && z == PlayerSettings.CubeSize - 1:
               cube.SetBottomWestNorthCorner();
               break;
            case > 0 when x < PlayerSettings.CubeSize - 1 && y == 0 && z == PlayerSettings.CubeSize - 1:
               cube.SetNorthBottomSide();
               break;
            case 0 when y > 0 && y < PlayerSettings.CubeSize - 1 && z == 0:
               cube.SetMiddleWestSouthCorner();
               break;
            case > 0 when x < PlayerSettings.CubeSize - 1 && y > 0 && y < PlayerSettings.CubeSize - 1 && z == 0:
               cube.SetSouthMiddleSide();
               break;
            case 0 when y > 0 && y < PlayerSettings.CubeSize - 1 && z == PlayerSettings.CubeSize - 1:
               cube.SetMiddleWestNorthCorner();
               break;
            case > 0 when x < PlayerSettings.CubeSize - 1 && y > 0 && y < PlayerSettings.CubeSize - 1 &&
                          z == PlayerSettings.CubeSize - 1:
               cube.SetNorthMiddleSide();
               break;
            case 0 when y == PlayerSettings.CubeSize - 1 && z == 0:
               cube.SetTopWestSouthCorner();
               break;
            case > 0 when x < PlayerSettings.CubeSize - 1 && y == PlayerSettings.CubeSize - 1 && z == 0:
               cube.SetTopBottomSide();
               break;
            case 0 when y == PlayerSettings.CubeSize - 1 && z > 0 && z < PlayerSettings.CubeSize - 1:
               cube.SetWestTopSide();
               break;
            case > 0 when x < PlayerSettings.CubeSize - 1 && y == PlayerSettings.CubeSize - 1 && z > 0 &&
                          z < PlayerSettings.CubeSize - 1:
               cube.SetTopMiddle();
               break;
            case 0 when y == PlayerSettings.CubeSize - 1 && z == PlayerSettings.CubeSize - 1:
               cube.SetTopWestNorthCorner();
               break;
            case > 0 when x < PlayerSettings.CubeSize - 1 && y == PlayerSettings.CubeSize - 1 &&
                          z == PlayerSettings.CubeSize - 1:
               cube.SetNorthTopSide();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y == 0 && z == 0:
               cube.SetBottomEastSouthCorner();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y == 0 && z > 0 && z < PlayerSettings.CubeSize - 1:
               cube.SetEastBottomSide();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y == 0 && z == PlayerSettings.CubeSize - 1:
               cube.SetBottomEastNorthCorner();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y > 0 && y < PlayerSettings.CubeSize - 1 && z == 0:
               cube.SetMiddleEastSouthCorner();
               break;
            case 0 when y > 0 && y < PlayerSettings.CubeSize - 1 && z > 0 && z < PlayerSettings.CubeSize - 1:
               cube.SetWestMiddleSide();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y > 0 && y < PlayerSettings.CubeSize - 1 && z > 0 &&
                          z < PlayerSettings.CubeSize - 1:
               cube.SetEastMiddleSide();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y > 0 && y < PlayerSettings.CubeSize - 1 &&
                          z == PlayerSettings.CubeSize - 1:
               cube.SetMiddleEastNorthCorner();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y == PlayerSettings.CubeSize - 1 && z == 0:
               cube.SetTopEastSouthCorner();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y == PlayerSettings.CubeSize - 1 && z > 0 &&
                          z < PlayerSettings.CubeSize - 1:
               cube.SetEastTopSide();
               break;
            case > 0 when x == PlayerSettings.CubeSize - 1 && y == PlayerSettings.CubeSize - 1 &&
                          z == PlayerSettings.CubeSize - 1:
               cube.SetTopEastNorthCorner();
               break;
         }
      }
   }
}

