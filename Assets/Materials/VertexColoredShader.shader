Shader "Custom/VertexColoredShader"
 {

	/*Properties{
		//_Shininess("Shininess", Range(0.03, 1)) = 0.078125
	}
		SubShader{
		Lighting Off
			Tags{ 
			  "RenderType" = "Opaque"
			 "ForceNoShadowCasting" = "True"
			}
			CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert
			//half _Shininess;
			struct Input {
				half2 uv_MainTex;
				fixed3 vertColors;
			};

			void vert(inout appdata_full v, out Input o) {
				o.vertColors = v.color.rgb;
				o.uv_MainTex = v.texcoord;
			}

			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = IN.vertColors.rgb;
				//o.Specular = 1;
				//o.Gloss = _Shininess;
			}
			ENDCG
		}*/
		//Fallback "Specular"
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
		_AmbientOcclusionIntensity("Ambient Occlusion Intensity", Range(0,1)) = 0.5
    }
    SubShader{
        Tags{ "RenderType" = "Opaque"}
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float4 color : Color;
        };

        half   _Glossiness;
        half   _Metallic;
        fixed4 _Color;
		half   _Alpha;
		half   _AmbientOcclusionIntensity;

        void surf(Input IN, inout SurfaceOutputStandard o) 
        {                        
            fixed4 c = IN.color;

			float ambientOcclusionStrength = IN.uv_MainTex.y;

            o.Albedo     = c * lerp(1.0, (1.0f - _AmbientOcclusionIntensity), ambientOcclusionStrength);
            o.Metallic   = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
        }
        FallBack "Diffuse"
}