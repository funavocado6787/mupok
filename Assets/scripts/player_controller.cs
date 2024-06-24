using UnityEngine;
using System.Collections;

public class player_controller : MonoBehaviour {

	public float run_speed;
	public float jump_power;
	public float airjump_coef;  //not yet implemented
	public float ground_friction;
	public float air_friction;
	public LayerMask ignoremask;
	public float gravity;
	public KeyCode jump_key;


	public bool debug;

	Vector2 movement;
	int state = 1;
	bool jump;
	bool grounded;
	RaycastHit2D groundtest;
	Vector2 ground_normal;


	
	void Update() {

		//Grab input inside the update function

		if (Input.GetKeyDown(jump_key)) {
			jump = true;
		}

	}



	void FixedUpdate () {

		//Changing player state
		if (grounded) {
			state = 0;
		}
		else state = 1;

		//Set the movement to actual velocity
		movement = rigidbody2D.velocity;


		// Apply changes to movement depending on player state
	
		switch (state) {
		case 0:
			//Vector2.right aligned to ground surface
			Vector2 ground_right = Quaternion.Euler(0, 0, 90) * ground_normal;

			if (debug) {
			Debug.DrawLine(transform.position + Vector3.down*1.25f, transform.position + Vector3.down*1.25f + Vector3.ClampMagnitude(new Vector3(movement.x, movement.y, 0)/10,3));
			}

			//Applying run force
			if (Input.GetAxis("Horizontal") != 0 && ground_normal.y > 0.75f) {
				movement = Vector2.MoveTowards(movement, Input.GetAxis("Horizontal") * run_speed * -ground_right, run_speed * ground_friction/50);
			}
			else if (ground_normal.y > 0.75f) {
				movement = Vector2.Lerp(movement, Vector2.zero, ground_friction/50);
			}

			//Applying gravity and jump forces if necessary
			if (ground_normal.y < 0.75f)  {
				movement.y -= gravity*1.5f;
			}
			if (jump) {
				if (debug) {
					Debug.Log (Mathf.Clamp (movement.magnitude/30,1,3));
				}
				movement += Mathf.Clamp (movement.magnitude/30,1,2.1f)*jump_power*Vector2.Lerp(Vector2.up, ground_normal, 0.5f);
				jump = false;
			}

			break;
		case 1:
			if (debug) {
				Debug.DrawLine(transform.position + Vector3.down*1.25f, transform.position + Vector3.down*1.25f + Vector3.ClampMagnitude(new Vector3(movement.x, movement.y, 0)/10,3));
			}
			//Fall down
			movement.y -= gravity;

				//Release the jump without jumping
				if (jump) {
					jump = false;
				}

			//Air control
			if (Input.GetAxis("Horizontal") != 0) {
				movement.x = Mathf.MoveTowards(movement.x, Input.GetAxis("Horizontal") * run_speed, run_speed * air_friction/50);
			}
			else {
				movement.x = Mathf.Lerp(movement.x, 0, air_friction/100);
			}
			break;
		}

		//Apply altered velocity
		rigidbody2D.velocity = movement;
   	}

	//Detecting ground collision and surface normals
	void OnCollisionStay2D(Collision2D col) {
			if (col.contacts[0].normal.y > 0.5f) {
			grounded = true;
			ground_normal = col.contacts[0].normal;
			}
	}

	void OnCollisionExit2D(Collision2D col) {
			if (col.contacts[0].normal.y > 0.5f) {
				grounded = false;
				ground_normal = -Vector2.up;
			}
		}



}