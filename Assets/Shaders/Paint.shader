Shader "Hidden/Paint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ResetBlend ("Reset Blend", Range(0, 0.1)) = 0.01
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float _Paint, _PaintSize, _ResetBlend;
			float3 _PaintPoint, _PaintDrag, _TransformPosition;
			float4x4 _RendererMatrix, _InverseMatrix;
			sampler2D _MainTex, _OriginTexture;

			fixed4 frag (v2f_img i) : SV_Target
			{
				float2 uv = i.uv;

				// current position
				fixed4 pos = tex2D(_MainTex, uv);

				// smooth shape reset
				fixed4 origin = tex2D(_OriginTexture, uv);
				pos = lerp(pos, origin, _ResetBlend);

				// go world space
				pos.rgb = mul(_RendererMatrix, pos) + _TransformPosition;

				// influence area
				float radius = distance(pos.rgb, _PaintPoint);
				float brush = 1. - smoothstep(_PaintSize * 0.2, _PaintSize, radius);

				// direction and force of painting
				pos.rgb += _Paint * _PaintDrag * brush;

				// back to local space
				pos.rgb = mul(_InverseMatrix, pos.rgb - _TransformPosition);

				return pos;
			}
			ENDCG
		}
	}
}
