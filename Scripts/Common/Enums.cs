
namespace DOUKH.Common.Enums
{
    //public enum MaterialRenderMode { Opaque, Cutout, Fade, Transparent };

    public enum UpdateMode { Update, FixedUpdate, LateUpdate }

    public enum DeltaTimeType { Normal, Fixed, Unscaled, Smooth }

    public enum SmoothingMode { None, Lerp, Damp }

    public enum ColliderDirection { Horizontal = 0, Vertical = 1, Forward = 2 }

    public enum ColliderType { Box, Sphere, Capsule }

    public enum Size { Tiny, Small, Medium, Large, Giant }

    public enum GameState { Loading, Playing, Paused, MainMenu }
}