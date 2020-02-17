using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using Borodar.FarlandSkies.LowPoly;

public enum TribeMovementsMode
{
	Wait,
	FollowPlayer,
	GoToBeacon,
    BeSpontaneous
}

[System.Serializable]
public struct TribeProperties
{
	public float MaxSpeed;
	public float MaxAngularSpeed;
	public float AccelerationForce;
	public float DecelerationForce;
	public float MinDistForAcceleration;
	public float MinDistForDeceleration;
	public float CriticalSpeedMultiplicator;
}

public enum TribeEmotionMode
{
    Normal,
    Aggress,
    Fear,
    Happy
}
public enum RotationDirection
{
    Left,
    Right,
    None
}

public class Tribe : ZoneInteractable
{
	public TribeProperties TribeProperties;
    public UnityEvent onEnergyFull = new UnityEvent();
    public Color IgnoringEmissionColor;

	[ShowInInspector][ReadOnly]
	TribeMovementsMode _TribeMovementsMode;

	#region Tribe Energy

	[Header("Current Tribe energy")]
	public float Energy = 100f;
	float CriticalEnergy;
    bool IsEnergyCritical => Energy <= CriticalEnergy;
    bool PreviousIsEnergyCritical = false;

    #endregion

	#region Tribe Events

	public UnityEvent onTribeEnergyEnterCritical = new UnityEvent();
	public UnityEvent onTribeEnergyExitCritical = new UnityEvent();

	#endregion

	//NavMeshAgent _TribeNavAgent;
	Player _Player;
	PlayerMovement _PlayerMovement;
    PlayerInventory _PlayerInventory;
    Terrain _Terrain;
    Animator animator;

    [HideInInspector]
    public int DocilityScore;
    int _DocilityLevel;
    bool _IsIgnoring;
    float _IgnoranceProbability;
    int _IgnoranceDuration;
    float _IgnoranceTimer;
    float _SpontaneityProbability;
    float _SpontaneityCheckTimer;
    Color _DefaultEmissionColor;


	protected override void Start()
    {
        base.Start();
		animator = GetComponentInChildren<Animator>();

		//TribeProperties = GameManager.I._data.TribeProperties;

		//_TribeNavAgent = GetComponentInParent<NavMeshAgent>();
        _Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
		_PlayerMovement = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
        _PlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
        _Terrain = ((GameObject)ObjectsManager.I["Terrain"]).GetComponent<Terrain>();

		//Energy = GameManager.I._data.InitialTribeEnergy;
		//CriticalEnergy = (Energy / 100) * GameManager.I._data.PercentEnergyTribeForCritical;

		//TribeInDefaultSpeed();
		//SetNavMeshAgent();

		//onTribeEnergyEnterCritical.AddListener(TribeInCriticalSpeed);
		//onTribeEnergyExitCritical.AddListener(TribeInDefaultSpeed);
		//InputManager.I.onTribeOrderKeyPressed.AddListener(SwitchModeFollowAndWait);
        //_PlayerInventory.onItemActivated.AddListener(AddItemActivationBonus);
        //AmbiantManager.I.onDayStateChanged.AddListener(AddNewDayBonus);
        //_PlayerMovement.onPlayerTooFarFromTribe.AddListener(AddTooFarMalus);
        AmbiantManager.I.onTimePhaseChanged.AddListener(ChangeColor);

  //      DocilityScore = GameManager.I._data.InitialDocilityScore;
  //      _DocilityLevel = 1;
  //      _IsIgnoring = false;
  //      _SpontaneityCheckTimer = ComputeRandomSpontaneityCheckTimer();
		//ModeStopAndWait();
        Random.InitState(System.DateTime.Now.Millisecond);
        StartChrono(Random.Range(5, 10), PlayFlip);

        _DefaultEmissionColor = transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.GetColor("_EmissionColor");

        _Player.onZoneEnter.AddListener(PlayerEnterDangerousZone);
        _Player.onZoneExit.AddListener(PlayerExitDangerousZone);

        CurrentAction = StartCoroutine(Live());
	}

	protected override void Update()
	{
		base.Update();
        Random.InitState(System.DateTime.Now.Millisecond);
		//UpdateEnergy();
		//EnergyCritical();

		// à changer quand l’énergie du convoi sera affichée autrement que par du texte
		//UIManager.I.SetTribeEnergy();

        //// Acceleration and deceleration controls of Tribe
        //if (_TribeNavAgent.hasPath)
        //	_TribeNavAgent.acceleration = (_TribeNavAgent.remainingDistance < TribeProperties.MinDistForDeceleration) ? TribeProperties.DecelerationForce : TribeProperties.AccelerationForce;

        //if (_IsIgnoring)
        //{
        //	_IgnoranceTimer -= Time.deltaTime;
        //	if (_IgnoranceTimer <= 0)
        //	{
        //              ChangeEmissive(_DefaultEmissionColor);
        //		_IsIgnoring = false;
        //	}
        //}
        //else
        //{
        //	UpdateDocility();
        //	_SpontaneityCheckTimer -= Time.deltaTime;
        //	if (_SpontaneityCheckTimer <= 0)
        //	{
        //		CheckSpontaneity();
        //	}
        //}

        //UIManager.I.SetTribeDocility(_IsIgnoring);

        //if (IsInMeleeRangeOf())
        //{
        //	RotateTowards();
        //}

        //if (Input.GetKeyDown(KeyCode.M))
        //    GoToPosition(new Vector3(_Player.transform.position.x, transform.position.y, _Player.transform.position.z), WaitAndComeBackSeconds: 2.0f);
        //if (Input.GetKeyDown(KeyCode.L))
        //    FollowPath(AroundIslandPath);
        //if (Input.GetKeyDown(KeyCode.K))
        //    GoToRandomPosition();
        //if (Input.GetKeyDown(KeyCode.J))
        //    DiveTo(_Player.transform.position);
        //if (Input.GetKeyDown(KeyCode.H))
        //    FollowTransform(_Player.transform, 5);
        //if (Input.GetKeyDown(KeyCode.N))
        //    LookTransform(_Player.transform, 5);
        //if (Input.GetKeyDown(KeyCode.B))
        //    GoDownUp(-300, true);
        //if (Input.GetKeyDown(KeyCode.V))
        //    GoDownUp(+300, true);
        if (Input.GetKeyDown(KeyCode.C))
        {
            StopAllCoroutines();
            StartCoroutine(Happy());
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            StopAllCoroutines();
            StartCoroutine(AggressPlayer());
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StopAllCoroutines();
            StartCoroutine(Live());
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StopAllCoroutines();
            StartCoroutine(Fear());
        }
    }

    #region Tribe behaviour
    /* Glaucus Actions
    1. Move from A to B
    2. Follow a path
    3. Turn around the island.
    4. Sleep
    5. Flight quick
    6. Flight very slowly
    7. Drop shit
    8. Attracted by something
    9. Angry
    10. Happy
    11. Sad
    12. Disappear
    13. Stay above the player (turning ?)
    14. falls
    */
    Coroutine CurrentAction = null;
    bool StopCurrentAction = false;
    float speed = 100.0f;
    float AngularSpeed = 0.5f;
    public GameObject AroundIslandPath;
    public float NormalHeight = 400f;
    //[Range(0, 1)]
    //public float Sadness = 1.0f;
    //[Range(0, 1)]
    //public float Fear = 0.0f;
    public List<Zone> DangerousZones;
    bool PauseRandomMove = false;

    // Triggers of complex actions
    void PlayerEnterDangerousZone(ZoneInteractable who, Zone zone)
    {
        Debug.Log("Enter zone");
        if (DangerousZones == null)
            return;

        if (DangerousZones.Exists(z => z == zone))
        {
            StopAllCoroutines();
            CurrentAction = StartCoroutine(AggressPlayer());
        }
    }
    void PlayerExitDangerousZone(ZoneInteractable who, Zone zone)
    {
        Debug.Log("Exit zone");

        if (DangerousZones == null)
            return;

        if (DangerousZones.Exists(z => z == zone))
        {
            StopAllCoroutines();
            CurrentAction = StartCoroutine(Live());
        }
    }
    public Light RedLight;
    // Complex Actions
    public IEnumerator AggressPlayer()
    {
        SetMode(TribeEmotionMode.Aggress);

        List<IEnumerable> cos = new List<IEnumerable>();
        yield return GoToPosition(new Vector3(_Player.transform.position.x-300, transform.position.y, _Player.transform.position.z-300));
        //cos.Add(Diving(_Player.transform.position, 300f));
        //cos.Add(LookingTransform(_Player.transform, 3f));
        cos.Add(RotatingAround(_Player.transform, 10f, 300f));
        while (true)
        {
            while (PauseRandomMove)
                yield return null;

            yield return StartCoroutine(cos[(int)Random.Range(0, cos.Count)].GetEnumerator());
        }
    }
    public IEnumerator Live()
    {
        SetMode(TribeEmotionMode.Normal);

        List<IEnumerable> cos = new List<IEnumerable>();
        cos.Add(GoToRandomPosition(200f));
        cos.Add(FollowingTransform(_Player.transform, 1, 50f));
        cos.Add(Following(AroundIslandPath, speedMove: 200f));
        yield return StartCoroutine(ResetCreature());
        speed = 100f;
        while (true)
        {
            while (PauseRandomMove)
                yield return null;
            yield return StartCoroutine(cos[(int)Random.Range(0, cos.Count)].GetEnumerator());
        }

    }
    public IEnumerator Happy()
    {

        SetMode(TribeEmotionMode.Happy);


        List<IEnumerable> cos = new List<IEnumerable>();
        cos.Add(GoDownUp(1000, speedMove: 300));
        cos.Add(GoToRandomPosition(speedMove: 500));
        cos.Add(FollowingTransform(_Player.transform, 2, speedMove: 50f));
        while (true)
        {
            while (PauseRandomMove)
                yield return null;
            yield return StartCoroutine(cos[(int)Random.Range(0, cos.Count)].GetEnumerator());
        }
    }
    public IEnumerator Fear()
    {
        SetMode(TribeEmotionMode.Fear);


        List<IEnumerable> cos = new List<IEnumerable>();
        cos.Add(FollowingTransform(_Player.transform, 2, speedMove: 200f));
        while (true)
        {
            while (PauseRandomMove)
                yield return null;
            yield return StartCoroutine(cos[(int)Random.Range(0, cos.Count)].GetEnumerator());
        }
    }

    public void SetMode(TribeEmotionMode mode = TribeEmotionMode.Normal)
    {
        switch (mode)
        {
            case TribeEmotionMode.Normal:
                Debug.Log("live");

                SoundManager.I.PlayCreature("Normal");

                //RedLight.gameObject.SetActive(false);
                break;
            case TribeEmotionMode.Aggress:
                Debug.Log("aggress");
                //RedLight.gameObject.SetActive(true);
                //RedLight.color = Color.red;
                SoundManager.I.PlayCreature("Angry2");

                break;
            case TribeEmotionMode.Fear:
                Debug.Log("Fear");
                SoundManager.I.PlayCreature("Angry");

                //RedLight.gameObject.SetActive(true);
                //RedLight.color = Color.blue;
                break;
            case TribeEmotionMode.Happy:
                Debug.Log("Happy");
                SoundManager.I.PlayCreature("Happy");
                //RedLight.gameObject.SetActive(true);
                //RedLight.color = Color.yellow;
                break;
        }
    }

    // Pause current random movement
    public void PauseRandom(bool pause = false, bool StopCoroutines = false)
    {
        PauseRandomMove = pause;
        if (StopCoroutines)
            StopAllCoroutines();
    }
    public void StopAll()
    {
        StopAllCoroutines();
    }
    // Primitive actions
    public IEnumerator ResetCreature()
    {
        yield return StartCoroutine(Rotating(Quaternion.LookRotation(transform.forward, Vector3.up)));
        yield return StartCoroutine(GoDownUp(400 - transform.position.y, ComeBack: false).GetEnumerator());
        speed = 100f;
    }
    public IEnumerable GoDownUp(float delta, bool LookAt = true, float speedMove = 0, bool ComeBack = true)
    {
        yield return StartCoroutine(GoingToPosition(new Vector3(transform.position.x, transform.position.y + delta, transform.position.z), speedMove: speedMove, ChangeRotation: LookAt));
        if (ComeBack)
            yield return StartCoroutine(GoingToPosition(new Vector3(transform.position.x, transform.position.y - delta, transform.position.z), speedMove: speedMove, ChangeRotation: LookAt));
    }

    GameObject lastpos;
    public IEnumerable GoToRandomPosition(float speedMove = 0)
    {
        Debug.Log("Random position");
        speedMove = speedMove == 0 ? speed : speedMove;

        Bounds terrainDimensions = _Terrain.terrainData.bounds;
        Vector2 randomDestination = new Vector2(terrainDimensions.center.x, terrainDimensions.center.z)
                                    + Random.insideUnitCircle * (terrainDimensions.size / 2);

        Vector3 pos = new Vector3(randomDestination.x, transform.position.y, randomDestination.y);

        if (lastpos)
            DestroyImmediate(lastpos);

        lastpos = new GameObject();//Instantiate(new GameObject(), pos, Quaternion.identity);
        lastpos.transform.position = pos;
        lastpos.AddComponent<MeshRenderer>();
        lastpos.AddComponent<MeshFilter>().mesh = new Mesh();
        
        yield return StartCoroutine(GoingToPosition(pos, speedMove: speedMove, dir: (RotationDirection)Random.Range(0, 2)));
    }
    public IEnumerator GoToPosition(Vector3 position, bool Force = false, float WaitAndComeBackSeconds=0f, bool ChangeRotation = true)
    {
        yield return StartCoroutine(GoingToPosition(position, WaitAndComeBackSeconds, true));
    }

    public LayerMask LayermaskCollision;
    // Technical Methods
    public IEnumerable RotatingAround(Transform t, float duration = 0f, float speedMove = 0)
    {
        speedMove = speedMove == 0 ? speed : speedMove;
        RotationDirection dir = (RotationDirection)Random.Range(0, 2);
        yield return StartCoroutine(GoDownUp(1000, true, 300, false).GetEnumerator());
        yield return StartCoroutine(GoingToPosition(new Vector3(t.position.x + 100, t.position.y + 100, t.position.z + 100), speedMove: speedMove, dir: dir));
        float starttime = Time.time;
        while (true)
        {
            if (duration > 0 && Time.time - starttime > duration)
                break;
            transform.RotateAround(t.position, Vector3.up, 10 * Time.deltaTime);
            RotateTowards(t.position, dir);
            yield return null;
        }
    }
    public IEnumerable LookingTransform(Transform target, float duration = 0.0f)
    {
        float StartedTime = Time.time;
        Quaternion InitialRotation = transform.rotation;
        while (true)
        {
            RotateTowards(target.position);
            if (duration > 0 && Time.time - StartedTime > duration)
                break;
            yield return null;
        }
        while (Mathf.Abs(Quaternion.Angle(transform.rotation, InitialRotation)) > 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, InitialRotation, Time.deltaTime * AngularSpeed);
            yield return null;
        }
    }
    public IEnumerable FollowingTransform(Transform target, float duration = 0.0f, float speedMove=0)
    {
        speedMove = speedMove == 0 ? speed : speedMove;

        float StartedTime = Time.time;
        while (true)
        {
            yield return GoingToPosition(new Vector3(target.position.x, transform.position.y, target.position.z), speedMove: speedMove);
            if (duration > 0 && Time.time - StartedTime > duration)
                break;
            yield return null;
        }
    }
    public IEnumerable Diving(Vector3 Target, float speedMove = 0)
    {
        speedMove = speedMove == 0 ? speed : speedMove;
        Vector3 Direction = new Vector3(Target.x - transform.position.x, transform.position.y, Target.z- transform.position.z);
        Vector3 FinalPosition = new Vector3(Target.x, 0, Target.z) + Direction;
        yield return GoingToPosition(new Vector3(Target.x, Target.y + 100f, Target.z), speedMove: speedMove);
        yield return GoingToPosition(FinalPosition, speedMove: speedMove);
    }
    public IEnumerator GoingToPosition(Vector3 position, float WaitAndComeBackSeconds = 0f, bool ChangeRotation = true, float speedMove = 0f, RotationDirection dir = RotationDirection.None)
    {
        speedMove = speedMove == 0 ? speed : speedMove;
        Vector3 initialPosition = transform.position;
        while (Vector3.Distance(transform.position, position) > 250f)
        {
            if (ChangeRotation)
            {
                RaycastHit ray;
                Debug.DrawRay(transform.position, transform.forward * 200, Color.white);
                RotateTowards(position, dir);
                if (Physics.Raycast(transform.position, transform.forward, out ray, 200, LayermaskCollision))
                {
                    transform.position += Vector3.up * speedMove * Time.deltaTime;
                }
             

                transform.position += transform.forward * speedMove * Time.deltaTime;//Vector3.MoveTowards(transform.position, position, speedMove * Time.deltaTime);
            }
            else
            {
                Vector3.MoveTowards(transform.position, position, speedMove * Time.deltaTime);
            }
            yield return null;
        }
        if (Vector3.Distance(transform.position, position) > 250f)
        {
            while (Vector3.Distance(transform.position, position) > 10f)
            {
                Vector3.MoveTowards(transform.position, position, speedMove * Time.deltaTime);
                yield return null;
            }
        }
        if (WaitAndComeBackSeconds > 0)
        {
            yield return new WaitForSeconds(WaitAndComeBackSeconds);
            while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, speedMove * Time.deltaTime);
                yield return null;
                if (ChangeRotation)
                    RotateTowards(initialPosition, dir);
            }
        }
    }
    public void RotateTowards(Vector3 direction, RotationDirection Rdirection = RotationDirection.Left)
    {
        Vector3 Direction = Vector3.zero;

        Direction = (direction - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(Direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * AngularSpeed);
        //float angle = GetSignedAngle(transform.rotation, lookRotation, Vector3.up);
        //if (Mathf.Abs(angle) <= 0.1f)
        //    return;

        //if (Rdirection == RotationDirection.None)
        //{
        //    if (angle < 0)
        //        Rdirection = RotationDirection.Left;
        //    else
        //        Rdirection = RotationDirection.Right;
        //}
        //if (Rdirection == RotationDirection.Left)
        //    transform.Rotate(Vector3.up, angle * Time.deltaTime * -1 * AngularSpeed, Space.Self);
        //else
        //    transform.Rotate(Vector3.up, angle * Time.deltaTime * AngularSpeed, Space.Self);

    }
    public float GetSignedAngle(Quaternion A, Quaternion B, Vector3 axis)
    {
        float angle = 0f;
        Vector3 angleAxis = Vector3.zero;
        (B * Quaternion.Inverse(A)).ToAngleAxis(out angle, out angleAxis);
        if (Vector3.Angle(axis, angleAxis) > 90f)
        {
            angle = -angle;
        }
        return Mathf.DeltaAngle(0f, angle);
    }

    public IEnumerator Rotating(Quaternion rotation)
    {
        while (Mathf.Abs(Quaternion.Angle(transform.rotation, rotation)) > 1.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * AngularSpeed);
            yield return null;
        }

    }
    public IEnumerable Following(GameObject path, int LoopCount = 0, float speedMove= 0)
    {
        speedMove = speedMove == 0 ? speed : speedMove;
        Transform[] Checkpoints = new Transform[path.transform.childCount];
        int i = 0;
        int loops = 0;
        while (i < Checkpoints.Length)
        {
            Checkpoints[i] = path.transform.GetChild(i);
            yield return StartCoroutine(GoingToPosition(new Vector3(Checkpoints[i].position.x, transform.position.y, Checkpoints[i].position.z)));

            if (LoopCount > 0)
            {
                i = (int)Mathf.Repeat(i + 1, Checkpoints.Length);
                if (i == 0)
                {
                    loops++;
                    if (loops > LoopCount)
                        break;
                }
            }
            else
                i++;

        }
    }

    public void StartRotating(Transform t, float duration = 0f, float speedMove = 0)
    {
        StartCoroutine(RotatingAround(t, duration, speedMove).GetEnumerator());
    }
    public void StartLive()
    {
        StartCoroutine(Live());
    }
    public void StartHappy()
    {
        StartCoroutine(Happy());
    }
    public void StartAggress()
    {
        StartCoroutine(AggressPlayer());
    }
    #endregion

    public void ChangeColor(int phaseIndex)
    {
        GameData gd = GameManager.I._data;
        Color emissionColor = phaseIndex == gd.Phases.Count ? gd.SpecialPhase.color : gd.Phases[phaseIndex].color;
        ChangeEmissive(emissionColor);
    }

    #region Tribe Movements
    //void SetNavMeshAgent()
    //{
    //	_TribeNavAgent.acceleration = TribeProperties.AccelerationForce;
    //	_TribeNavAgent.stoppingDistance = TribeProperties.MinDistForAcceleration;
    //	TribeInDefaultSpeed();
    //}
    bool IsInMeleeRangeOf()
	{
		float distance = Vector3.Distance(transform.position, _Player.transform.position);
		return distance < TribeProperties.MinDistForAcceleration;
	}
	void RotateTowards()
	{
		Vector3 direction = (_Player.transform.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * TribeProperties.MaxAngularSpeed);
	}
    void PlayFlip()
    {
        if (Random.Range(0, 2) == 0)
            animator.Play("@loopingSide");
        else
            animator.Play("@rear");

        StartChrono(Random.Range(30, 100), PlayFlip);
    }

	public void SwitchModeFollowAndWait()
	{
        if(!IsIgnoring())
        {
            if (_PlayerMovement.TribeDistance > GameManager.I._data.MaximumDistanceOfTribe)
            {
                return;
            }
            if (_TribeMovementsMode == TribeMovementsMode.FollowPlayer)
                ModeStopAndWait();
            else
                ModeFollowPlayer();
        }
	}

	public void ModeStopAndWait()
	{
        _TribeMovementsMode = TribeMovementsMode.Wait;
        _PlayerMovement.onPlayerHasMoved.RemoveListener(TribeFollowPlayer);
        //_TribeNavAgent.ResetPath();
	}

	public void ModeFollowPlayer()
	{
        _TribeMovementsMode = TribeMovementsMode.FollowPlayer;
        _PlayerMovement.onPlayerHasMoved.AddListener(TribeFollowPlayer);
	}

	void TribeFollowPlayer(Vector3 playerPosition)
	{
        //NavMeshHit hit;
        //Vector3 playerPositionOnNavMesh;
        //if(NavMesh.SamplePosition(playerPosition, out hit, 500, NavMesh.AllAreas))
        //{
        //    playerPositionOnNavMesh = hit.position;
        //    if (_TribeNavAgent.destination != playerPositionOnNavMesh)
        //        _TribeNavAgent.SetDestination(playerPositionOnNavMesh);
        //    _TribeNavAgent.SetDestination(Vector3.Slerp(transform.position, playerPositionOnNavMesh, 5));
        //}
	}

    public void ModeGoToBeacon(Vector3 destination)
	{
        if(!IsIgnoring())
        {
            ModeStopAndWait();
            _TribeMovementsMode = TribeMovementsMode.GoToBeacon;
            //NavMeshHit hit;
            //if(NavMesh.SamplePosition(destination, out hit, 500, NavMesh.AllAreas))
            //{
            //    _TribeNavAgent.SetDestination(hit.position);
            //}
        }
	}

    public void ModeBeSpontaneous(Vector2 destination)
    {
        ModeStopAndWait();
        _TribeMovementsMode = TribeMovementsMode.BeSpontaneous;
        //NavMeshHit hit;
        //if(NavMesh.SamplePosition(new Vector3(destination.x, 0, destination.y), out hit, 500, NavMesh.AllAreas))
        //{
        //    _TribeNavAgent.SetDestination(hit.position);
        //}
    }

    public void UpdateDocility()
    {
        switch(ComputeDocilityLevel())
        {
            case 0:
                _IgnoranceProbability = GameManager.I._data.Level0Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level0Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level0Params.SpontaneityProbability;
                break;
            case 1:
                _IgnoranceProbability = GameManager.I._data.Level1Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level1Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level1Params.SpontaneityProbability;
                break;
            case 2:
                _IgnoranceProbability = GameManager.I._data.Level2Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level2Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level2Params.SpontaneityProbability;
                break;
            case 3:
                _IgnoranceProbability = GameManager.I._data.Level3Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level3Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level3Params.SpontaneityProbability;
                break;
            case 4:
                _IgnoranceProbability = GameManager.I._data.Level4Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level4Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level4Params.SpontaneityProbability;
                break;
        }
    }

    public void AddItemActivationBonus(Item item)
    {
        DocilityScore += GameManager.I._data.ItemActivationBonus;
    }

    public void AddNewDayBonus(DayStatesProperties currentDayState, DayStatesProperties nextDayState)
    {
        if(currentDayState.State == DayState.Day)
        {
            DocilityScore += GameManager.I._data.NewDayBonus;
        }
    }

    public void AddTooFarMalus()
    {
        DocilityScore -= GameManager.I._data.TooFarFromTribeMalus;
    }

    // public void AddObstacleDocilityMalus() {}

    public int ComputeDocilityLevel()
    {
        int bonusMalus = 0;
        bonusMalus -= GameManager.I._data.InitialTribeEnergy - Mathf.RoundToInt(Energy);
        if(IsEnergyCritical)
        {
            bonusMalus -= GameManager.I._data.CriticalEnergyMalus;
        }
        if(AmbiantManager.I.IsNight)
        {
            bonusMalus += GameManager.I._data.NightBonus;
        }

        int finalScore = DocilityScore + bonusMalus;
        if(finalScore <         0) { _DocilityLevel = 0; }
        else if(finalScore <  100) { _DocilityLevel = 1; }
        else if(finalScore <  200) { _DocilityLevel = 2; }
        else if(finalScore <  300) { _DocilityLevel = 3; }
        else if(finalScore >= 300) { _DocilityLevel = 4; }
        return _DocilityLevel;
    }

    public bool IsIgnoring()
    {
        if(!_IsIgnoring && Random.Range(0, 1f) < _IgnoranceProbability)
        {
            ChangeEmissive(IgnoringEmissionColor);
            _IsIgnoring = true;
            _IgnoranceTimer = _IgnoranceDuration;
        }
        return _IsIgnoring;
    }

    public bool CheckSpontaneity()
    {
        if(Random.Range(0, 1f) < _SpontaneityProbability)
        {
            _SpontaneityCheckTimer = ComputeRandomSpontaneityCheckTimer();
            Bounds terrainDimensions = _Terrain.terrainData.bounds;
            Vector2 randomDestination = new Vector2(terrainDimensions.center.x, terrainDimensions.center.z)
                    + Random.insideUnitCircle * (terrainDimensions.size / 2);
            ModeBeSpontaneous(randomDestination);
            ChangeEmissive(IgnoringEmissionColor);
            _IsIgnoring = true;
            _IgnoranceTimer = _IgnoranceDuration;
            return true;
        }
        return false;
    }

    public int ComputeRandomSpontaneityCheckTimer()
    {
        return Random.Range(GameManager.I._data.SpontaneityCheckTimerMinDuration, GameManager.I._data.SpontaneityCheckTimerMaxDuration);
    }

    public void ChangeEmissive(Color emissionColor)
    {
        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("_EmissionColor", emissionColor);
    }

    #endregion

	#region Tribe speed

	void TribeInDefaultSpeed()
	{
		//_TribeNavAgent.speed = GameManager.I._data.TribeProperties.MaxSpeed;
		//_TribeNavAgent.angularSpeed = GameManager.I._data.TribeProperties.MaxAngularSpeed;

	}

	void TribeInCriticalSpeed()
	{
		//_TribeNavAgent.speed = GameManager.I._data.TribeProperties.MaxSpeed * GameManager.I._data.TribeProperties.CriticalSpeedMultiplicator;
	}

	#endregion

    #region Tribe energy
    bool EventCalled = false;
	public void UpdateEnergy()
	{
		// Lost energy
		if (!InZones.Exists(z => z.GainEnergySpeed > 0))
        {
            if(_TribeMovementsMode == TribeMovementsMode.Wait)
            {
                Energy -= GameManager.I._data.TribeEnergyLossWaiting * Time.deltaTime;
            }
            else
            {
			    Energy -= GameManager.I._data.TribeEnergyLossMoving * Time.deltaTime;
            }
        }

        // Clamp if life is out of range
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialTribeEnergy);
        if (Energy == GameManager.I._data.InitialTribeEnergy && !EventCalled)
        {
            EventCalled = true;
            onEnergyFull.Invoke();
        }
        else if (Energy != GameManager.I._data.InitialTribeEnergy)
            EventCalled = false;

        if(IsEnergyCritical)
        {
            animator.SetFloat("Blend", 1);
        }
        else
        {
            animator.SetFloat("Blend", 1 - (Energy / GameManager.I._data.InitialTribeEnergy * 2));
        }
    }

    void EnergyCritical()
	{
		if (IsEnergyCritical && !PreviousIsEnergyCritical)
		{
            PreviousIsEnergyCritical = true;
			onTribeEnergyEnterCritical.Invoke();
		}
		else if (!IsEnergyCritical && PreviousIsEnergyCritical)
		{
            PreviousIsEnergyCritical = false;
			onTribeEnergyExitCritical.Invoke();
		}
	}

    #endregion

    #region Tribe in zone

    public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);
        GainTribeEnergy(zone);
        LooseEnergy(zone);
    }

    public void GainTribeEnergy(Zone zone)
	{
		Energy += zone.GainEnergySpeed * Time.deltaTime;
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialTribeEnergy);
    }

    public void LooseEnergy(Zone zone)
    {
        Energy += zone.LooseEnergySpeed * Time.deltaTime;
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialTribeEnergy);
    }

    #endregion
}
