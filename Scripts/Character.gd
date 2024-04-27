extends CharacterBody3D

const WALK_SPEED = 5.0
const SPRINT_SPEED = 8.0
const SLIDE_SPEED= 10.0
const CROUCH_SPEED = 3.0
const JUMP_VELOCITY = 4.5
const SENSITIVITY = 0.003
const RUNNING_FRICTION = 7.0
const AIR_FRICTION = 3.0

#bob variables
const HEAD_BOB_FREQ = 2.0
const BOB_AMP = 0.08
var t_bob = 0.0

#fov variables
const BASE_FOV = 75.0
const SPRINT_FOV_CHANGE = 1.5
const SLIDE_FOV_CHANGE = .75
const FOV_LERP_DISTANCE_PER_FRAME = 8.0

#crouch\slide Variables
const CROUCH_SCALE : Vector3 = Vector3(1,.75,1)
const WALK_SCALE : Vector3 = Vector3(1,1,1)

# Energy Drain variables.
const SPRINT_ENERGY_DRAIN = 2.5

var speed = WALK_SPEED

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = 9.8

@onready var head = $Head
@onready var camera = $Head/Camera3D
@onready var collider : CollisionShape3D = $CollisionShape3D 
@onready var weapon_mesh : Node3D = $Head/Camera3D/WeaponBase/WeaponMesh
var is_crouching : bool = false
var direction : Vector3
var input_dir : Vector2

enum state {SPRINT,WALK,AIR,JUMP,SLIDE,CROUCH,IDLE}
var last_state : state
var current_state : state = state.IDLE

func _ready():
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _input(event):
	if event is InputEventMouseMotion:
		head.rotate_y(-event.relative.x * SENSITIVITY)	
		camera.rotate_x(-event.relative.y * SENSITIVITY)
		camera.rotation.x = clamp(camera.rotation.x, deg_to_rad(-40),deg_to_rad(60))


func _process(_delta):
	input_dir = Input.get_vector("left", "right", "up", "down")
	direction = (head.transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	handle_state()

func handle_state():
	if is_moving() and is_on_floor() and Input.is_action_pressed("sprint") and Input.is_action_pressed("crouch"):
		change_state(state.SLIDE)
	elif is_on_floor() and Input.is_action_pressed("crouch"):
		change_state(state.CROUCH)
	elif is_moving() and Input.is_action_pressed("sprint") and is_on_floor() and not Input.is_action_pressed("crouch"):
		change_state(state.SPRINT)
	elif is_moving() and is_on_floor() and not Input.is_action_pressed("crouch"):
		change_state(state.WALK)
	elif not Input.is_action_pressed("crouch") and is_on_floor() or not Input.is_action_pressed("sprint") and is_on_floor() or not is_moving() and is_on_floor():
		change_state(state.IDLE)
	if Input.is_action_just_pressed("jump") and is_on_floor():
		change_state(state.JUMP)
	if not is_on_floor():
		change_state(state.AIR)

func _physics_process(delta):
	velocity.y -= gravity * delta
	match current_state:
		state.JUMP:
			velocity.y = JUMP_VELOCITY
		state.AIR:
			velocity.x = lerp(velocity.x, direction.x * speed, delta * AIR_FRICTION)
			velocity.z = lerp(velocity.z, direction.z * speed, delta * AIR_FRICTION)
		state.WALK:
			collider.scale = WALK_SCALE
			speed = WALK_SPEED
		state.SPRINT:	
			speed = SPRINT_SPEED
			collider.scale = WALK_SCALE
		state.CROUCH:	
			collider.scale = CROUCH_SCALE
			speed = CROUCH_SPEED
		state.SLIDE:
			collider.scale = CROUCH_SCALE
			speed = SLIDE_SPEED
		state.IDLE:	
			velocity.x = lerp(velocity.x, direction.x * speed, delta * RUNNING_FRICTION)
			velocity.z = lerp(velocity.z, direction.z * speed, delta * RUNNING_FRICTION)
	if current_state != state.CROUCH:	
		t_bob += delta * velocity.length() * float(is_on_floor())
		camera.transform.origin = _headbob(t_bob)
		weapon_mesh.transform.origin = _headbob(t_bob)
	if current_state != state.IDLE:
		velocity.x = direction.x * speed
		velocity.z = direction.z * speed

	#FOV
	var velocity_clamped = clamp(velocity.length(), 0.5, SPRINT_SPEED * 2)
	var target_fov = BASE_FOV + SPRINT_FOV_CHANGE * velocity_clamped
	camera.fov = lerp(camera.fov,target_fov,delta * FOV_LERP_DISTANCE_PER_FRAME)

	move_and_slide()

func _headbob(time) -> Vector3:
	var pos = Vector3.ZERO
	pos.y = sin(time * HEAD_BOB_FREQ) * BOB_AMP
	pos.x = cos(time * HEAD_BOB_FREQ/2) * BOB_AMP
	return pos

func is_moving() -> bool:
	return velocity != Vector3.ZERO

func change_state(new_state : state):
	if new_state != current_state:
		last_state = current_state
		current_state = new_state
		match current_state:
			state.AIR:
				print("state is AIR")
			state.WALK:
				print("state is WALK")
			state.SPRINT:	
				print("state is SPRINT")
			state.CROUCH:	
				print("state is CROUCH")
			state.SLIDE:
				print("state is SLIDE")
			state.IDLE:	
				print("state is IDLE")
