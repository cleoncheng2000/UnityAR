Shader "Custom/BarrelDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LensDistortionTightness ("Distortion Tightness", Range(0.1, 5.0)) = 1.5
        _LensDistortionStrength ("Distortion Strength", Range(0.0, 1.0)) = 0.2
        _OutOfBoundColour ("Out of Bounds Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _LensDistortionTightness;
            float _LensDistortionStrength;
            float4 _OutOfBoundColour;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Convert UV range from (0,1) to (-1,1)
                float2 uv_centered = i.uv * 2.0 - 1.0;

                // Compute distortion magnitude (1 at corners, 0 at center)
                float distortionMagnitude = abs(uv_centered.x * uv_centered.y);

                // Apply smooth distortion function
                float smoothDistortionMagnitude = pow(distortionMagnitude, _LensDistortionTightness);

                // Distort UV coordinates
                float2 uvDistorted = i.uv + uv_centered * smoothDistortionMagnitude * _LensDistortionStrength;

                // Handle out-of-bounds UV
                if (uvDistorted.x < 0 || uvDistorted.x > 1 || uvDistorted.y < 0 || uvDistorted.y > 1)
                {
                    return _OutOfBoundColour;
                }
                else
                {
                    return tex2D(_MainTex, uvDistorted);
                }
            }
            ENDCG
        }
    }
}
