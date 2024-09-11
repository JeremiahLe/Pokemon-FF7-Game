// Fantasy Adventure Environment
// staggart.xyz

float4 _WindDirection;
float _TrunkWindSpeed;
float _TrunkWindSwinging;
float _TrunkWindWeight;
float _WindSpeed;
float _WindAmplitude;
float _WindStrength;

TEXTURE2D(_WindVectors); SAMPLER(sampler_WindVectors);

float WindSpeed() {
	return _WindSpeed * _TimeParameters.x * 0.25; //10x faster than legacy _Time.x
}

float3 WindDirection() {
	return _WindDirection.xyz + 0.001;
}
void GetGlobalParams_float(out float3 windDir, out float trunkSpeed, out float trunkSwinging, out float trunkWeight, out float windSpeed)
{
	windDir = WindDirection().xyz;
    trunkSpeed = _TrunkWindSpeed ;
    trunkSwinging = _TrunkWindSwinging;
    trunkWeight = _TrunkWindWeight;
    windSpeed = WindSpeed();
};

void GetLocalParams_float(in float3 wPos, in float windFreqMult, out float3 windDir, out float trunkSpeed, out float trunkSwinging, out float trunkWeight, out float windSpeed, out float windFreq, out float windStrength)
{
    windDir = WindDirection().xyz;
    trunkSpeed = _TrunkWindSpeed;
    trunkSwinging = _TrunkWindSwinging;
    trunkWeight = _TrunkWindWeight;
    windSpeed = WindSpeed();
    windFreq = length(wPos.xz * 0.01) * (_WindAmplitude * windFreqMult);
    windStrength = _WindStrength;
};

float3 GetPivotPos() {
	return float3(UNITY_MATRIX_M[0][3], UNITY_MATRIX_M[1][3] + 0.25, UNITY_MATRIX_M[2][3]);
}

float ObjectPosRand01() {
	return frac(UNITY_MATRIX_M[0][3] + UNITY_MATRIX_M[1][3] + UNITY_MATRIX_M[2][3]);
}

void ApplyFoliageWind_float(in float3 wPos, in float maxStrength, in float mask, in float leafFlutter, in float globalMotion, in float swinging, in float freqMult, in float3 positionOS, out float3 offset)
{
	float speed = WindSpeed();

	float2 windUV = (wPos.xz * 0.01) * _WindAmplitude * freqMult;
	windUV += (WindDirection().xz * (speed));

	float3 windVec = UnpackNormal(SAMPLE_TEXTURE2D_LOD(_WindVectors, sampler_WindVectors, windUV, 0)).rgb;
	windVec = TransformWorldToObjectDir(windVec);
	
	float sine = sin(ObjectPosRand01() + length(WindDirection().xz) * speed * 25);
	sine = lerp(sine * 0.5 + 0.5, sine, swinging);

	windVec = maxStrength * mask * ((sine * globalMotion * 0.5) + (windVec * leafFlutter));

	offset = (float3(windVec.x, 0, windVec.y)) + positionOS;
};

float4 _ObstaclePosition;
float _BendingStrength;
float _BendingRadius;

void GetBendingOffset_float(in float3 positionOS, in float3 wPos, in float mask, in float influence, out float3 offsetPosition)
{
	float3 dir = normalize(_ObstaclePosition.xyz - wPos);
	dir = TransformWorldToObjectDir(dir);

	float falloff = 1-saturate(distance(_ObstaclePosition.xyz, wPos) / _BendingRadius);

	float3 offset = 0;
	offset.xz = dir.xz * (_BendingStrength * 0.1);
	offset *= -(falloff * influence) * mask;
	
	offsetPosition = positionOS + offset;
}

void SampleWind_float(in float2 wPos, out float3 vec)
{
    float2 v = SAMPLE_TEXTURE2D_LOD(_WindVectors, sampler_WindVectors, wPos, 0).rg ;

    vec = float3(v.x, 0, v.y);
};

void ApplyLODCrossfade_float(in float4 screenPos, in float alpha, out float dithered)
{
	#ifndef UNIVERSAL_SHADOW_CASTER_PASS_INCLUDED
	#if LOD_FADE_CROSSFADE
	float hash = GenerateHashedRandomFloat(screenPos.xy * _ScreenParams.xy);
	float sign = CopySign(hash, unity_LODFade.x);
	
	clip(unity_LODFade.x - sign);
	#endif
	#endif
	dithered = alpha;
}

void GetSunColor_float(out float3 color) 
{
#ifdef UNIVERSAL_LIGHTING_INCLUDED
	Light mainLight = GetMainLight();
	color = mainLight.color;
#else
	color = 0;
#endif
}

void MainLight_half(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
#ifdef UNIVERSAL_LIGHTING_INCLUDED
	#if SHADERGRAPH_PREVIEW
		Direction = half3(0.5, 0.5, 0);
		Color = 1;
		DistanceAtten = 1;
		ShadowAtten = 1;
	#else
	#if SHADOWS_SCREEN
		half4 clipPos = TransformWorldToHClip(WorldPos);
		half4 shadowCoord = ComputeScreenPos(clipPos);
	#else
		half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
#endif
	Light mainLight = GetMainLight(shadowCoord);
	Direction = mainLight.direction;
	Color = mainLight.color;
	DistanceAtten = mainLight.distanceAttenuation;
	ShadowAtten = mainLight.shadowAttenuation;
#else
	Direction = half3(0.5, 0.5, 0);
	Color = 1;
	DistanceAtten = 1;
	ShadowAtten = 1;
#endif
}