using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AmbiantManager : Singleton<AmbiantManager>
{
    public MaterialData Materials;
	public HourChangedEvent onHourChanged = new HourChangedEvent();
	public float _LastHour;
    GameObject Terrain;

    protected override void Start()
    {
        base.Start();
        //Fog = (GameObject)ObjectsManager.I["Fog"];
        //FogParticles = Fog.GetComponentInChildren<ParticleSystem>();
        //Fog.SetActive(false);
        //FogZone = Fog.GetComponentInChildren<Zone>();
        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        Terrain = (GameObject)ObjectsManager.I["Terrain"];

        InitDaysStates();
        ChangeDayState();

		ResetLastHour(CurrentDayState, NextDayState);
		onDayStateChanged.AddListener(ResetLastHour);
        onDayStateChanged.AddListener(ChangeMaterial);

        //StartChrono(GameManager.I._data.MinimumTimeBetweenFog, StartFog);
    }

    private void Update()
	{
		#region Hour Management
	
		if (CurrentTimeOfDay >= _LastHour + 1)
		{
			_LastHour = CurrentTimeOfDay;
			onHourChanged.Invoke(Mathf.RoundToInt(_LastHour), CurrentDayState.State);
		}

		#endregion
	}

    void ChangeMaterial(DayStatesProperties currentDayStateProperties, DayStatesProperties nextDayStateProperties)
    {
        UIManager.I.AlertMessage("It is " + currentDayStateProperties.State.ToString());
        if (!Materials)
            return;

        string Suffix = "D";
        List<Material> SearchList = Materials.DayMaterials;
        if (currentDayStateProperties.State == DayState.Night)
        {
            Suffix = "N";
            SearchList = Materials.NightMaterials;
        }

        for (int i = 0; i < Terrain.transform.childCount; i++)
        {
            Renderer childRenderer = Terrain.transform.GetChild(i).GetComponent<Renderer>();
            string matName = childRenderer.material.name.Replace("(Instance)", "").Trim();
            if (!matName.EndsWith("_D"))
                continue;

            string otherName = matName.Substring(0, matName.Length - 1) + Suffix;
            Material other = null;

            other = SearchList.Find(m => m.name == otherName);

            if (other)
            {
                if (other.IsKeywordEnabled("_EMISSION"))
                    childRenderer.material.EnableKeyword("_EMISSION");
                else
                    childRenderer.material.DisableKeyword("_EMISSION");
                childRenderer.material.SetColor("_EmissionColor", other.GetColor("_EmissionColor"));
            }
        }
    }

	void ResetLastHour(DayStatesProperties currentDayStateProperties, DayStatesProperties nextDayStateProperties)
	{
		_LastHour = 0;
		// Invoke at 00h00
		onHourChanged.Invoke(Mathf.RoundToInt(_LastHour), CurrentDayState.State);
	}

	public bool IsUsableNow(List<DayState> states)
    {
        if (states != null)
        {
            int i = 0;
            for (i = 0; i < states.Count; i++)
                if (states[i] == AmbiantManager.I.CurrentDayState.State)
                    break;
            if (i >= states.Count)
                return false;
        }
        return true;
    }

	#region Fog
	//GameObject Fog;
    Zone FogZone;
    Player Player;
    //ParticleSystem FogParticles;


    //void StartFog()
    //{
    //    if (GameManager.I._data.MinimumTimeBetweenFog == 0)
    //        return;
    //    Fog.SetActive(true);
    //    Fog.transform.position = new Vector3(CameraManager.I._MainCamera.transform.position.x, CameraManager.I._MainCamera.transform.position.y, CameraManager.I._MainCamera.transform.position.z);// + Vector3.ProjectOnPlane(Player.transform.forward, Vector3.up);
    //    FogParticles.Play();
    //    StartChrono(GameManager.I._data.MinimumTimeBetweenFog, EndFog);
    //    Debug.Log("Fog on");
    //}
    //void EndFog()
    //{
    //    if (GameManager.I._data.MinimumTimeBetweenFog == 0)
    //        return;
    //    FogParticles.Stop();
    //    Fog.SetActive(false) ;
    //    Player.ExitZone(FogZone);
    //    StartChrono(GameManager.I._data.MinimumTimeBetweenFog, StartFog);
    //    Debug.Log("Fog off");
    //}
    #endregion

	#region Day States Management
	public DayStateChangedEvent onDayStateChanged = new DayStateChangedEvent();
    public int CurrentDayStateIndex = 0;
    public DayStatesProperties CurrentDayState => GameManager.I._data.States[CurrentDayStateIndex];
    public DayStatesProperties NextDayState => GameManager.I._data.States[(int)Mathf.Repeat(CurrentDayStateIndex + 1, GameManager.I._data.States.Length)];
    public bool IsDay => CurrentDayState.State == DayState.Day;
    public bool IsNight => CurrentDayState.State == DayState.Night;
    public float CurrentTimeOfDay;
    public float TotalDayTime;
    void InitDaysStates()
    {
        TotalDayTime = GameManager.I._data.States[0].StateHoursCount + GameManager.I._data.States[1].StateHoursCount;

        CurrentDayStateIndex = 1;
    }
    void ChangeDayState()
    {
        CurrentDayStateIndex++;

        CurrentDayStateIndex = (int)Mathf.Repeat(CurrentDayStateIndex, GameManager.I._data.States.Length);
        GameManager.I._data.States[CurrentDayStateIndex].TimeStateChanged = Time.time;
        DayStatesProperties NextState = GameManager.I._data.States[(int)Mathf.Repeat(CurrentDayStateIndex + 1, GameManager.I._data.States.Length)];
        onDayStateChanged?.Invoke(GameManager.I._data.States[CurrentDayStateIndex], NextState);

        StartChrono(GameManager.I._data.States[CurrentDayStateIndex].DayStateDurationInSecond, ChangeDayState, DayTimeCallback);
    }
    void DayTimeCallback(float timeElapsed, float timeRatio)
    {
        CurrentTimeOfDay = Mathf.Lerp(0, CurrentDayState.StateHoursCount, timeRatio);
    }
    #endregion
}
public class DayStateChangedEvent : UnityEvent<DayStatesProperties, DayStatesProperties> { }
public class HourChangedEvent : UnityEvent<int, DayState> { }
