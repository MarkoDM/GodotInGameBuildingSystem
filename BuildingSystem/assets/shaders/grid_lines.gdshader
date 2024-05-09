shader_type spatial;
render_mode blend_mix, unshaded;

uniform float gridlineOnEvery = 1.0;
uniform float lineThickness = .01;
uniform vec2 originOffset = vec2(0.0, 0.0);
uniform vec4 color: source_color = vec4(0.0, 0.0, 0.0, .5);
uniform float glowStrength = 0.3;
uniform float glowSpread = 0.05;

varying vec2 pointWorld;

void vertex(){
	pointWorld = (MODEL_MATRIX * vec4(VERTEX, 1.0) ).xz - originOffset;
}

void fragment() {
	float thick = lineThickness / gridlineOnEvery;
	vec2 df = fract(pointWorld / gridlineOnEvery + thick / 2.0);
	float grid = step(thick, df.x) * step(thick, df.y);
	
	 // Calculate glow falloff
    float distanceToLine = min(min(df.x, 1.0 - df.x), min(df.y, 1.0 - df.y));
    float glow = 1.0 - smoothstep(0.0, glowSpread, distanceToLine);
	
	// Combine grid and glow
    float combinedAlpha = max(1.0 - grid, glow * glowStrength) * color.a;
	
	// ALBEDO = color.rgb;
	// ALPHA = (1.0 - grid) * color.a;
	ALBEDO = color.rgb;
    ALPHA = combinedAlpha;
}

// Thanks to people from this post
// https://godotforums.org/d/36314-draw-grid-on-floor/11