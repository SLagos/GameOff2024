Shader "Custom/ShaderMask"
{
    Properties{}

	SubShader{

		Tags {
			"RenderType" = "Opaque"
		}

		Pass {
			ZWrite Off
		}
	}
}
