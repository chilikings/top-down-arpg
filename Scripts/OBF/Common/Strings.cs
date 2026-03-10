
namespace DOUKH.OBF.Strings
{
    public static class KeywordsOBF
    {
        public const string Highlights = "_SPECULARHIGHLIGHTS_OFF", Reflections = "_GLOSSYREFLECTIONS_OFF", 
            Transparent = "_ALPHAPREMULTIPLY_ON", Cutout = "_ALPHATEST_ON", Fade = "_ALPHABLEND_ON";
    }

    public static class PropertiesOBF
    {
        public const string Highlights = "_SpecularHighlights", Reflections = "_GlossyReflections", DestinationBlend = "_DstBlend", 
            SourceBlend = "_SrcBlend", Depth = "_ZWrite", Color = "_Color", Mode = "_Mode";
    }

    public static class NamesOBF
    {
        public const string RenderTag = "RenderType", Transparent = "Transparent", PlayerTag = "Player", RaysParent = "Rays", RayTargetsParent = "Ray Targets", DefaultShader = "Standard";
    }
}