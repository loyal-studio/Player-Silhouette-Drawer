using UnityEngine;
using UnityEditor;

namespace Daisen.Editor
{
    public static class SilhouetteDrawerUtility
    {
        private static readonly Vector3[] s_EllipsePts = new Vector3[25];
        private static readonly Vector3[] s_CirclePts = new Vector3[29];
        private static readonly (Vector3 centre, float rx, float rz)[] s_RingsCache = new (Vector3, float, float)[10];

        private static readonly Vector3[] s_Line2 = new Vector3[2];
        private static readonly Vector3[] s_Line3 = new Vector3[3];
        private static readonly Vector3[] s_Poly4 = new Vector3[4];

        private static readonly (float ny, float rx, float rz)[] k_BodyRingDef =
        {
            (0.00f, 0.040f, 0.040f),
            (0.08f, 0.055f, 0.055f),
            (0.26f, 0.075f, 0.065f),
            (0.38f, 0.090f, 0.080f),
            (0.50f, 1.000f, 0.650f),
            (0.62f, 1.000f, 0.700f),
            (0.70f, 1.000f, 0.720f),
            (0.80f, 1.000f, 0.600f),
            (0.88f, 0.065f, 0.058f),
            (0.90f, 0.055f, 0.050f),
        };

        private static class Styles
        {
            public static GUIStyle rulerLabel;

            public static void Init()
            {
                if (rulerLabel != null) return;
                rulerLabel = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 9
                };
            }
        }

        public static void DrawSilhouette3D(
            Vector3 feet, Quaternion rot, 
            float height, Color wireCol,
            float shoulderW, float hipW, float waistW, float headR,
            int edges, float thick,
            bool fillEnabled, Color fillCol)
        {
            edges = Mathf.Clamp(edges, 4, 64);
            int ringCount = k_BodyRingDef.Length;

            for (int i = 0; i < ringCount; i++)
            {
                var def = k_BodyRingDef[i];
                float worldRx = def.rx;
                float worldRz = def.rz;

                if (i == 4) 
                { 
                    worldRx = hipW * 0.5f * def.rx; 
                    worldRz = hipW * 0.5f * def.rz; 
                }
                else if (i == 5) 
                { 
                    worldRx = waistW * 0.5f * def.rx; 
                    worldRz = waistW * 0.5f * def.rz; 
                }
                else if (i == 6) 
                { 
                    worldRx = shoulderW * 0.44f * def.rx; 
                    worldRz = shoulderW * 0.44f * def.rz; 
                }
                else if (i == 7) 
                { 
                    worldRx = shoulderW * 0.5f * def.rx; 
                    worldRz = shoulderW * 0.5f * def.rz; 
                }

                s_RingsCache[i] = (feet + rot * new Vector3(0f, def.ny * height, 0f), worldRx, worldRz);
            }

            float headBottomY = k_BodyRingDef[^1].ny * height;
            Vector3 headCenter = feet + rot * new Vector3(0f, headBottomY + headR * 1.1f, 0f);

            if (fillEnabled)
            {
                DrawBodyFill(s_RingsCache, edges, fillCol, rot);
                DrawSphereFilledApprox(headCenter, headR, edges, fillCol, rot);
            }

            Handles.color = wireCol;

            for (int i = 0; i < ringCount; i++)
            {
                var ring = s_RingsCache[i];
                DrawEllipseXZ(ring.centre, ring.rx, ring.rz, thick, rot);
            }

            for (int e = 0; e < edges; e++)
            {
                float angle = Mathf.PI * 2f * e / edges;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                for (int r = 0; r < ringCount - 1; r++)
                {
                    var rA = s_RingsCache[r];
                    var rB = s_RingsCache[r + 1];

                    s_Line2[0] = rA.centre + rot * new Vector3(cos * rA.rx, 0f, sin * rA.rz);
                    s_Line2[1] = rB.centre + rot * new Vector3(cos * rB.rx, 0f, sin * rB.rz);
                    Handles.DrawAAPolyLine(thick, s_Line2);
                }
            }

            Draw3DSphere(headCenter, headR, wireCol, thick, rot);
            DrawHeightRuler(feet, height, wireCol, rot);
        }

        private static void DrawBodyFill((Vector3 centre, float rx, float rz)[] rings, int edges, Color col, Quaternion rot)
        {
            Handles.color = col;
            int ringCount = rings.Length;

            for (int e = 0; e < edges; e++)
            {
                float a0 = Mathf.PI * 2f * e / edges;
                float a1 = Mathf.PI * 2f * (e + 1) / edges;
                
                float cosA0 = Mathf.Cos(a0);
                float sinA0 = Mathf.Sin(a0);
                float cosA1 = Mathf.Cos(a1);
                float sinA1 = Mathf.Sin(a1);

                for (int r = 0; r < ringCount - 1; r++)
                {
                    var rA = rings[r];
                    var rB = rings[r + 1];

                    s_Poly4[0] = rA.centre + rot * new Vector3(cosA0 * rA.rx, 0f, sinA0 * rA.rz);
                    s_Poly4[1] = rA.centre + rot * new Vector3(cosA1 * rA.rx, 0f, sinA1 * rA.rz);
                    s_Poly4[2] = rB.centre + rot * new Vector3(cosA1 * rB.rx, 0f, sinA1 * rB.rz);
                    s_Poly4[3] = rB.centre + rot * new Vector3(cosA0 * rB.rx, 0f, sinA0 * rB.rz);

                    Handles.DrawAAConvexPolygon(s_Poly4);
                }
            }
        }

        private static void DrawSphereFilledApprox(Vector3 centre, float radius, int edges, Color col, Quaternion rot)
        {
            Handles.color = col;
            const int latSegs = 10;
            
            for (int e = 0; e < edges; e++)
            {
                float a0 = Mathf.PI * 2f * e / edges;
                float a1 = Mathf.PI * 2f * (e + 1) / edges;

                float cosA0 = Mathf.Cos(a0);
                float sinA0 = Mathf.Sin(a0);
                float cosA1 = Mathf.Cos(a1);
                float sinA1 = Mathf.Sin(a1);

                for (int lat = 0; lat <= latSegs; lat++)
                {
                    float phi0 = Mathf.PI * lat / latSegs - Mathf.PI * 0.5f;
                    float phi1 = Mathf.PI * (lat + 1) / latSegs - Mathf.PI * 0.5f;

                    float r0 = radius * Mathf.Cos(phi0);
                    float r1 = radius * Mathf.Cos(phi1);
                    float y0 = radius * Mathf.Sin(phi0);
                    float y1 = radius * Mathf.Sin(phi1);

                    Vector3 c0 = centre + rot * new Vector3(0f, y0, 0f);
                    Vector3 c1 = centre + rot * new Vector3(0f, y1, 0f);

                    s_Poly4[0] = c0 + rot * new Vector3(cosA0 * r0, 0f, sinA0 * r0);
                    s_Poly4[1] = c0 + rot * new Vector3(cosA1 * r0, 0f, sinA1 * r0);
                    s_Poly4[2] = c1 + rot * new Vector3(cosA1 * r1, 0f, sinA1 * r1);
                    s_Poly4[3] = c1 + rot * new Vector3(cosA0 * r1, 0f, sinA0 * r1);

                    Handles.DrawAAConvexPolygon(s_Poly4);
                }
            }
        }

        private static void DrawEllipseXZ(Vector3 centre, float rx, float rz, float thick, Quaternion rot)
        {
            int segs = s_EllipsePts.Length - 1;
            for (int i = 0; i <= segs; i++)
            {
                float a = Mathf.PI * 2f * i / segs;
                s_EllipsePts[i] = centre + rot * new Vector3(Mathf.Cos(a) * rx, 0f, Mathf.Sin(a) * rz);
            }
            Handles.DrawAAPolyLine(thick, s_EllipsePts);
        }

        private static void Draw3DSphere(Vector3 centre, float radius, Color col, float thick, Quaternion rot)
        {
            Handles.color = col;
            DrawGreatCircle(centre, radius, rot * Vector3.right, rot * Vector3.up, thick);
            DrawGreatCircle(centre, radius, rot * Vector3.right, rot * Vector3.forward, thick);
            DrawGreatCircle(centre, radius, rot * Vector3.up, rot * Vector3.forward, thick);
        }

        private static void DrawGreatCircle(Vector3 c, float r, Vector3 axis1, Vector3 axis2, float thick)
        {
            int segs = s_CirclePts.Length - 1;
            for (int i = 0; i <= segs; i++)
            {
                float a = Mathf.PI * 2f * i / segs;
                s_CirclePts[i] = c + (Mathf.Cos(a) * axis1 + Mathf.Sin(a) * axis2) * r;
            }
            Handles.DrawAAPolyLine(thick, s_CirclePts);
        }

        private static void DrawHeightRuler(Vector3 feet, float height, Color col, Quaternion rot)
        {
            Styles.Init();
            Color rulerCol = col;
            rulerCol.a *= 0.55f;
            Handles.color = rulerCol;

            float xOff = 0.32f;
            Vector3 top = feet + rot * new Vector3(xOff, height, 0f);
            Vector3 bot = feet + rot * new Vector3(xOff, 0f, 0f);

            s_Line2[0] = bot;
            s_Line2[1] = top;
            Handles.DrawAAPolyLine(1.5f, s_Line2);

            float ah = 0.035f;
            s_Line3[0] = top + rot * new Vector3(-ah, -ah, 0f);
            s_Line3[1] = top;
            s_Line3[2] = top + rot * new Vector3(ah, -ah, 0f);
            Handles.DrawAAPolyLine(1.5f, s_Line3);

            s_Line3[0] = bot + rot * new Vector3(-ah, ah, 0f);
            s_Line3[1] = bot;
            s_Line3[2] = bot + rot * new Vector3(ah, ah, 0f);
            Handles.DrawAAPolyLine(1.5f, s_Line3);

            Styles.rulerLabel.normal.textColor = col;
            Handles.Label(feet + rot * new Vector3(xOff + 0.06f, height * 0.5f, 0f), $"{height:F2} m", Styles.rulerLabel);
        }
    }
}
