using System.Linq;
using UnityEngine;

public static class ManipulateHologramShader
{
    /**
     * Randomize Properties of the hologram material.
     */
    public static void RandomizeTilingAndSpeed(this Material material)
    {
        // Change Material Hologram Shader so it changes _ScanSpeed and _ScanTiling randomly
        material.SetFloat("_ScanSpeed", Random.Range(0.1f, .3f));
        material.SetFloat("_ScanTiling", Random.Range(5f, 7f));
        material.SetFloat("_GlitchSpeed", Random.Range(10f, 30f));

    }

}
