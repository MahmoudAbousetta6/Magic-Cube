using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
   /// <summary>
   /// This class handle inputs either a touch or mouse for cube rotation, movement and zooming.
   /// </summary>
   public class InputHandler : MonoBehaviour
   {
      [SerializeField] private  float cameraRotationRadius = 5f;
      [SerializeField] private  float zoomSpeed = 0.5f;
      [SerializeField] private Transform cameraPivot;

      private MagicCube magicCube;
      private Camera mainCam;
      
      
      private GameObject firstHit;
      
      private Vector3 localRotation;
      private Vector3 firstHitNormal;
      private Vector3 firstHitCenter;
      private Vector3 secondHitNormal;
      private Vector3 secondHitCenter;
      
      private float offset;
      
      private const float rotateAngle = 90f;

      private Stack<MoveInfo> playerMoves;

      private void Awake()
      {
         PlayerSettings.CameraDisable = true;
         PlayerSettings.CubeRotation = false;
         offset = PlayerSettings.CubeSize * 0.5f - 0.5f;
         playerMoves = new Stack<MoveInfo>();
      }

      private void Start()
      {
         mainCam = Camera.main;
      }

      // Handle the movement undo.
      public void Undo()
      {
         if (playerMoves.Count <= 0 || PlayerSettings.CubeRotation || PlayerSettings.FaceRotation) return;
         PlayerSettings.CubeRotation = true;
         PlayerSettings.FaceRotation = true;
         RotateCube(playerMoves.Pop().ReverseMove());
      }

      public void SetMagicCube(MagicCube _magicCube)
      {
         magicCube = _magicCube;
      }

      // Handle camera zoom.
      private void UpdateZoom(float delta)
      {
         mainCam.fieldOfView += delta * zoomSpeed * 0.5f;
         mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView, 20f, 90f);
      }

      private void LateUpdate()
      {
         if (magicCube is null) return;

         if (Input.touchCount <= 0) return;
         var touch = Input.GetTouch(0);

         // Check if the cube was touched
         var whatCubeTouched = mainCam.ScreenPointToRay(touch.position);
         var hitRay = new RaycastHit();
         var cubeWasTouched = false;

         if (Physics.Raycast(whatCubeTouched, out hitRay))
         {
            cubeWasTouched = hitRay.transform.gameObject.GetComponent<CubeFace>().Info.InPlay;
         }

         var targetLocation = Quaternion.Euler(localRotation.y, localRotation.x, 0f);
         if (cubeWasTouched && Input.touchCount == 1 && !PlayerSettings.CubeRotation)
         {
            // Handle the first and end touch after hitting the cube for rotation.
            switch (touch)
            {
               // Record initial touch position.
               case { phase: TouchPhase.Began }:
                  PlayerSettings.FaceRotation = true;
                  firstHitNormal = hitRay.normal;
                  firstHitCenter = hitRay.transform.gameObject.GetComponent<Renderer>().bounds.center;
                  firstHit = hitRay.transform.parent.gameObject;
                  break;

               // Report that a direction has been chosen when the finger is lifted.
               case { phase: TouchPhase.Ended }:
                  if (!PlayerSettings.FaceRotation) break;
                  secondHitNormal = hitRay.normal;
                  secondHitCenter = hitRay.transform.gameObject.GetComponent<Renderer>().bounds.center;
                  var move = secondHitCenter - firstHitCenter;
                  move.Normalize();
                  CalculateCubeInteractions(move);
                  break;

            }
         }

         else
         {
            // Handle rotating the cube around.
            if (PlayerSettings.CameraDisable && !PlayerSettings.SettingsOn && !PlayerSettings.IsGameEnd)
            {
               if (Input.touchCount == 1 &&
                   !(touch.position.x > Screen.width * 0.80 && touch.position.y < Screen.height * 0.20) &&
                   !(touch.position.x < Screen.width * 0.20 && touch.position.y < Screen.height * 0.20))
               {
                  localRotation.x += touch.deltaPosition.x;
                  localRotation.y -= touch.deltaPosition.y;
                  PlayerSettings.CubeRotation = true;
               }
            }

            if (!PlayerSettings.SettingsOn && !PlayerSettings.IsGameEnd)
            {
               // Handle camera zoom.
               var mouseWheel = Input.mouseScrollDelta.y;
               if (Input.touchCount == 2)
               {
                  var touchOne = Input.GetTouch(1);
                  var touchZeroPrevPos = touch.position - touch.deltaPosition;
                  var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                  var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                  var touchDeltaMag = (touch.position - touchOne.position).magnitude;
                  var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                  if (deltaMagnitudeDiff is > 5f or < -5f)
                  {
                     UpdateZoom(deltaMagnitudeDiff);
                  }
               }
               else if (Mathf.Abs(mouseWheel) > 0)
                  UpdateZoom(mouseWheel);
            }

            // Actual Camera Rig Transformation
            if (PlayerSettings.CubeRotation)
               cameraPivot.rotation =
                  Quaternion.Slerp(cameraPivot.rotation, targetLocation, Time.deltaTime * cameraRotationRadius);
            PlayerSettings.CubeRotation = false;
         }
      }
      
      private bool CheckRotationMatch(Vector3 normal, Vector3 targetMatch, Vector3 move, char axis)
      {
         var sum = normal + targetMatch;
         sum = axis switch
         {
            'X' => new Vector3(Mathf.Abs(move.x), Mathf.Abs(sum.y), Mathf.Abs(sum.z)),
            'Y' => new Vector3(Mathf.Abs(sum.x), Mathf.Abs(move.y), Mathf.Abs(sum.z)),
            'Z' => new Vector3(Mathf.Abs(sum.x), Mathf.Abs(sum.y), Mathf.Abs(move.z)),
            _ => sum
         };
         return sum == new Vector3(1, 1, 1);
      }


      // Handle cube calculations for movement and rotation along axes.
      private void CalculateCubeInteractions(Vector3 move)
      {
         if (firstHitNormal != secondHitNormal) return;
         var moveInfo = new MoveInfo();
         if (CheckRotationMatch(firstHitNormal, new Vector3(0, 0, 1), move, 'Y'))
         {
            moveInfo.Angle = firstHitNormal.x * move.y * rotateAngle;
            moveInfo.Index = Mathf.RoundToInt(firstHit.transform.position.z + offset);
            moveInfo.Axis = EAxis.Z;
         }
         else if (CheckRotationMatch(firstHitNormal, new Vector3(0, 1, 0), move, 'Z'))
         {
            moveInfo.Angle = firstHitNormal.x * move.z * -rotateAngle;
            moveInfo.Index = Mathf.RoundToInt(firstHit.transform.position.y + offset);
            moveInfo.Axis = EAxis.Y;
         }
         else if (CheckRotationMatch(firstHitNormal, new Vector3(0, 0, 1), move, 'X'))
         {
            moveInfo.Angle = firstHitNormal.y * move.x * -rotateAngle;
            moveInfo.Index = Mathf.RoundToInt(firstHit.transform.position.z + offset);
            moveInfo.Axis = EAxis.Z;
         }
         else if (CheckRotationMatch(firstHitNormal, new Vector3(1, 0, 0), move, 'Z'))
         {
            moveInfo.Angle = firstHitNormal.y * move.z * rotateAngle;
            moveInfo.Index = Mathf.RoundToInt(firstHit.transform.position.x + offset);
            moveInfo.Axis = EAxis.X;
         }
         else if (CheckRotationMatch(firstHitNormal, new Vector3(0, 1, 0), move, 'X'))
         {
            moveInfo.Angle = firstHitNormal.z * move.x * rotateAngle;
            moveInfo.Index = Mathf.RoundToInt(firstHit.transform.position.y + offset);
            moveInfo.Axis = EAxis.Y;
         }
         else if (CheckRotationMatch(firstHitNormal, new Vector3(1, 0, 0), move, 'Y'))
         {
            moveInfo.Angle = firstHitNormal.z * move.y * -rotateAngle;
            moveInfo.Index = Mathf.RoundToInt(firstHit.transform.position.x + offset);
            moveInfo.Axis = EAxis.X;
         }
         playerMoves.Push(moveInfo);
         RotateCube(moveInfo);
      }

      // Rotate the cube along the detected axis.
      private void RotateCube(MoveInfo move)
      {
         switch (move.Axis)
         {
            case EAxis.None:
               break;
            case EAxis.X:
               StartCoroutine(magicCube.RotateAlongX(move.Angle, move.Index));
               break;
            case EAxis.Y:
               StartCoroutine(magicCube.RotateAlongY(move.Angle, move.Index));
               break;
            case EAxis.Z:
               StartCoroutine(magicCube.RotateAlongZ(move.Angle, move.Index));
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
         
         GameManager.Instance.PlayShuffleAudio();
      }
   }
}
