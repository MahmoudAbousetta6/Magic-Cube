namespace Scripts
{
   /// <summary>
   /// Hold player data across scenes.
   /// </summary>
   public static class PlayerSettings {
      
      public static int CubeSize { get; set; }

      public static bool SettingsOn { get; set; }

      public static bool IsGameEnd { get; set; }

      public static bool TimerOn { get; set; }

      public static bool CameraDisable { get; set; }

      public static bool FaceRotation { get; set; }

      public static bool CubeRotation { get; set; }
      
      public static bool IsShuffling { get; set; }
      
      public static bool IsLoaded { get; set; }
      
      public static MagicCubeInfo Data { get; set; }
      

      public static void ResetData()
      {
         CubeSize = default;
         SettingsOn = default;
         IsGameEnd = default;
         TimerOn = default;
         CameraDisable = default;
         FaceRotation = default;
         CubeRotation = default;
         IsShuffling = default;
      }
   }
}
