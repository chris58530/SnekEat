Shader "Custom/UI_GlitchEffect_Interval"
{
    Properties
    {
        // 主貼圖
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Glitch Settings)]
        _Amount ("Glitch Amount (強度)", Range(0, 0.1)) = 0.01
        _ChromaticAberration ("Chromatic Aberration (色差)", Range(0, 0.05)) = 0.015
        _Speed ("Speed (速度)", Range(0, 50)) = 10.0
        _Frequency ("Frequency (密度)", Range(0, 50)) = 10.0

        [Header(Time Trigger Settings)]
        _GlitchInterval ("Interval (間隔秒數)", Range(1.0, 10.0)) = 3.0
        _GlitchDuration ("Duration (持續時間)", Range(0.1, 2.0)) = 0.5
        
        // UI 必要屬性
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _MainTex_ST;

            float _Amount;
            float _ChromaticAberration;
            float _Speed;
            float _Frequency;
            
            // 新增的時間變數
            float _GlitchInterval;
            float _GlitchDuration;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                
                // --- 時間控制邏輯 ---
                // 使用 fmod (取餘數) 讓時間在 0 ~ Interval 之間循環
                // 例如 Interval 是 3，時間就會是 0, 1, 2, 2.9, 0, 1...
                float timeVal = _Time.y;
                float triggerTimer = fmod(timeVal, _GlitchInterval);

                // 判斷是否在持續時間內
                // step(a, b) => 如果 b >= a 則回傳 1，否則回傳 0
                // 這裡的意思是：如果 "持續時間" >= "目前循環時間"，則 isGlitching = 1，否則 = 0
                float isGlitching = step(triggerTimer, _GlitchDuration);

                // --- Glitch 核心計算 ---
                
                // 如果 isGlitching 是 0，這裡的 t 雖然有在跑，但後面會被乘上 0
                float t = timeVal * _Speed;
                
                float jitter = sin(uv.y * _Frequency + t) * sin(t * 8.76) * sin(t * 19.12);
                jitter = pow(abs(jitter), 3.0) * sign(jitter); 

                // 關鍵：將計算出來的抖動值 乘上 開關(0 或 1)
                // 這樣當時間沒到的時候，偏移量就是 0，畫面就會正常
                float shiftAmount = jitter * _Amount * isGlitching;
                float chromaticShift = jitter * _ChromaticAberration * isGlitching;

                // --- 採樣與合成 ---
                
                fixed4 rCol = tex2D(_MainTex, float2(uv.x - chromaticShift - shiftAmount, uv.y));
                fixed4 gCol = tex2D(_MainTex, float2(uv.x + shiftAmount * 0.5, uv.y));
                fixed4 bCol = tex2D(_MainTex, float2(uv.x + chromaticShift + shiftAmount, uv.y));

                fixed4 finalColor;
                finalColor.r = rCol.r;
                finalColor.g = gCol.g;
                finalColor.b = bCol.b;
                finalColor.a = (rCol.a + gCol.a + bCol.a) / 3.0;

                return finalColor * IN.color;
            }
            ENDCG
        }
    }
}