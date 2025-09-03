// 1. Global variables
float4x4 World;
float4x4 View;
float4x4 Projection;

float3 lightPosition;
float3 lightColor;
float lightIntensity;
float constantAttenuation;
float linearAttenuation;
float quadraticAttenuation;
float gameTime; // in seconds

// SamplerState, DX11 style
Texture2D Texture;
SamplerState TextureSampler {
    Filter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

// Vertex input
struct VSInput
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;   
};

struct VSOutput
{
    float4 Position     : SV_POSITION; // Clip space position
    float3 Normal       : TEXCOORD0; // Transformed normal
    float4 Color        : COLOR0;
    float2 TexCoord     : TEXCOORD1; // Texture coordinates
    float3 FragPosition : TEXCOORD2; // World space position
};

VSOutput VSMain(VSInput input)
{
    VSOutput output;

    float4 worldPos = mul(input.Position, World);
    float4 viewPos  = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);

    output.FragPosition = worldPos.xyz;
    output.Normal       = mul(input.Normal, (float3x3)World);
    output.TexCoord     = input.TexCoord;
    output.Color        = input.Color; // ← NEW

    return output;
}

// 2. Shader functions
float4 PSMain(VSOutput input) : SV_Target
{   
    /*
    float3 lightDir = normalize(lightPosition - input.FragPosition);
    float distance  = length(lightPosition - input.FragPosition);

    float attenuation = 1.0 / (constantAttenuation +
                               linearAttenuation * distance +
                               quadraticAttenuation * distance * distance);

    float diffuseFactor = max(dot(input.Normal, lightDir), 0.0);
    float3 diffuse = diffuseFactor * lightColor * lightIntensity * attenuation;

    float4 texColor = Texture.Sample(TextureSampler, input.TexCoord);
    float3 finalColor = diffuse * texColor.rgb * input.Color.rgb;
    float finalAlpha  = texColor.a * input.Color.a;

    return float4(finalColor, finalAlpha);
    */
    float4 texColor = Texture.Sample(TextureSampler, input.TexCoord);
    return texColor * input.Color; // Modulate texture with vertex color
}

// 3. Technique block
technique TexturedPointLight
{
    pass P0
    {
        VertexShader = compile vs_4_0_level_9_1 VSMain();
        PixelShader = compile ps_4_0_level_9_1 PSMain();
    }
}
