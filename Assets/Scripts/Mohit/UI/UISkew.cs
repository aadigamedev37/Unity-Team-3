using UnityEngine;
using UnityEngine.UI;

namespace Mohit.MetaSystems
{
    // Leans any UI Image to the right (like the slanted buttons in the concept).
    // Put it BELOW an Outline component in the Inspector so the outline leans too.
    [AddComponentMenu("UI/Effects/Skew")]
    public class UISkew : BaseMeshEffect
    {
        [SerializeField] private float skew = 0.3f;   // 0.3 is about 17 degrees

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0) return;

            float centerY = ((RectTransform)transform).rect.center.y;
            UIVertex v = default;
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                Vector3 p = v.position;
                p.x += (p.y - centerY) * skew;
                v.position = p;
                vh.SetUIVertex(v, i);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (graphic != null) graphic.SetVerticesDirty();
        }
#endif
    }
}