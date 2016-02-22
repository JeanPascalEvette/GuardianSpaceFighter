//This Shader is designed to produce a star like effect by having the alpha set to max in the very middle of the disc and progressively lowering as you get closer to the edges. This means that while the center is very intense the sides get more and more blurry.
Shader "Custom/Stars" {
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Radius("Radius", Float) = 1.0
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		ZWrite Off
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha     // Alpha blending

			Pass{
			CGPROGRAM

#pragma vertex vert  
#pragma fragment frag 

			struct vertexInput {
				float4 vertex : POSITION;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 posInObjectCoords : TEXCOORD0;
				fixed4 color : COLOR;
			};
			fixed4 _Color;
			float _Radius;

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.posInObjectCoords = input.vertex;
				output.color = _Color;
				return output;
			}

			fixed4 frag(vertexOutput input) : COLOR
			{
				fixed4 outColor = input.color;
				float xPart = input.posInObjectCoords.x * input.posInObjectCoords.x;
				float yPart = input.posInObjectCoords.y * input.posInObjectCoords.y;
				float distance = sqrt(xPart + yPart);
				//Set The Alpha of the stars to decay as you go further from the center
				outColor.a = min(max((_Radius - distance)/_Radius,0),1);


			return outColor; 
			}

				ENDCG
			}
		}
	}