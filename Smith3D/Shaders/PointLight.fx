float4x4 World;
float4x4 View;
float4x4 Projection;

float3 lightPosition;
float3 lightColor;
float lightIntensity;
float constantAttenuation;
float linearAttenuation;
float quadraticAttenuation;
float3 ambientColor;
float ambientIntensity;

Texture2D Texture;
SamplerState TextureSampler
{
    Filter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position     : SV_POSITION;
    float3 WorldNormal  : TEXCOORD0;
    float4 Color        : COLOR0;
    float2 TexCoord     : TEXCOORD1;
    float3 WorldPos     : TEXCOORD2;
};

VSOutput VS(VSInput input)
{
    VSOutput output;

    float4 worldPos = mul(input.Position, World);
    float4 viewPos = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);

    output.WorldPos = worldPos.xyz;
    output.WorldNormal = normalize(mul(float4(input.Normal, 0.0f), World).xyz);
    output.TexCoord = input.TexCoord;
    output.Color = input.Color;

    return output;
}

float4 PS(VSOutput input) : SV_Target
{
    float4 texColor = Texture.Sample(TextureSampler, input.TexCoord);

    float3 toLight = lightPosition - input.WorldPos;
    float distanceToLight = length(toLight);
    float3 lightDir = distanceToLight > 0.0001f ? toLight / distanceToLight : float3(0.0f, 0.0f, 0.0f);

    float ndotl = max(dot(normalize(input.WorldNormal), lightDir), 0.0f);
    float attenuationDenominator = constantAttenuation
        + (linearAttenuation * distanceToLight)
        + (quadraticAttenuation * distanceToLight * distanceToLight);
    float attenuation = attenuationDenominator > 0.0001f ? (1.0f / attenuationDenominator) : 1.0f;

    float3 ambient = ambientColor * ambientIntensity;
    float3 diffuse = lightColor * lightIntensity * ndotl * attenuation;
    float3 litColor = ambient + diffuse;

    float3 finalRgb = texColor.rgb * input.Color.rgb * litColor;
    float finalAlpha = texColor.a * input.Color.a;

    return float4(finalRgb, finalAlpha);
}

technique TexturedPointLight
{
    pass P0
    {
        VertexShader = compile vs_4_0_level_9_1 VS();
        PixelShader = compile ps_4_0_level_9_1 PS();
    }
}
