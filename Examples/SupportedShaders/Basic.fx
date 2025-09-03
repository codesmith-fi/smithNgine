// === TRANSFORMATION MATRICES ===
float4x4 World;
float4x4 View;
float4x4 Projection;

// === VERTEX INPUT STRUCTURE ===
struct VSInput {
    float4 Position : POSITION;
    float4 Color    : COLOR0;
};

// === VERTEX OUTPUT STRUCTURE ===
struct VSOutput {
    float4 Position : SV_POSITION;
    float4 Color    : COLOR0;
};

// === VERTEX SHADER ===
VSOutput VSMain(VSInput input) {
    VSOutput output;
    float4 worldPos = mul(input.Position, World);
    float4 viewPos = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);
    output.Color = input.Color;
    return output;
}

float4 PSMainColored(VSOutput input) : SV_TARGET {
    return input.Color; // Modulate texture with vertex color
}

technique Colored
{
    pass P0
    {
        VertexShader = compile vs_4_0_level_9_1 VSMain();
        PixelShader = compile ps_4_0_level_9_1 PSMainColored();
    }
}
