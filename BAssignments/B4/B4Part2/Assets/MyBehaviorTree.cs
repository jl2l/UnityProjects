using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class MyBehaviorTree : MonoBehaviour
{

	public Transform DoorOpenPoint;
	public GameObject[] friends;
	public GameObject participant;
	public GameObject zombie;
	public GameObject zombie1;
	public GameObject door;
	public GameObject camera;
	public GameObject[] music;
	public Text AnnounceText;
	public Text FirstZombie;
	public Text SecondZombie;
	public Text PlayerText;
	public Val<bool> DoorIsOpen;

	public Val<GameObject> player;
	public Val<GameObject> CameraOnPlayer;
	public Val<int> playerNum;
	public Val<int> MusicInt;
	public Val<int> SongNumber;
	public Val<int> MusicIntValue;

	public Val<int> cameraOn;
	public Val<bool> UserControl;
	// 1 - on player
	// 2 - on zombie 1
	// 3 - on zombie 2

	private Vector3 offset;

	//private AudioSource audioS;
	//private AudioClip Song;

	private BehaviorAgent behaviorAgent;
	public Vector3 ZombieInitialPosition;
	private Vector3 Zombie1InitialPosition;
	private Vector3[] spawnPos = new Vector3[3];

	// Use this for initialization
	void Start ()
	{

		MusicIntValue = 0;
		spawnPos [0] = zombie1.transform.position;
		spawnPos [1] = zombie1.transform.position + new Vector3 (40, 0, 0);
		spawnPos [2] = zombie1.transform.position +  new Vector3 (70, 0, 0);
		int spawnPoint = 0;

		spawnPoint = Random.Range(0, 3);
		Zombie1InitialPosition = new Vector3(spawnPos[spawnPoint].x,spawnPos[spawnPoint].y,spawnPos[spawnPoint].z);
		print (spawnPoint + "\n");
		zombie1.GetComponent<SteeringController> ().Warp (Zombie1InitialPosition);

		spawnPos [0] = zombie.transform.position;
		spawnPos [1] = zombie.transform.position + new Vector3 (60, 0, -180);
		spawnPos [2] = zombie.transform.position +  new Vector3 (90, 0, 0);
		spawnPoint = 0;
		
		spawnPoint = Random.Range(0, 3);
		ZombieInitialPosition = new Vector3(spawnPos[spawnPoint].x,spawnPos[spawnPoint].y,spawnPos[spawnPoint].z);
		print (spawnPoint + "\n");
		zombie.GetComponent<SteeringController> ().Warp (ZombieInitialPosition);

		//ZombieInitialPosition = zombie.transform.position;
		//Zombie1InitialPosition = zombie1.transform.position;
		offset = participant.transform.position - camera.transform.position;
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}

	// Update is called once per frame
	void Update ()
	{


	}
/*
	protected Node ST_ApproachAndWait(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
	}
*/

	protected Node BuildDoorRoot()
	{
		return
			new SequenceParallel (AssertAndWaitForClap(friends[0]),
			                      AssertAndWaitForClap(friends[1]),
			                      AssertAndWaitForClap(friends[2]),
			                      AssertAndWaitForClap(friends[3]),
			                      AssertAndWaitForClap(friends[4]));
	}

	protected Node AssertAndWaitForClap(GameObject CurrentPerson)
	{
		return new DecoratorLoop(
			new Sequence(
			new DecoratorInvert(
			new DecoratorLoop ((new DecoratorInvert (
			new Sequence(this.WaitForClap (CurrentPerson)))))),
			new LeafWait(1000),OpenDoor()));
	}

	protected Node WaitForClap(GameObject CurrentPerrson)
	{
		return new LeafAssert (() => CurrentPerrson.GetComponent<Animator>().GetBool("H_Clap"));
	}
/*
	protected Node CheckForWin()
	{
		Val<Vector3> CurrPosition = Val.V (() => participant.transform.position);
		Val<Vector3> BoxPosition = Val.V (() => Winwander.transform.position);
		Val<Vector3> DiffrencePosition = Val.V (() => CurrPosition.Value - BoxPosition.Value);
		Val<float> DistanceAway = Val.V (() => DiffrencePosition.Value.sqrMagnitude);


		return new LeafAssert (() => (DistanceAway.Value < 50));
	}

	protected Node AssertCheckForWin()
	{
		return new DecoratorLoop(
			new Sequence(
			new DecoratorInvert(
			new DecoratorLoop ((new DecoratorInvert (
			new Sequence( this.CheckForWin ()))))),
			new LeafInvoke(() =>UserWins())));
	}
*/
	void UserWins()
	{
		AnnounceText.text = "The User Won";
		Time.timeScale = 0;
		AnnounceText.enabled = true;
		Application.Quit();
	}

	protected Node BuildTreeRoot()
	{
		DoorIsOpen = Val.V (() => false);
		player = Val.V (() => friends[0]);
		CameraOnPlayer = Val.V (() => friends[0]);
		cameraOn = Val.V (() => 1);
		UserControl = Val.V (() => false);
		playerNum = Val.V (() => 0);
		MusicInt = Val.V (() => 0);
		MusicIntValue = Val.V (() => 0);


		return
			new DecoratorLoop (
				new SequenceParallel (
				this.BuildMainTreeRoot (friends[0],0),
				this.BuildMainTreeRoot (friends[1],1),
				this.BuildMainTreeRoot (friends[2],2),
				this.BuildMainTreeRoot (friends[3],3),
				this.BuildMainTreeRoot (friends[4],4),
				this.AssertNextToPowerUp(friends[4],4),
				this.ZombieAssertAndGetHurt(zombie,1),
				this.ZombieAssertAndGetHurt(zombie1,2),
				//this.AssertUserClickedA(),
				//this.AssertUserClickedD(),
				this.BuildZombieTreeRoot(),
				this.BuildZombie1TreeRoot(),
				this.BuildDoorRoot(),
				this.UserControlsPlayer()
				));
	}

	protected Node BuildMainTreeRoot(GameObject CurrentPerson, int PlayerNumber)
	{
		return 
			new SequenceParallel (AssertAndGetScared (CurrentPerson, PlayerNumber),
			                      AssertAndGetHurt(CurrentPerson, PlayerNumber),
			                      AssertCheckNextToDoor(CurrentPerson, PlayerNumber),
			                      AssertCheckNextToCloseDoorPoint(CurrentPerson, PlayerNumber),
			                     ParticipantOpenDoor(CurrentPerson, PlayerNumber));
	}
	protected Node AssertCheckNextToCloseDoorPoint(GameObject CurrentPerson, int PlayerNumber)
	{

		return new DecoratorLoop(
			new Sequence(
			new DecoratorInvert(
			new DecoratorLoop ((new DecoratorInvert (
			new Sequence( this.CheckNextToCloseDoorPoint(CurrentPerson, PlayerNumber)))))),
			CloseDoor()));

	}

	protected Node CheckNextToCloseDoorPoint(GameObject CurrentPerson, int PlayerNumber)
	{
		Val<Vector3> CurrPosition = Val.V (() => CurrentPerson.transform.position);
		Val<Vector3> DoorPosition = Val.V (() => door.transform.position);
		Val<Vector3> DiffrencePosition = Val.V (() => CurrPosition.Value - DoorPosition.Value);
		Val<float> DistanceAway = Val.V (() => DiffrencePosition.Value.sqrMagnitude);

		return new LeafAssert (() => (DistanceAway.Value > 200 && DoorIsOpen.Value == true && PlayerNumber == playerNum.Value));
	}


	protected Node CheckNextToDoor(GameObject CurrentPerson, int PlayerNumber)
	{

		Val<Vector3> CurrPosition = Val.V (() => CurrentPerson.transform.position);
		Val<Vector3> DoorPosition = Val.V (() => door.transform.position);
		Val<Vector3> DiffrencePosition = Val.V (() => CurrPosition.Value - DoorPosition.Value);
		Val<float> DistanceAway = Val.V (() => DiffrencePosition.Value.sqrMagnitude);
		return new LeafAssert (() => (DistanceAway.Value < 30 && DoorIsOpen.Value == false && PlayerNumber == playerNum.Value));
	}
	
	protected Node AssertCheckNextToDoor(GameObject CurrentPerson, int PlayerNumber)
	{

		return new DecoratorLoop(
			new Sequence(
			new DecoratorInvert(
			new DecoratorLoop ((new DecoratorInvert (
			new Sequence( this.CheckNextToDoor(CurrentPerson, PlayerNumber)))))),
			PlayerOpensDoor(CurrentPerson)));
	}

	protected Node PlayerOpensDoor(GameObject CurrentPerson)
	{


		return new Sequence (
						 FacePosition (CurrentPerson, door.transform), 
			             CurrentPerson.GetComponent<BehaviorMecanim> ().ST_PlayHandGesture ("CLAP", 1000),
		                 new LeafWait (1000));


	}

	protected Node AssertNextToPowerUp(GameObject CurrentPerson, int PlayerNumber)
	{
		
		return new DecoratorLoop(
			new Sequence(
			new DecoratorInvert(
			new DecoratorLoop ((new DecoratorInvert (
			new Sequence( this.CheckNextToPowerUp(CurrentPerson, PlayerNumber)))))),
			new LeafInvoke (() => StartMusic()),
			new LeafWait(45000),
			new LeafInvoke (() => EndMusic())));
	}
	void StartMusic()
	{
		music[MusicIntValue.Value].GetComponent<Basketballsong> ().PlaySong = 1;

		player.Value.GetComponent<UnitySteeringController> ().acceleration = 10f;
		player.Value.GetComponent<UnitySteeringController> ().maxSpeed = 28f;
		player.Value.GetComponent<UnitySteeringController> ().SlowArrival = false;
		player.Value.GetComponent<UnitySteeringController> ().stoppingRadius = 2f;


	}
	void EndMusic()
	{
		music[MusicIntValue.Value].GetComponent<Basketballsong> ().PlaySong = 0;
		int IncreaseMusicIntValue = MusicIntValue.Value + 1;
		MusicInt = Val.V (() => 0);
		MusicIntValue = Val.V (() => IncreaseMusicIntValue);
		player.Value.GetComponent<UnitySteeringController> ().acceleration = 2f;
		player.Value.GetComponent<UnitySteeringController> ().maxSpeed = 3.5f;
		player.Value.GetComponent<UnitySteeringController> ().SlowArrival = true;
		player.Value.GetComponent<UnitySteeringController> ().stoppingRadius = 0.4f;

	}
	protected Node CheckNextToPowerUp(GameObject CurrentPerson, int PlayerNumber)
	{
		return new LeafAssert (() => MusicInt.Value > 0);
	}
	protected Node ParticipantOpenDoor(GameObject CurrentPerson, int PlayerNumber)
	{
		return
			new DecoratorLoop(
			new Sequence (new LeafInvoke (() => HandlePlayer(CurrentPerson,PlayerNumber))));

	}

	void HandlePlayer(GameObject CurrentPerson, int PlayerNumber)
	{

		GameObject collectObject;
		GameObject PowerObject;
		Vector3 Zombiediff;
		Vector3 Zombie1diff;
		Vector3 Playerdiff;
		Vector3 PowerDiff;
		int randomnumber;
		collectObject = NearestObjectByTag("Collectable",CurrentPerson.transform.position);
		if(collectObject == null)
		{
			UserWins();
			
		}
		Vector3 Positiondiff = collectObject.transform.position - CurrentPerson.transform.position;
		float DistanceAway = Positiondiff.sqrMagnitude;

		if (PlayerNumber != playerNum.Value)
		{

			if(CurrentPerson.GetComponent<SteeringController> ().IsAtTarget() || CurrentPerson.GetComponent<SteeringController> ().IsStopped())
			{
			
				CurrentPerson.GetComponent<SteeringController> ().Target  = new Vector3(Random.Range(248f,259f),0.1f,Random.Range(1142f,1169f));
			}
			return;

		}

		InteractionSystem playerIS = player.Value.GetComponent<InteractionSystem>();
		PowerObject =  NearestObjectByTag("PowerUp",CurrentPerson.transform.position);

		if(PowerObject != null)
		{
			
			PowerDiff = PowerObject.transform.position - CurrentPerson.transform.position;
			if(PowerDiff.sqrMagnitude < 3 && MusicInt.Value == 0)
			{
				
				playerIS.StartInteraction(FullBodyBipedEffector.RightHand, PowerObject.GetComponent<InteractionObject>(), false);
				
				MusicInt =  Val.V (() => 1);
				
				PowerObject.SetActive(false);
			}
		}

		if (cameraOn.Value == 1)
		{
			Zombiediff = zombie.transform.position - player.Value.transform.position;
			Zombie1diff = zombie1.transform.position - player.Value.transform.position;
			Playerdiff = new Vector3(0,0,0);
			FirstZombie.text = "First Zombie: " + (int)Zombiediff.sqrMagnitude;
			SecondZombie.text = "Second Zombie: " + (int)Zombie1diff.sqrMagnitude;
			PlayerText.text = "Player: " + (int)Playerdiff.sqrMagnitude;
			
			FirstZombie.enabled = true;
			SecondZombie.enabled = true;
			PlayerText.enabled = true;



		}
		if (cameraOn.Value == 2)
		{

			Zombiediff = new Vector3(0,0,0);
			Zombie1diff = zombie1.transform.position - zombie.transform.position;
			Playerdiff = player.Value.transform.position - zombie.transform.position;
			FirstZombie.text = "First Zombie: " + (int)Zombiediff.sqrMagnitude;
			SecondZombie.text = "Second Zombie: " + (int)Zombie1diff.sqrMagnitude;
			PlayerText.text = "Player: " + (int)Playerdiff.sqrMagnitude;
			
			FirstZombie.enabled = true;
			SecondZombie.enabled = true;
			PlayerText.enabled = true;
			
			
			
		}
		if (cameraOn.Value == 3)
		{
			Zombiediff = zombie.transform.position - zombie1.transform.position;  
			Zombie1diff = new Vector3(0,0,0);
			Playerdiff = player.Value.transform.position - zombie1.transform.position;
			FirstZombie.text = "First Zombie: " + (int)Zombiediff.sqrMagnitude;
			SecondZombie.text = "Second Zombie: " + (int)Zombie1diff.sqrMagnitude;
			PlayerText.text = "Player: " + (int)Playerdiff.sqrMagnitude;
			
			FirstZombie.enabled = true;
			SecondZombie.enabled = true;
			PlayerText.enabled = true;
			
			
			
		}
		if (cameraOn.Value == 1 && UserControl.Value == true)
		{


			if(DistanceAway<4)
			{

				collectObject.SetActive(false);
			}

			return;
			
		}

		if (PlayerNumber == playerNum.Value)
		{




			if (CurrentPerson.GetComponent<CharacterMecanim> ().Body.NavCanReach (collectObject.transform.position) == false) 
			{
				CurrentPerson.GetComponent<SteeringController> ().Target = DoorOpenPoint.position;
				
			}
			else
			{

				if(DistanceAway<3)
				{

					collectObject.SetActive(false);
				}
				else
				{
				CurrentPerson.GetComponent<SteeringController> ().Target = collectObject.transform.position;
				}


			}


		} 
		else
		{
			return;
		}

	}

	protected Node OpenDoor()
	{

			Vector3 doorPosition = door.transform.position;
			doorPosition.y = doorPosition.y + 5;
			return new Sequence (new LeafInvoke (() => StartCoroutine (MoveObject (door.transform,
																			doorPosition,
		                                                                       4f))));

		                    
	}

	protected Node CloseDoor()
	{
		Vector3 doorPosition = door.transform.position;
		//doorPosition.y = doorPosition.y;
		return new Sequence (new LeafInvoke (() =>StartCoroutine (MoveObject (door.transform,
		                                                                      doorPosition,
		                                                                       4f))));


	}

	IEnumerator MoveObject(Transform referenceObj, Vector3 target, float overTime)
	{
		if (target.y > door.transform.position.y) 
		{
			DoorIsOpen = Val.V (() => true);
		} 
		else
		{
			DoorIsOpen = Val.V (() => false);
		}

		Vector3 source = referenceObj.position;
		float startTime = Time.time;
		while(Time.time < startTime + overTime)
		{
			referenceObj.position = Vector3.Lerp(source, target, (Time.time - startTime)/overTime);
			yield return null;
		}
		referenceObj.position = target;


	}

	protected Node UserControlsPlayer()
	{
		return new DecoratorLoop (new Sequence (AssertUserClicked()));
	}


	void PointOnMap()
	{
		Val<Ray> ray = Val.V (() => Camera.main.ScreenPointToRay (Input.mousePosition));
		RaycastHit hit;
		Physics.Raycast (ray.Value, out hit, 100);
		Val.V (() => Physics.Raycast (ray.Value, out hit, 100));





		if (cameraOn.Value == 1) 
		{
			if (player.Value.GetComponent<CharacterMecanim> ().Body.NavCanReach ((Val.V (() => hit.point)).Value) == false)
			{
				return;
				
			}
			UserControl = Val.V (() => true);
			player.Value.GetComponent<SteeringController> ().Target = (Val.V (() => hit.point)).Value;
		} 
		else if (cameraOn.Value == 2) 
		{
			if (zombie.GetComponent<CharacterMecanim> ().Body.NavCanReach ((Val.V (() => hit.point)).Value) == false)
			{
				return;
				
			}
			UserControl = Val.V (() => true);
			zombie.GetComponent<SteeringController> ().Target = (Val.V (() => hit.point)).Value;
		} 
		else 
		{
			if (zombie1.GetComponent<CharacterMecanim> ().Body.NavCanReach ((Val.V (() => hit.point)).Value) == false)
			{
				return;
				
			}
			UserControl = Val.V (() => true);
			zombie1.GetComponent<SteeringController> ().Target = (Val.V (() => hit.point)).Value;
		}


	}

	protected Node CheckClickInMap()
	{
		Val<Ray> ray = Val.V (() => Camera.main.ScreenPointToRay (Input.mousePosition));
		RaycastHit hit;
		return new LeafAssert (() => Physics.Raycast (ray.Value, out hit, 100));
	}

	protected Node AssertClickInMap()
	{
		return
			new Sequence(new DecoratorInvert(new DecoratorLoop(new DecoratorInvert(new Sequence(this.CheckClickInMap())))),
						 new LeafInvoke(() => PointOnMap ())
						 );
	}

	protected Node AssertUserClicked()
	{
		return new DecoratorLoop(new Sequence(new DecoratorInvert(new DecoratorLoop (new DecoratorInvert(new Sequence(this.CheckClicked ())))),
											  AssertClickInMap()));
	}

	protected Node CheckClicked()
	{
		return new LeafAssert (() => Input.GetButton("Fire1"));
	}

	protected Node AssertAndGetScared(GameObject CurrentPerson, int PlayerNumber)
	{
		return new DecoratorLoop(new Sequence(new DecoratorInvert(new DecoratorLoop ((new DecoratorInvert(new Sequence(this.CheckScared(CurrentPerson,PlayerNumber)))))),
											  CurrentPerson.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("SURPRISED",1000)));
	}

	protected Node ZombieAssertAndGetHurt(GameObject ZombiePerson, int ZombieNum)
	{
		return new DecoratorLoop(new Sequence(new DecoratorInvert(new DecoratorLoop ((new DecoratorInvert(new Sequence(this.ZombieCheckHurt(ZombiePerson)))))),
		                                      new LeafInvoke(() =>HandleZombieHit(ZombiePerson, ZombieNum))));

	}
	void HandleZombieHit(GameObject ZombiePerson,int ZombieNum)
	{
		if (ZombieNum == 1) {
			ZombiePerson.GetComponent<SteeringController> ().Warp (ZombieInitialPosition);
		} else {
			ZombiePerson.GetComponent<SteeringController> ().Warp (Zombie1InitialPosition);
		}
	}

	protected Node ZombieCheckHurt(GameObject CurrentPerson)
	{
		Val<Vector3> CurrPosition = Val.V (() => player.Value.transform.position);
		Val<Vector3> ZombiePosition = Val.V (() => CurrentPerson.transform.position);
		Val<Vector3> DiffrencePosition = Val.V (() => CurrPosition.Value - ZombiePosition.Value);
		Val<float> DistanceAway = Val.V (() => DiffrencePosition.Value.sqrMagnitude);
		
		return new LeafAssert (() => DistanceAway.Value < 5 && MusicInt.Value > 0 );
	}



	protected Node AssertAndGetHurt(GameObject CurrentPerson, int PlayerNumber)
	{
		return new DecoratorLoop(new Sequence(new DecoratorInvert(new DecoratorLoop ((new DecoratorInvert(new Sequence(this.CheckHurt(CurrentPerson,PlayerNumber)))))),
											  CurrentPerson.GetComponent<BehaviorMecanim> ().ST_PlayBodyGesture ("DYING",1000),
											  new LeafInvoke(() =>UserLoses(PlayerNumber))));
	}

	void UserLoses(int PlayerNumber)
	{

		HandleZombieHit (zombie, 1);
		HandleZombieHit (zombie1, 2);

		int num = 0;
		if (PlayerNumber == 4)
		{
			AnnounceText.text = "The Zombies Win";
			AnnounceText.enabled = true;
			Time.timeScale = 0;
		} 
		else if(PlayerNumber == playerNum.Value)
		{
			num = playerNum.Value;
			playerNum = Val.V (() => num + 1);
			player = Val.V (() => friends [playerNum.Value]);
			if(cameraOn.Value == 1)
			{
			UserControl = Val.V (() => false);
			CameraOnPlayer = Val.V (() => player.Value);
			}
		}
		//Application.Quit();
	}

	protected Node BuildZombieTreeRoot()
	{

		return
			new DecoratorLoop (
				new Sequence (
			              	  new LeafInvoke (() => this.ApproachHuman (1))));
	}

	protected Node BuildZombie1TreeRoot()
	{

		return
			new DecoratorLoop (
				new Sequence (
			              new LeafInvoke (() => this.ApproachHuman (2))));
	}


	protected Node FacePosition(GameObject PersonMoving, Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence (PersonMoving.GetComponent<BehaviorMecanim> ().Node_OrientTowards(position));
	}

	void ApproachHuman(int ZombieNum)
	{

		if (ZombieNum == 1)
		{
			if (cameraOn.Value == 2 && UserControl.Value == true)
			{
				return;

			}



			if (zombie.GetComponent<CharacterMecanim> ().Body.NavCanReach (player.Value.transform.position) == false || MusicInt.Value > 0) 
			{
				zombie.GetComponent<SteeringController> ().Target = ZombieInitialPosition;
			
			} 
			else 
			{
				zombie.GetComponent<SteeringController> ().Target = player.Value.transform.position;
			}
		} 
		else if (ZombieNum == 2)
		{
			if (cameraOn.Value == 3 && UserControl.Value == true)
			{
				return;
					
			}
				
				
				
			if (zombie1.GetComponent<CharacterMecanim> ().Body.NavCanReach (player.Value.transform.position) == false || MusicInt.Value > 0)
			{
				zombie1.GetComponent<SteeringController> ().Target = Zombie1InitialPosition;
					
			} else
			{
				zombie1.GetComponent<SteeringController> ().Target = player.Value.transform.position;
			}
		}

	}

	GameObject NearestObjectByTag(string tag, Vector3 CurrentPosition)
	{
		GameObject[] AllObjects;
		AllObjects = GameObject.FindGameObjectsWithTag(tag);
		float smallestDistance = float.PositiveInfinity;
		GameObject smallestDistanceObj = null;
		foreach (GameObject SingleObject in AllObjects) {
			Vector3 Positiondiff = SingleObject.transform.position - CurrentPosition;
			float DistanceAway = Positiondiff.sqrMagnitude;
			if (DistanceAway < smallestDistance) {
				smallestDistance = DistanceAway;
				smallestDistanceObj = SingleObject;
			}
		}

		return smallestDistanceObj;
	}

	protected Node CheckHurt(GameObject CurrentPerson, int PlayerNumber)
	{
		Val<Vector3> CurrPosition = Val.V (() => CurrentPerson.transform.position);
		Val<Vector3> ZombiePosition = Val.V (() => zombie.transform.position);
		Val<Vector3> DiffrencePosition = Val.V (() => CurrPosition.Value - ZombiePosition.Value);
		Val<float> DistanceAway = Val.V (() => DiffrencePosition.Value.sqrMagnitude);

		Val<Vector3> Zombie1Position = Val.V (() => zombie1.transform.position);
		Val<Vector3> Diffrence1Position = Val.V (() => CurrPosition.Value - Zombie1Position.Value);
		Val<float> Distance1Away = Val.V (() => Diffrence1Position.Value.sqrMagnitude);

		return new LeafAssert (() => (DistanceAway.Value < 5 || Distance1Away.Value < 5) && PlayerNumber == playerNum.Value && MusicInt.Value == 0 );
	}


	protected Node CheckScared(GameObject CurrentPerson, int PlayerNumber)
	{
		Val<Vector3> CurrPosition = Val.V (() => CurrentPerson.transform.position);
		Val<Vector3> ZombiePosition = Val.V (() => zombie.transform.position);
		Val<Vector3> DiffrencePosition = Val.V (() => CurrPosition.Value - ZombiePosition.Value);
		Val<float> DistanceAway = Val.V (() => DiffrencePosition.Value.sqrMagnitude);

		Val<Vector3> Zombie1Position = Val.V (() => zombie1.transform.position);
		Val<Vector3> Diffrence1Position = Val.V (() => CurrPosition.Value - Zombie1Position.Value);
		Val<float> Distance1Away = Val.V (() => Diffrence1Position.Value.sqrMagnitude);


		return new LeafAssert (() => ((DistanceAway.Value < 100 && DistanceAway.Value > 50 || Distance1Away.Value < 100 && Distance1Away.Value > 50 ) && PlayerNumber == playerNum.Value && MusicInt.Value == 0 ));
	}


	protected Node AssertUserClickedA()
	{
		return new DecoratorLoop(new Sequence(new DecoratorInvert(new DecoratorLoop (new DecoratorInvert(new Sequence(this.CheckClickedA ())))),
		                                      new LeafInvoke (() => SwitchForward()),
		                                      new LeafWait(1000)));
	}
	
	protected Node CheckClickedA()
	{
		return new LeafAssert (() => Input.GetKey(KeyCode.A));
	}

	void SwitchForward()
	{
		UserControl = Val.V (() => false);
		if (cameraOn.Value == 1)
		{
			CameraOnPlayer = Val.V (() => zombie);
			cameraOn = Val.V (() => 2);
		} 
		else if (cameraOn.Value == 2)
		{
			CameraOnPlayer = Val.V (() => zombie1);
			cameraOn = Val.V (() => 3);
		} 
		else 
		{	CameraOnPlayer = Val.V (() => player.Value);
			cameraOn = Val.V (() => 1);
		}

	}

	void SwitchBackward()
	{
		UserControl = Val.V (() => false);
		if (cameraOn.Value == 1)
		{
			CameraOnPlayer = Val.V (() => zombie1);
			cameraOn = Val.V (() => 3);
		} 
		else if (cameraOn.Value == 3)
		{
			CameraOnPlayer = Val.V (() => zombie);
			cameraOn = Val.V (() => 2);
		} 
		else 
		{	CameraOnPlayer = Val.V (() => player.Value);
			cameraOn = Val.V (() => 1);
		}



	}

	protected Node AssertUserClickedD()
	{
		return new DecoratorLoop(new Sequence(new DecoratorInvert(new DecoratorLoop (new DecoratorInvert(new Sequence(this.CheckClickedD ())))),
		                                      new LeafInvoke (() => SwitchBackward()),
		                                      new LeafWait(1000)));
		                                      
	}
	
	protected Node CheckClickedD()
	{
		return new LeafAssert (() => Input.GetKey(KeyCode.D));
	}


	void LateUpdate()
	{

		Vector3 newPosition;
		float angle = CameraOnPlayer.Value.transform.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, angle, 0);
		newPosition = CameraOnPlayer.Value.transform.position - (rotation * offset);
		//transform.LookAt (player.transform);
		StartCoroutine (TransitionCamera (newPosition));
		
	}

		
	IEnumerator TransitionCamera(Vector3 EndPosition)
	{

		float TransitionTime = 1f;
		float t = 0.0f;
		Vector3 StartingPos = camera.transform.position;
		while (t < 1.0f)
		{
			t += Time.deltaTime * (Time.timeScale/TransitionTime);
			
			
			camera.transform.position = Vector3.Lerp(StartingPos, EndPosition, t);
			camera.transform.LookAt (CameraOnPlayer.Value.transform);
			yield return 0;
		}
		
	}
}
