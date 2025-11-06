using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.Serialization;
using System.Linq;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Displays a Texture2D for the UI System.
    /// </summary>
    /// <remarks>
    /// If you don't have or don't wish to create an atlas, you can simply use this script to draw a texture.
    /// Keep in mind though that this will create an extra draw call with each RawImage present, so it's
    /// best to use it only for backgrounds or temporary visible graphics.
    /// </remarks>

    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/Torus", 12)]
    public class UITorus : MaskableGraphic
    {
        [FormerlySerializedAs("m_Tex")]
        [SerializeField] Texture m_Texture;
        [SerializeField] Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        /// <summary>
        /// Returns the texture used to draw this Graphic.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (m_Texture == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return m_Texture;
            }
        }

        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect uvRect
        {
            get
            {
                return m_UVRect;
            }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Adjust the scale of the Graphic to make it pixel-perfect.
        /// </summary>
        /// <remarks>
        /// This means setting the RawImage's RectTransform.sizeDelta  to be equal to the Texture dimensions.
        /// </remarks>
        public override void SetNativeSize()
        {
            Texture tex = mainTexture;
            if (tex != null)
            {
                int w = Mathf.RoundToInt(tex.width * uvRect.width);
                int h = Mathf.RoundToInt(tex.height * uvRect.height);
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
            }
        }

        [SerializeField] public float innerRadius = 0.25f;
        [SerializeField] public float outerRadius = 0.5f;
        public float InnerRadius { get => innerRadius * Mathf.Min(rectTransform.rect.width, rectTransform.rect.height); }
        public float OuterRadius { get => outerRadius * Mathf.Min(rectTransform.rect.width, rectTransform.rect.height); }
        [SerializeField] private int segments = 64;
        [SerializeField] private int tubeSegments = 16;
        [Range(0, 1)] [SerializeField] private float fillAmount = 1f; // Radial fill amount
        public float FillAmount { get => fillAmount; set => fillAmount = value; }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            float torusRadius = (OuterRadius - InnerRadius) / 2f;
            float centerRadius = torusRadius + InnerRadius;

            // Calculate the angle for the fill amount
            float fillAngle = fillAmount * 2f * Mathf.PI;

            for (int i = 0; i <= segments; i++)
            {
                float angle = 2f * Mathf.PI * i / segments;

                // Check if this segment is within the fill amount
                if (angle <= fillAngle)
                {
                    Vector3 center = new Vector3(Mathf.Cos(angle) * centerRadius, Mathf.Sin(angle) * centerRadius, 0f);

                    for (int j = 0; j <= tubeSegments; j++)
                    {
                        float tubeAngle = 2f * Mathf.PI * j / tubeSegments;
                        Vector3 offset = new Vector3(Mathf.Cos(tubeAngle) * torusRadius, 0f, Mathf.Sin(tubeAngle) * torusRadius);
                        Vector3 position = center + Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg) * offset;

                        vh.AddVert(position, color, Vector2.zero);
                    }
                }
            }

            // Generate torus triangles
            for (int i = 0; i < segments; i++)
            {
                // Check if both segments are within the fill amount
                if (2f * Mathf.PI * i / segments <= fillAngle && 2f * Mathf.PI * (i + 1) / segments <= fillAngle)
                {
                    for (int j = 0; j < tubeSegments; j++)
                    {
                        int v1 = i * (tubeSegments + 1) + j;
                        int v2 = (i + 1) * (tubeSegments + 1) + j;
                        int v3 = i * (tubeSegments + 1) + j + 1;
                        int v4 = (i + 1) * (tubeSegments + 1) + j + 1;

                        vh.AddTriangle(v1, v2, v3);
                        vh.AddTriangle(v2, v4, v3);
                    }
                }
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetMaterialDirty();
            SetVerticesDirty();
            SetRaycastDirty();
        }
    }
}