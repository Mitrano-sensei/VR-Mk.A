using System.Linq;
using UnityEngine;

public static class ManipulateHologramShader
{
    public static void RandomizeTilingAndSpeed(this Material material)
    {
        // Change Material Hologram Shader so it changes _ScanSpeed and _ScanTiling randomly
        material.SetFloat("_ScanSpeed", Random.Range(0.1f, .3f));
        material.SetFloat("_ScanTiling", 6f);
        material.SetFloat("_GlitchSpeed", Random.Range(10f, 30f));

    }

}
