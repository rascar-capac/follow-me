#pragma warning disable 649
using System.Diagnostics.CodeAnalysis;
using Borodar.FarlandSkies.LowPoly.DotParams;
using UnityEngine;

namespace Borodar.FarlandSkies.LowPoly
{
    [ExecuteInEditMode]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [HelpURL("http://www.borodar.com/stuff/farlandskies/lowpoly/docs/QuickStart_v2.5.1.pdf")]
    public class SkyboxDayNightCycle : Borodar.FarlandSkies.Core.Helpers.Singleton<SkyboxDayNightCycle>
    {
        public Color DayMultiplier = Color.white;
        public Color NightMultiplier = Color.white;

        // Sky

        [SerializeField]
        private SkyParamsList _skyParamsList = new SkyParamsList();

        // Stars

        [SerializeField]
        private StarsParamsList _starsParamsList = new StarsParamsList();

        // Sun

        [SerializeField]
        [Range(0, 100)]
        public float _sunrise = 6f;

        [SerializeField]
        [Range(0, 100)]
        public float _sunset = 20f;

        [SerializeField]
        private float _sunAltitude = 45f;

        [SerializeField]
        [Tooltip("Angle between z-axis and the center of sun’s disk at sunrise")]
        private float _sunLongitude = 0f;

        [SerializeField]
        [Tooltip("A pair of angles that limit visible orbit of the sun")]
        private Vector2 _sunOrbit = new Vector2(-20f, 200f);

        [SerializeField]
        private CelestialParamsList _sunParamsList = new CelestialParamsList();

        // Moon 1

        [SerializeField]
        [Range(0, 100)]
        public float _moonrise1 = 22f;

        [SerializeField]
        [Range(0, 100)]
        public float _moonset1 = 5f;

        [SerializeField]
        [Tooltip("Max angle between the horizon and the center of moon’s disk")]
        private float _moon1Altitude = 45f;

        [SerializeField]
        [Tooltip("Angle between z-axis and the center of moon’s disk at moonrise")]
        private float _moon1Longitude = 0f;

        [SerializeField]
        [Tooltip("A pair of angles that limit visible orbit of the moon")]
        private Vector2 _moon1Orbit = new Vector2(-20f, 200f);

        [SerializeField]
        private CelestialParamsList _moon1ParamsList = new CelestialParamsList();

        // Moon 2

        [SerializeField]
        [Range(0, 100)]
        public float _moonrise2 = 22f;

        [SerializeField]
        [Range(0, 100)]
        public float _moonset2 = 5f;

        [SerializeField]
        [Tooltip("Max angle between the horizon and the center of moon’s disk")]
        private float _moon2Altitude = 45f;

        [SerializeField]
        [Tooltip("Angle between z-axis and the center of moon’s disk at moonrise")]
        private float _moon2Longitude = 0f;

        [SerializeField]
        [Tooltip("A pair of angles that limit visible orbit of the moon")]
        private Vector2 _moon2Orbit = new Vector2(-20f, 200f);

        [SerializeField]
        private CelestialParamsList _moon2ParamsList = new CelestialParamsList();

        // Clouds

        [SerializeField]
        private CloudsParamsList _cloudsParamsList = new CloudsParamsList();

        // General

        [SerializeField]
        [Tooltip("Reduce the skybox day-night cycle update to run every \"n\" frames")]
        private int _framesInterval = 2;

        // Private

        private SkyboxController _skyboxController;

        private float _sunDuration;
        private Vector3 _sunAttitudeVector;
        private float _moon1Duration;
        private Vector3 _moon1AttitudeVector;
        private float _moon2Duration;
        private Vector3 _moon2AttitudeVector;
        private int _framesToSkip;
        private bool _initialized;

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------

        private float _timeOfDay;

        /// <summary>
        /// Time of day, in percents (0-100).</summary>
        public float TimeOfDay
        {
            get { return _timeOfDay; }
            set { _timeOfDay = value % 100; }
        }

        public SkyParam CurrentSkyParam { get; private set;  }
        public StarsParam CurrentStarsParam { get; private set; }
        public CelestialParam CurrentSunParam { get; private set; }
        public CelestialParam CurrentMoon1Param { get; private set; }
        public CelestialParam CurrentMoon2Param { get; private set; }
        public CloudsParam CurrentCloudsParam { get; private set; }

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        protected void Awake()
        {
            // Sun position
            _sunDuration = (_sunrise < _sunset) ? _sunset - _sunrise : 100f - _sunrise + _sunset;

            var radAngle = _sunAltitude * Mathf.Deg2Rad;
            _sunAttitudeVector = new Vector3(Mathf.Sin(radAngle), Mathf.Cos(radAngle));

            // Moon 1 position
            _moon1Duration = (_moonrise1 < _moonset1) ? _moonset1 - _moonrise1 : 100f - _moonrise1 + _moonset1;

            radAngle = _moon1Altitude * Mathf.Deg2Rad;
            _moon1AttitudeVector = new Vector3(Mathf.Sin(radAngle), Mathf.Cos(radAngle));

            // Moon 2 position
            _moon2Duration = (_moonrise2 < _moonset2) ? _moonset2 - _moonrise2 : 100f - _moonrise2 + _moonset2;

            radAngle = _moon2Altitude * Mathf.Deg2Rad;
            _moon2AttitudeVector = new Vector3(Mathf.Sin(radAngle), Mathf.Cos(radAngle));

            // DOT params
            _skyParamsList.Init();
            _starsParamsList.Init();
            _sunParamsList.Init();
            _moon1ParamsList.Init();
            _moon2ParamsList.Init();
            _cloudsParamsList.Init();
        }

        public void Start()
        {
            _skyboxController = SkyboxController.Instance;
            CurrentSkyParam = _skyParamsList.GetParamPerTime(TimeOfDay);
            CurrentStarsParam = _starsParamsList.GetParamPerTime(TimeOfDay);
            CurrentSunParam = _sunParamsList.GetParamPerTime(TimeOfDay);
            CurrentMoon1Param = _moon1ParamsList.GetParamPerTime(TimeOfDay);
            CurrentMoon2Param = _moon2ParamsList.GetParamPerTime(TimeOfDay);
            CurrentCloudsParam = _cloudsParamsList.GetParamPerTime(TimeOfDay);
            _initialized = true;
        }

        public void Update()
        {
            if (--_framesToSkip > 0) return;
            _framesToSkip = _framesInterval;

            UpdateSky();
            UpdateStars();
            UpdateSun();
            UpdateMoon1();
            UpdateMoon2();
            UpdateClouds();
        }

        protected void OnValidate()
        {
            if (!_initialized) return;
            _skyboxController = SkyboxController.Instance;

            // Sky
            _skyParamsList.Update();

            // Stars
            if (_skyboxController.StarsEnabled)
            {
                _starsParamsList.Update();
            }

            // Sun
            if (_skyboxController.SunEnabled)
            {
                _sunParamsList.Update();

                // position
                _sunDuration = (_sunrise < _sunset) ? _sunset - _sunrise : 100f - _sunrise + _sunset;
                var radAngle = _sunAltitude * Mathf.Deg2Rad;
                _sunAttitudeVector = new Vector3(Mathf.Sin(radAngle), Mathf.Cos(radAngle));
            }

            // Moon 1
            if (_skyboxController.Moon1Enabled)
            {
                _moon1ParamsList.Update();

                // position
                _moon1Duration = (_moonrise1 < _moonset1) ? _moonset1 - _moonrise1 : 100f - _moonrise1 + _moonset1;
                var radAngle = _moon1Altitude * Mathf.Deg2Rad;
                _moon1AttitudeVector = new Vector3(Mathf.Sin(radAngle), Mathf.Cos(radAngle));
            }

            // Moon 2
            if (_skyboxController.Moon2Enabled)
            {
                _moon2ParamsList.Update();

                // position
                _moon2Duration = (_moonrise2 < _moonset2) ? _moonset2 - _moonrise2 : 100f - _moonrise2 + _moonset2;
                var radAngle = _moon2Altitude * Mathf.Deg2Rad;
                _moon2AttitudeVector = new Vector3(Mathf.Sin(radAngle), Mathf.Cos(radAngle));
            }

            // Clouds
            if (_skyboxController.CloudsEnabled)
            {
                _cloudsParamsList.Update();
            }
        }

        //---------------------------------------------------------------------
        // Helpers
        //---------------------------------------------------------------------

        private void UpdateSky()
        {
            CurrentSkyParam = _skyParamsList.GetParamPerTime(TimeOfDay);

            _skyboxController.TopColor = CurrentSkyParam.TopColor;
            _skyboxController.MiddleColor = CurrentSkyParam.MiddleColor;
            _skyboxController.BottomColor = CurrentSkyParam.BottomColor;
            _skyboxController.CloudsTint = CurrentSkyParam.CloudsTint;
        }

        private void UpdateStars()
        {
            if (!_skyboxController.StarsEnabled) return;
            CurrentStarsParam = _starsParamsList.GetParamPerTime(TimeOfDay);
            _skyboxController.StarsTint = CurrentStarsParam.TintColor;
        }

        private void UpdateSun()
        {
            if (!_skyboxController.SunEnabled) return;

            // rotation
            if (TimeOfDay > _sunrise || TimeOfDay < _sunset)
            {
                var sunCurrent = (_sunrise < TimeOfDay) ? TimeOfDay - _sunrise : 100f + TimeOfDay - _sunrise;
                var ty = (sunCurrent < _sunDuration) ? sunCurrent / _sunDuration : (_sunDuration - sunCurrent) / _sunDuration;
                var dy = Mathf.Lerp(_sunOrbit.x, _sunOrbit.y, ty);
                var rotation = Quaternion.AngleAxis(_sunLongitude - 180, Vector3.up) * Quaternion.AngleAxis(dy, _sunAttitudeVector);
                rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
                _skyboxController.SunLight.transform.rotation = rotation;
            }

            // colors
            CurrentSunParam = _sunParamsList.GetParamPerTime(TimeOfDay);

            if (DayMultiplier != Color.white)
            {
                _skyboxController.SunLight.color = DayMultiplier;
                _skyboxController.SunTint = DayMultiplier;
            }
            else
            {
                _skyboxController.SunLight.color = CurrentSunParam.LightColor;
                _skyboxController.SunTint = CurrentSunParam.TintColor;
            }
            _skyboxController.SunLight.intensity = CurrentSunParam.LightIntencity;
        }

        private void UpdateMoon1()
        {
            if (!_skyboxController.Moon1Enabled) return;

            // rotation
            if (TimeOfDay > _moonrise1 || TimeOfDay < _moonset1)
            {
                var moonCurrent = (_moonrise1 < TimeOfDay) ? TimeOfDay - _moonrise1 : 100f + TimeOfDay - _moonrise1;
                var ty = (moonCurrent < _moon1Duration) ? moonCurrent / _moon1Duration : (_moon1Duration - moonCurrent) / _moon1Duration;
                var dy = Mathf.Lerp(_moon1Orbit.x, _moon1Orbit.y, ty);
                var rotation = Quaternion.AngleAxis(_moon1Longitude - 180, Vector3.up) * Quaternion.AngleAxis(dy, _moon1AttitudeVector);
                rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
                _skyboxController.Moon1Light.transform.rotation = rotation;
            }

            // colors
            CurrentMoon1Param = _moon1ParamsList.GetParamPerTime(TimeOfDay);


            if (NightMultiplier != Color.white)
            {
                _skyboxController.Moon1Light.color = NightMultiplier;
                _skyboxController.Moon1Tint = NightMultiplier;
            }
            else
            {
                _skyboxController.Moon1Tint = CurrentMoon1Param.TintColor;
                _skyboxController.Moon1Light.color = CurrentMoon1Param.LightColor;
            }

            _skyboxController.Moon1Light.intensity = CurrentMoon1Param.LightIntencity;
        }

        private void UpdateMoon2()
        {
            if (!_skyboxController.Moon2Enabled) return;

            // rotation
            if (TimeOfDay > _moonrise2 || TimeOfDay < _moonset2)
            {
                var moonCurrent = (_moonrise2 < TimeOfDay) ? TimeOfDay - _moonrise2 : 100f + TimeOfDay - _moonrise2;
                var ty = (moonCurrent < _moon2Duration) ? moonCurrent / _moon2Duration : (_moon2Duration - moonCurrent) / _moon2Duration;
                var dy = Mathf.Lerp(_moon2Orbit.x, _moon2Orbit.y, ty);
                var rotation = Quaternion.AngleAxis(_moon2Longitude - 180, Vector3.up) * Quaternion.AngleAxis(dy, _moon2AttitudeVector);
                rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
                _skyboxController.Moon2Light.transform.rotation = rotation;
            }

            // colors
            CurrentMoon2Param = _moon2ParamsList.GetParamPerTime(TimeOfDay);


            if (NightMultiplier != Color.white)
            {
                _skyboxController.Moon2Light.color = NightMultiplier;
                _skyboxController.Moon2Tint = NightMultiplier;
            }
            else
            {
                _skyboxController.Moon2Tint = CurrentMoon2Param.TintColor;
                _skyboxController.Moon2Light.color = CurrentMoon2Param.LightColor;
            }

            _skyboxController.Moon2Light.intensity = CurrentMoon2Param.LightIntencity;
        }

        private void UpdateClouds()
        {
            if (!_skyboxController.CloudsEnabled) return;
            CurrentCloudsParam = _cloudsParamsList.GetParamPerTime(TimeOfDay);
            _skyboxController.CloudsTint = CurrentCloudsParam.TintColor;
        }
    }
}