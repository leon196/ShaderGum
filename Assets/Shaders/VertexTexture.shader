
// Leon 2017

Shader "VertexTexture/Mesh"  {
	Properties {
		_MainTex ("Mesh Texture", 2D) = "white" {}
		_VertexTexture ("Vertex Texture", 2D) = "white" {}
	}
	SubShader  {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			Name "Custom/VertexTexture"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			// mesh texture
			sampler2D _MainTex;

			// frame buffer
			sampler2D _VertexTexture;

			// texture coordinate
			float4 _MainTex_ST;

			// mesh position
			float3 _TransformPosition;

			// mesh matrix
			float4x4 _RendererMatrix;

			struct attributes {
				float2 texcoord : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			attributes vert (appdata_full v) {

				attributes o;

				// get position from generated texture
				v.vertex.xyz = tex2Dlod(_VertexTexture, v.texcoord1).rgb;

				// go world space
				v.vertex.xyz = _TransformPosition + mul(_RendererMatrix, v.vertex.xyz);
				
				// apply camera projection
				o.vertex = mul(UNITY_MATRIX_VP, v.vertex);

				// send texture coordinate
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}
			
			fixed4 frag (attributes i) : SV_Target {

				// unlit texture
				return tex2D(_MainTex, i.texcoord);
			}
			
			ENDCG
		}
	}
}
