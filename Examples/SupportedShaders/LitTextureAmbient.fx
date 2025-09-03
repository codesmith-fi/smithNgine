// === TRANSFORMATION MATRICES ===
float4x4 World;
float4x4 View;
float4x4 Projection;

// === Lighting Parameters ===
float4 AmbientColor;          // e.g., (0.2, 0.2, 0.2, 1.0)
float AmbientIntensity;       // e.g., 0.5
Texture2D Texture;

// === TEXTURE SAMPLING ===
SamplerState TextureSampler {
    Filter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexInput {
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;     // Not used yet, but included for future lighting
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

// === PIXEL INPUT ===
struct PixelInput {
    float4 Position : SV_POSITION;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

// === VERTEX SHADER ===
PixelInput VS(VertexInput input) {
    PixelInput output;

    float4 worldPos = mul(input.Position, World);
    float4 viewPos = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);

    output.Color = input.Color;
    output.TexCoord = input.TexCoord;

    return output;
}

// === PIXEL SHADER ===
float4 PS(PixelInput input) : SV_Target {
    float4 texColor = Texture.Sample(TextureSampler, input.TexCoord);
    float4 ambient = texColor * AmbientColor * AmbientIntensity;
    return ambient * input.Color;
}

// === TECHNIQUE ===
technique AmbientTextureColor {
    pass P0 {
        VertexShader = compile vs_4_0_level_9_1 VS();
        PixelShader  = compile ps_4_0_level_9_1 PS();
    }
}


/*

// === VERTEX INPUT STRUCTURE ===
// === Vertex Input/Output ===
struct VertexInput
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct PixelInput
{
    float4 Position : SV_POSITION;
    float3 Normal   : TEXCOORD1;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

// === Vertex Shader ===
PixelInput VS(VertexInput input)
{
    PixelInput output;

    float4 worldPos = mul(input.Position, World);
    output.Position = mul(worldPos, View * Projection);

    // Transform normal to world space
    output.Normal = normalize(mul(float4(input.Normal, 0.0), World).xyz);

    output.TexCoord = input.TexCoord;
    output.Color = input.Color;

    return output;
}

// === Pixel Shader ===
float4 PS(PixelInput input) : SV_Target
{
    // Sample texture
    float4 texColor = Texture.Sample(TextureSampler, input.TexCoord);

    // Ambient lighting
    float4 ambient = texColor * AmbientColor * AmbientIntensity;

    // Diffuse lighting
    float NdotL = saturate(dot(input.Normal, -normalize(LightDirection)));
    float4 diffuse = texColor * DiffuseColor * DiffuseIntensity * NdotL;

    // Combine lighting and modulate with vertex color
    float4 finalColor = (ambient + diffuse) * input.Color;

    return finalColor;
}

// === Technique ===
technique LitTextureAmbientDiffuse
{
    pass P0
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader  = compile ps_4_0 PS();
    }
}
*/