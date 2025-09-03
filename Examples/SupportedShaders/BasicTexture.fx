// === TRANSFORMATION MATRICES ===
float4x4 World;
float4x4 View;
float4x4 Projection;
Texture2D Texture;

// === TEXTURE SAMPLING ===
SamplerState TextureSampler {
    Filter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

// === VERTEX INPUT STRUCTURE ===
struct VSInput {
    float4 Position : POSITION;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

// === VERTEX OUTPUT STRUCTURE ===
struct VSOutput {
    float4 Position : SV_POSITION;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

// === VERTEX SHADER ===
VSOutput VSMain(VSInput input) {
    VSOutput output;
    float4 worldPos = mul(input.Position, World);
    float4 viewPos = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);
    output.TexCoord = input.TexCoord;
    output.Color = input.Color;
    return output;
}

// === PIXEL SHADER for TEXTURED ===
float4 PSMainTextured(VSOutput input) : SV_TARGET {
    float4 texColor = Texture.Sample(TextureSampler, input.TexCoord);
    return texColor * input.Color; // Modulate texture with vertex color
}

technique Textured
{
    pass P0
    {
        VertexShader = compile vs_4_0_level_9_1 VSMain();
        PixelShader = compile ps_4_0_level_9_1 PSMainTextured();
    }
}
