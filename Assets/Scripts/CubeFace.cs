using UnityEngine;

namespace Scripts
{
   /// <summary>
   /// Handle cube face's color and holding data of each face.
   /// </summary>
   [RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
   public class CubeFace : MonoBehaviour
   {
      private MeshRenderer meshRenderer;

      public CubeFaceInfo Info { get; set; }

      private void Awake()
      {
         
         meshRenderer = GetComponent<MeshRenderer>();
      }

      public void SetMaterial(Material mat)
      {
         meshRenderer.material = mat;
      }
   }
}
