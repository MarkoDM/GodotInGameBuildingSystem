shader_type canvas_item;

uniform vec2 viewport_size;
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
    vec2 size = 1.0 / viewport_size;
    vec4 color = texture(screen_texture, SCREEN_UV);

    // Box blur size
    int radius = 4;

    for(int i = -radius; i <= radius; i++) {
        for(int j = -radius; j <= radius; j++) {
            color += texture(screen_texture, SCREEN_UV + vec2(float(i), float(j)) * size);
        }
    }

    color /= pow(float(radius * 2 + 1), 2.0);
    COLOR = color;
}
