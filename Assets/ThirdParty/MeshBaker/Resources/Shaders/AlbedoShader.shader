Shader "MeshBaker/AlbedoShader" {
		Properties{
			_MainTex("Base (RGB)", 2D) = "white" {}
		}
			Category{
			Lighting Off
			ZWrite On
			Cull Back
			SubShader{
			Pass{
			SetTexture[_MainTex]{

		}
		}
		}
	}
}