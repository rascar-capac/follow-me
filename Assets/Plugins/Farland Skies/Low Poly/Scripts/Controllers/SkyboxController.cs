using UnityEngine;

namespace Borodar.FarlandSkies.LowPoly
{
    [ExecuteInEditMode]
    [HelpURL("http://www.borodar.com/stuff/farlandskies/lowpoly/docs/QuickStart_v2.5.1.pdf")]
    public class SkyboxController : Borodar.FarlandSkies.Core.Helpers.Singleton<SkyboxController>
    {
        public Material SkyboxMaterial;

        // Sky

        [SerializeField]
        [Tooltip("Color at the top pole of skybox sphere")]
        private Color _topColor = Color.gray;

        [SerializeField]
        [Tooltip("Color on equator of skybox sphere")]
        private Color _middleColor = Color.gray;

        [SerializeField]
        [Tooltip("Color at the bottom pole of skybox sphere")]
        private Color _bottomColor = Color.gray;

        [SerializeField]
        [Range(0.01f, 5f)]
        [Tooltip("Color interpolation coefficient between top pole and equator")]
        private float _topExponent = 1f;

        [SerializeField]
        [Range(0.01f, 5f)]
        [Tooltip("Color interpolation coefficient between bottom pole and equator")]
        private float _bottomExponent = 1f;

        // Stars

        [SerializeField]
        private bool _starsEnabled = true;

        [SerializeField]
        private Cubemap _starsCubemap;

        [SerializeField]
        private Color _starsTint = Color.gray;

        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("Reduction in stars apparent brightness closer to the horizon")]
        private float _starsExtinction = 2f;

        [SerializeField]
        [Range(0f, 25f)]
        [Tooltip("Variation in stars apparent brightness caused by the atmospheric turbulence")]
        private float _starsTwinklingSpeed = 10f;

        // Sun

        [SerializeField]
        private bool _sunEnabled = true;

        [SerializeField]
        private Texture2D _sunTexture;

        [SerializeField]
        private Light _sunLight;

        [SerializeField]
        private Color _sunTint = Color.gray;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float _sunSize = 1f;

        [SerializeField]
        [Range(0f, 2f)]
        private float _sunHalo = 1f;

        [SerializeField]
        private bool _sunFlare = true;

        [SerializeField]
        [Range(0.01f, 2f)]
        [Tooltip("Actual flare brightness depends on sun tint alpha, and this property is just a coefficient for that value")]
        private float _sunFlareBrightness = 0.3f;

        // Moon 1

        [SerializeField]
        private bool _moon1Enabled = true;

        [SerializeField]
        private Texture2D _moon1Texture;

        [SerializeField]
        private Light _moon1Light;

        [SerializeField]
        private Color _moon1Tint = Color.gray;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float _moon1Size = 1f;

        [SerializeField]
        [Range(0f, 2f)]
        private float _moon1Halo = 1f;

        [SerializeField]
        private bool _moon1Flare = true;

        [SerializeField]
        [Range(0.01f, 2f)]
        [Tooltip("Actual flare brightness depends on moon tint alpha, and this property is just a coefficient for that value")]
        private float _moon1FlareBrightness = 0.3f;

        // Moon 2

        [SerializeField]
        private bool _moon2Enabled = true;

        [SerializeField]
        private Texture2D _moon2Texture;

        [SerializeField]
        private Light _moon2Light;

        [SerializeField]
        private Color _moon2Tint = Color.gray;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float _moon2Size = 1f;

        [SerializeField]
        [Range(0f, 2f)]
        private float _moon2Halo = 1f;

        [SerializeField]
        private bool _moon2Flare = true;

        [SerializeField]
        [Range(0.01f, 2f)]
        [Tooltip("Actual flare brightness depends on moon tint alpha, and this property is just a coefficient for that value")]
        private float _moon2FlareBrightness = 0.3f;

        // Clouds

        [SerializeField]
        private bool _cloudsEnabled = true;

        [SerializeField]
        private Cubemap _cloudsCubemap;

        [SerializeField]
        private Color _cloudsTint = Color.gray;

        [SerializeField]
        [Range(-0.75f, 0.75f)]
        [Tooltip("Height of the clouds relative to the horizon.")]
        public float _cloudsHeight = 0f;

        [SerializeField]
        [Range(0, 360f)]
        [Tooltip("Rotation of the clouds around the positive y axis.")]
        public float _cloudsRotation = 0f;

        // General

        [SerializeField]
        [Range(0, 8f)]
        private float _exposure = 1f;

        [SerializeField]
        [Tooltip("Keep fog color in sync with the sky middle color automatically")]
        private bool _adjustFogColor;

        // Private

        private LensFlare _sunFlareComponent;
        private LensFlare _moon1FlareComponent;
        private LensFlare _moon2FlareComponent;

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------

        // Sky

        public Color TopColor
        {
            get { return _topColor; }
            set
            {
                _topColor = value;
                SkyboxMaterial.SetColor("_TopColor", _topColor);
            }
        }

        public Color MiddleColor
        {
            get { return _middleColor; }
            set
            {
                _middleColor = value;
                SkyboxMaterial.SetColor("_MiddleColor", _middleColor);
            }
        }

        public Color BottomColor
        {
            get { return _bottomColor; }
            set
            {
                _bottomColor = value;
                SkyboxMaterial.SetColor("_BottomColor", _bottomColor);
            }
        }

        public float TopExponent
        {
            get { return _topExponent; }
            set
            {
                _topExponent = value;
                SkyboxMaterial.SetFloat("_TopExponent", _topExponent);
            }
        }

        public float BottomExponent
        {
            get { return _bottomExponent; }
            set
            {
                _bottomExponent = value;
                SkyboxMaterial.SetFloat("_BottomExponent", _bottomExponent);
            }
        }

        // Stars

        public bool StarsEnabled
        {
            get { return _starsEnabled; }
        }

        public Color StarsTint
        {
            get { return _starsTint; }
            set
            {
                _starsTint = value;
                SkyboxMaterial.SetColor("_StarsTint", _starsTint);
            }
        }

        public Cubemap StarsCubemap
        {
            get { return _starsCubemap; }
            set
            {
                _starsCubemap = value;
                SkyboxMaterial.SetTexture("_StarsTex", _starsCubemap);
            }
        }

        public float StarsExtinction
        {
            get { return _starsExtinction; }
            set
            {
                _starsExtinction = value;
                SkyboxMaterial.SetFloat("_StarsExtinction", _starsExtinction);
            }
        }

        public float StarsTwinklingSpeed
        {
            get { return _starsTwinklingSpeed; }
            set
            {
                _starsTwinklingSpeed = value;
                SkyboxMaterial.SetFloat("_StarsTwinklingSpeed", _starsTwinklingSpeed);
            }
        }

        // Sun

        public bool SunEnabled
        {
            get { return _sunEnabled; }
        }

        public Light SunLight
        {
            get { return _sunLight; }
            set
            {
                _sunLight = value;
            }
        }

        public Texture2D SunTexture
        {
            get { return _sunTexture; }
            set
            {
                _sunTexture = value;
                SkyboxMaterial.SetTexture("_SunTex", _sunTexture);
            }
        }

        public Color SunTint
        {
            get { return _sunTint; }
            set
            {
                _sunTint = value;
                SkyboxMaterial.SetColor("_SunTint", _sunTint);
            }
        }

        public float SunSize
        {
            get { return _sunSize; }
            set
            {
                _sunSize = value;
                SkyboxMaterial.SetFloat("_SunSize", _sunSize);
            }
        }

        public float SunHalo
        {
            get { return _sunHalo; }
            set
            {
                _sunHalo = value;
                SkyboxMaterial.SetFloat("_SunHalo", _sunHalo);
            }
        }

        public bool SunFlare
        {
            get { return _sunFlare; }
            set
            {
                _sunFlare = value;
                if (_sunFlareComponent) _sunFlareComponent.enabled = value;
            }
        }

        public float SunFlareBrightness
        {
            get { return _sunFlareBrightness; }
            set { _sunFlareBrightness = value; }
        }

        // Moon 1

        public bool Moon1Enabled
        {
            get { return _moon1Enabled; }
        }

        public Texture2D Moon1Texture
        {
            get { return _moon1Texture; }
            set
            {
                _moon1Texture = value;
                SkyboxMaterial.SetTexture("_Moon1Tex", _moon1Texture);
            }
        }

        public float Moon1Size
        {
            get { return _moon1Size; }
            set
            {
                _moon1Size = value;
                SkyboxMaterial.SetFloat("_Moon1Size", _moon1Size);
            }
        }

        public float Moon1Halo
        {
            get { return _moon1Halo; }
            set
            {
                _moon1Halo = value;
                SkyboxMaterial.SetFloat("_Moon1Halo", _moon1Halo);
            }
        }

        public Color Moon1Tint
        {
            get { return _moon1Tint; }
            set
            {
                _moon1Tint = value;
                SkyboxMaterial.SetColor("_Moon1Tint", _moon1Tint);
            }
        }

        public Light Moon1Light
        {
            get { return _moon1Light; }
            set
            {
                _moon1Light = value;
            }
        }

        public bool Moon1Flare
        {
            get { return _moon1Flare; }
            set
            {
                _moon1Flare = value;
                if (_moon1FlareComponent) _moon1FlareComponent.enabled = value;
            }
        }

        public float Moon1FlareBrightness
        {
            get { return _moon1FlareBrightness; }
            set { _moon1FlareBrightness = value; }
        }

        // Moon 2

        public bool Moon2Enabled
        {
            get { return _moon2Enabled; }
        }

        public Texture2D Moon2Texture
        {
            get { return _moon2Texture; }
            set
            {
                _moon2Texture = value;
                SkyboxMaterial.SetTexture("_Moon2Tex", _moon2Texture);
            }
        }

        public float Moon2Size
        {
            get { return _moon2Size; }
            set
            {
                _moon2Size = value;
                SkyboxMaterial.SetFloat("_Moon2Size", _moon2Size);
            }
        }

        public float Moon2Halo
        {
            get { return _moon2Halo; }
            set
            {
                _moon2Halo = value;
                SkyboxMaterial.SetFloat("_Moon2Halo", _moon2Halo);
            }
        }

        public Color Moon2Tint
        {
            get { return _moon2Tint; }
            set
            {
                _moon2Tint = value;
                SkyboxMaterial.SetColor("_Moon2Tint", _moon2Tint);
            }
        }

        public Light Moon2Light
        {
            get { return _moon2Light; }
            set
            {
                _moon2Light = value;
            }
        }

        public bool Moon2Flare
        {
            get { return _moon2Flare; }
            set
            {
                _moon2Flare = value;
                if (_moon2FlareComponent) _moon2FlareComponent.enabled = value;
            }
        }

        public float Moon2FlareBrightness
        {
            get { return _moon2FlareBrightness; }
            set { _moon2FlareBrightness = value; }
        }

        // Clouds

        public bool CloudsEnabled
        {
            get { return _cloudsEnabled; }
        }

        public Cubemap CloudsCubemap
        {
            get { return _cloudsCubemap; }
            set
            {
                _cloudsCubemap = value;
                SkyboxMaterial.SetTexture("_CloudsTex", _cloudsCubemap);
            }
        }

        public Color CloudsTint
        {
            get { return _cloudsTint; }
            set
            {
                _cloudsTint = value;
                SkyboxMaterial.SetColor("_CloudsTint", _cloudsTint);
            }
        }

        public float CloudsRotation
        {
            get { return _cloudsRotation; }
            set
            {
                _cloudsRotation = value;
                SkyboxMaterial.SetFloat("_CloudsRotation", _cloudsRotation);
            }
        }

        public float CloudsHeight
        {
            get { return _cloudsHeight; }
            set
            {
                _cloudsHeight = value;
                SkyboxMaterial.SetFloat("_CloudsHeight", _cloudsHeight);
            }
        }

        // General

        public float Exposure
        {
            get { return _exposure; }
            set
            {
                _exposure = value;
                SkyboxMaterial.SetFloat("_Exposure", _exposure);
            }
        }

        public bool AdjustFogColor
        {
            get { return _adjustFogColor; }
            set
            {
                _adjustFogColor = value;
                if (_adjustFogColor) RenderSettings.fogColor = MiddleColor;
            }
        }

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        protected void Awake()
        {
            UpdateSkyboxProperties();
        }

        protected void OnValidate()
        {
            UpdateSkyboxProperties();
        }

        protected void Update()
        {
            if (SkyboxMaterial == null) return;

            if (_adjustFogColor) RenderSettings.fogColor = MiddleColor;

            if (_sunEnabled)
            {
                SkyboxMaterial.SetMatrix("sunMatrix", _sunLight.transform.worldToLocalMatrix);
                if (_sunFlare && _sunFlareComponent) _sunFlareComponent.brightness = _sunTint.a * _sunFlareBrightness;
            }

            if (_moon1Enabled)
            {
                SkyboxMaterial.SetMatrix("moon1Matrix", _moon1Light.transform.worldToLocalMatrix);
                if (_moon1Flare && _moon1FlareComponent) _moon1FlareComponent.brightness = _moon1Tint.a * _moon1FlareBrightness;
            }

            if (_moon2Enabled)
            {
                SkyboxMaterial.SetMatrix("moon2Matrix", _moon2Light.transform.worldToLocalMatrix);
                if (_moon2Flare && _moon2FlareComponent) _moon2FlareComponent.brightness = _moon2Tint.a * _moon2FlareBrightness;
            }
        }

        //---------------------------------------------------------------------
        // Helpers
        //---------------------------------------------------------------------

        private void UpdateSkyboxProperties()
        {
            if (SkyboxMaterial == null)
            {
                Debug.LogWarning("SkyboxController: Skybox material is not assigned.");
                return;
            }

            // Sky

            SkyboxMaterial.SetColor("_TopColor", _topColor);
            SkyboxMaterial.SetColor("_MiddleColor", _middleColor);
            SkyboxMaterial.SetColor("_BottomColor", _bottomColor);
            SkyboxMaterial.SetFloat("_TopExponent", _topExponent);
            SkyboxMaterial.SetFloat("_BottomExponent", _bottomExponent);

            // Stars

            if (_starsEnabled)
            {
                SkyboxMaterial.DisableKeyword("STARS_OFF");
                SkyboxMaterial.SetTexture("_StarsTex", _starsCubemap);
                SkyboxMaterial.SetColor("_StarsTint", _starsTint);
                SkyboxMaterial.SetFloat("_StarsExtinction", _starsExtinction);
                SkyboxMaterial.SetFloat("_StarsTwinklingSpeed", _starsTwinklingSpeed);
            }
            else
            {
                SkyboxMaterial.EnableKeyword("STARS_OFF");
            }

            // Sun

            if (_sunEnabled)
            {
                SkyboxMaterial.DisableKeyword("SUN_OFF");
                SkyboxMaterial.SetTexture("_SunTex", _sunTexture);
                SkyboxMaterial.SetFloat("_SunSize", _sunSize);
                SkyboxMaterial.SetFloat("_SunHalo", _sunHalo);
                SkyboxMaterial.SetColor("_SunTint", _sunTint);

                if (_sunLight)
                {
                    _sunLight.gameObject.SetActive(true);
                    _sunFlareComponent = _sunLight.GetComponent<LensFlare>();
                }
                else
                {
                    Debug.LogWarning("SkyboxController: Sun light object is not assigned.");
                }

                if (_sunFlareComponent) _sunFlareComponent.enabled = _sunFlare;
            }
            else
            {
                SkyboxMaterial.EnableKeyword("SUN_OFF");
                if (_sunLight) _sunLight.gameObject.SetActive(false);
            }

            // Moon 1

            if (_moon1Enabled)
            {
                SkyboxMaterial.DisableKeyword("MOON1_OFF");
                SkyboxMaterial.SetTexture("_Moon1Tex", _moon1Texture);
                SkyboxMaterial.SetFloat("_Moon1Size", _moon1Size);
                SkyboxMaterial.SetFloat("_Moon1Halo", _moon1Halo);
                SkyboxMaterial.SetColor("_Moon1Tint", _moon1Tint);

                if (_moon1Light)
                {
                    _moon1Light.gameObject.SetActive(true);
                    _moon1FlareComponent = _moon1Light.GetComponent<LensFlare>();
                }
                else
                {
                    Debug.LogWarning("SkyboxController: Moon 1 light object is not assigned.");
                }

                if (_moon1FlareComponent) _moon1FlareComponent.enabled = _moon1Flare;
            }
            else
            {
                SkyboxMaterial.EnableKeyword("MOON1_OFF");
                if (_moon1Light) _moon1Light.gameObject.SetActive(false);
            }

            // Moon 2

            if (_moon2Enabled)
            {
                SkyboxMaterial.DisableKeyword("MOON2_OFF");
                SkyboxMaterial.SetTexture("_Moon2Tex", _moon2Texture);
                SkyboxMaterial.SetFloat("_Moon2Size", _moon2Size);
                SkyboxMaterial.SetFloat("_Moon2Halo", _moon2Halo);
                SkyboxMaterial.SetColor("_Moon2Tint", _moon2Tint);

                if (_moon2Light)
                {
                    _moon2Light.gameObject.SetActive(true);
                    _moon2FlareComponent = _moon2Light.GetComponent<LensFlare>();
                }
                else
                {
                    Debug.LogWarning("SkyboxController: Moon 2 light object is not assigned.");
                }

                if (_moon2FlareComponent) _moon2FlareComponent.enabled = _moon2Flare;
            }
            else
            {
                SkyboxMaterial.EnableKeyword("MOON2_OFF");
                if (_moon2Light) _moon2Light.gameObject.SetActive(false);
            }

            // Clouds

            if (_cloudsEnabled)
            {
                SkyboxMaterial.DisableKeyword("CLOUDS_OFF");
                SkyboxMaterial.SetTexture("_CloudsTex", _cloudsCubemap);
                SkyboxMaterial.SetColor("_CloudsTint", _cloudsTint);
                SkyboxMaterial.SetFloat("_CloudsRotation", _cloudsRotation);
                SkyboxMaterial.SetFloat("_CloudsHeight", _cloudsHeight);
            }
            else
            {
                SkyboxMaterial.EnableKeyword("CLOUDS_OFF");
            }

            // General

            SkyboxMaterial.SetFloat("_Exposure", _exposure);

            // Update skybox

            RenderSettings.skybox = SkyboxMaterial;
        }
    }
}