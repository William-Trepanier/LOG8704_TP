// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Adaptation du shader Skybox/6-sides pour ajouter des effets relativistes au skybox.
// Source
// https://en.wikipedia.org/wiki/Relativistic_aberration
// https://en.wikipedia.org/wiki/Rapidity
// https://en.wikipedia.org/wiki/Lorentz_factor
// https://en.wikipedia.org/wiki/Relativistic_Doppler_effect
// https://en.wikipedia.org/wiki/Relative_luminance
// https://en.wikipedia.org/wiki/Planck%27s_law

Shader "Skybox/Relativistic" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _Rotation ("Rotation", Range(0, 360)) = 0
    [NoScaleOffset] _Tex ("Cubemap (HDR)", Cube) = "grey" {}
    
    _Velocity ("Velocity Direction", Vector) = (0, 0, 1, 0)
    
    // On utilise la Rapidité atanh v/c pour éviter les problèmes de précisions qu'on aurait avec le pourcentage de la 
    // vitesse de la lumière. v étant la vitesse et c étant la vitesse de la lumière.
    _Rapidity ("Rapidity (atanh v/c)", Range(0, 12)) = 0
    
    // CMB (Cosmic Microwave Background). Température en K du CMB. Après une certaine vitesse, le CMB devient visible dû
    // à l'effet de Doppler
    _CmbTemperature ("CMB Temperature", Float) = 2.725
    
    // Température du blanc pur à l'écran. La température qui donne une couleur (1, 1, 1)
    _WhiteTemperature ("White Temperature", Float) = 6500
    
    // Atténuation de la lueur du CMB
    _CmbFadeTemperature ("CMB Fade Temperature", Float) = 20000
    
    // Maximum de l'amplification de la luminosité.
    _MaxBoost ("Max Headlight Boost", Range(1, 64)) = 16
    
    // Force de l'effet de Doppler sur le skybox. 0 = seul l'aberration est visible, 1 = effet de couleur complet.
    _DopplerStrength ("Doppler Color Strength", Range(0, 1)) = 1
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0

        #include "UnityCG.cginc"

        samplerCUBE _Tex;
        half4 _Tex_HDR;
        half4 _Tint;
        half _Exposure;
        float _Rotation;
        float4 _Velocity;
        float _Rapidity;
        float _CmbTemperature;
        float _WhiteTemperature;
        float _CmbFadeTemperature;
        float _MaxBoost;
        float _DopplerStrength;

        float3 RotateAroundYInDegrees (float3 vertex, float degrees)
        {
            float alpha = degrees * UNITY_PI / 180.0;
            float sina, cosa;
            sincos(alpha, sina, cosa);
            float2x2 m = float2x2(cosa, -sina, sina, cosa);
            return float3(mul(m, vertex.xz), vertex.y).xzy;
        }

        struct appdata_t {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2_f {
            float4 vertex : SV_POSITION;
            float3 worldDir : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2_f vert (appdata_t v)
        {
            v2_f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            float3 rotated = RotateAroundYInDegrees(v.vertex.xyz, _Rotation);
            o.vertex = UnityObjectToClipPos(rotated);
            o.worldDir = rotated;
            return o;
        }
        
        // Combien un corps à la température T en Kelvin rayonne à la longueur d'onde lambda
        // Simplification de la formule réelle puisqu'on l'utilise toujours dans un rapport à la même longueur d'onde.
        // On peut donc supprimer complétement le terme qui dépend seulement de la longueur d'onde.
        // La formule complète est :
        //      B(lambda, T) = [2hc² / (lambda^5)] * [1 / (e^x - 1)]
        //      x = hc / (lambda * kb * T)
        //      on supprime le terme suivant puisque ça dépend seulement de la longueur d'onde : [2hc² / (lambda^5)]
        // h = Constante de Planck (6.626 * 10^-34)
        // c = Vitesse de la lumière (2.998 * 10^8)
        // kb = Constante de Boltzmann (1.381 * 10^-23)
        // hc / kb = 0.014388, on multiplie par 1 000 000 puisqu'on utilise des longueurs d'ondes en micro mètre
        float PlanckRelative(float lambda, float T)
        {
            float x = 14388.0 / (lambda * T);
            return x > 80.0 ? 0.0 : 1.0 / (exp(x) - 1.0); // Si x > 80, le résultat est pratiquement 0
        }

        // Normalisation de la couleur du corps noir en considérant que Twhite est équivalent à une valeur 1 pour chaque
        // canal de couleur.
        float3 BlackbodyRGB(float temperature, float whiteTemperature)
        {
            const float3 lambda = float3(0.62, 0.54, 0.46); // Longueur d'onde des couleurs primaires
            return float3(
                PlanckRelative(lambda.r, temperature) / PlanckRelative(lambda.r, whiteTemperature),
                PlanckRelative(lambda.g, temperature) / PlanckRelative(lambda.g, whiteTemperature),
                PlanckRelative(lambda.b, temperature) / PlanckRelative(lambda.b, whiteTemperature));
        }

        fixed4 frag (v2_f i) : SV_Target
        {
            float3 pixeldir = normalize(i.worldDir);
            
            // Direction du mouvement. Si le mouvement est à 0 on prend simplement la direction forward (z positif)
            float3 movementDir = length(_Velocity.xyz) > 1e-5 ? normalize(_Velocity.xyz) : float3(0, 0, 1);

            // Calcul de l'abberation.
            // On fait la conversion entre le repère source (Repère où les étoiles sont fixes, soit le visuel de base du Skybox)
            // et le repère de l'observateur (Repère du vaisseau qui avance à une vitesse proche de c)
            // On connait la direction des pixels par rapport à l'obervateur donc on fait le calcul de la direction des 
            // pixels par rapport à la source.
            // Les formules utilisés pour calculer les deux angles de la source sont
            // cosSrc = (cosObs − beta) / (1 − beta * cosObs)
            // sinSrc = sinObs / (gamma * (1 * beta * cosObs)
            // srcWorld = cosSrc * axe + sinSrc * side
            
            float beta = tanh(_Rapidity); // Équivalent de v / c
            float gamma  = cosh(_Rapidity); // Facteur de Lorentz
            float oneMinusBeta = exp(-_Rapidity) / gamma; // On calcul comme ça plutôt que 1 - beta à cause du manque de précision d'un float
            
            // Transformations pour obtenir les valeurs connus
            float cosObs = dot(pixeldir, movementDir);
            float3 pixelDirMinusMovementDir = pixeldir - movementDir;
            float oneMinusCos = 0.5 * dot(pixelDirMinusMovementDir, pixelDirMinusMovementDir);
            
            // Calcul des angles
            float denominator = oneMinusBeta + beta * oneMinusCos; // (1 − beta * cosObs)
            float numerator = oneMinusBeta - oneMinusCos; // cosObs - beta
            float cosSrc = numerator / denominator;
            float3 perpendicular = pixeldir - cosObs * movementDir;
            float sinObs = length(perpendicular);
            float sinSrc = sinObs / (gamma * denominator);
            
            // Transformation des angles en vecteur
            float3 sideDir = perpendicular / max(sinObs, 1e-5);
            float3 srcWorld = cosSrc * movementDir + sinSrc * sideDir;
            
            float3 sampleDir = RotateAroundYInDegrees(srcWorld, -_Rotation);

            half4 tex = texCUBE(_Tex, sampleDir);
            half3 color = DecodeHDR(tex, _Tex_HDR);
            color *= _Tint.rgb * unity_ColorSpaceDouble.rgb * _Exposure;

            // Calcul de l'effet Doppler sur la couleur
            float dopplerFactor = 1.0 / (gamma * denominator);

            // Changement de couleur des étoiles (skybox)
            const float3 luminanceRelative = float3(0.2126, 0.7152, 0.0722);
            float3 shift = BlackbodyRGB(dopplerFactor * _WhiteTemperature, _WhiteTemperature);
            shift *= min(1.0, _MaxBoost / max(dot(shift, luminanceRelative), 1e-6));
            color *= lerp(float3(1, 1, 1), shift, _DopplerStrength);

            // Changement de couleur du CMB (Apparait par dessus les étoiles)
            float  cmbTemp   = dopplerFactor * _CmbTemperature;
            float3 cmb = BlackbodyRGB(cmbTemp, _WhiteTemperature);
            float  tf  = cmbTemp / _CmbFadeTemperature;
            cmb *= 1.0 / (1.0 + tf * tf); // atténuation pour ne pas saturer l'écran
            color += cmb;

            return half4(color, 1);
        }
        ENDCG
    }
}


Fallback Off

}
