using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutoutMaskUI : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material material = base.materialForRendering;

            // Remplacer le _StencilComp pour appliquer la comparaison sur la valeur correcte
            material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
            material.SetInt("_Stencil", 1); // Assurez-vous que le stencil est à la valeur correcte
            material.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Replace);

            return material;
        }
    }
}