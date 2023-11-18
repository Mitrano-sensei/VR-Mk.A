using System.Linq;
using UnityEngine;

public static class ManipulateHologramShader
{
    public static void RandomizeTilingAndSpeed(this Material material)
    {
        // Change Material Hologram Shader so it changes _ScanSpeed and _ScanTiling randomly
        material.SetFloat("_ScanSpeed", Random.Range(0.1f, 1f));
        material.SetFloat("_ScanTiling", Random.Range(0.1f, 0.5f));

        material.shaderKeywords.ToList().ForEach(x => Debug.Log("Keyword : " + x));
    }

}
