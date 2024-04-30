extends CharacterBody3D

@export_group("movement")
@export var walk_speed : float = 5.0
@export var sprint_speed : float = 8.0
@export var slide_speed : float = 10.0
@export var crouch_speed : float  = 3.0
@export var jump_velocity : float  = 4.5
@export var sensitivity : float  = 0.003
@export var running_friction : float  = 7.0
@export var air_friction : float  = 3.0
@export_group("head bob")
#bob variables
@export var t_bob = 0.0
@export var head_bob_freq : float = 2.0
@export var head_bob_amp : float = 0.08
@export var t_weapon_bob = 0.0
@export var weapon_bob_freq : float = 2.0
@export var weapon_bob_amp : float = 0.02

@export_group("FOV")
#fov variables
@export var base_fov : float = 75.0
@export var sprint_fov_change : float = 1.5
@export var slide_fov_change : float = .75
@export var fov_lerp_distance_per_frame : float = 8.0

@export_group("Other")
#crouch\slide Variables
@export var crouch_scale : Vector3 = Vector3(1,.75,1)
@export var walk_scale : Vector3 = Vector3(1,1,1)
@export var motion_lines : ColorRect

@export var speed = walk_speed

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
		head.rotate_y(-event.relative.x * sensitivity)	
		camera.rotate_x(-event.relative.y * sensitivity)
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
			velocity.y = jump_velocity
		state.AIR:
			velocity.x = lerp(velocity.x, direction.x * speed, delta * air_friction)
			velocity.z = lerp(velocity.z, direction.z * speed, delta * air_friction)
		state.WALK:
			collider.scale = walk_scale
			speed = walk_speed
		state.SPRINT:	
			speed = sprint_speed
			collider.scale = walk_scale
		state.CROUCH:	
			collider.scale = crouch_scale
			speed = crouch_speed
		state.SLIDE:
			collider.scale = crouch_scale
			speed = slide_speed
		state.IDLE:	
			velocity.x = lerp(velocity.x, direction.x * speed, delta * running_friction)
			velocity.z = lerp(velocity.z, direction.z * speed, delta * running_friction)
	if current_state != state.CROUCH:	
		t_bob += delta * velocity.length() * float(is_on_floor())
		t_weapon_bob += delta * velocity.length()
		camera.transform.origin = _headbob(t_bob,head_bob_amp,head_bob_freq)
		# weapon_mesh.transform.origin = _headbob(t_weapon_bob,weapon_bob_amp,weapon_bob_freq)
	if current_state != state.IDLE:
		velocity.x = direction.x * speed
		velocity.z = direction.z * speed

	#FOV
	var velocity_clamped = clamp(velocity.length(), 0.5, sprint_speed * 2)
	var target_fov = base_fov + sprint_fov_change * velocity_clamped
	camera.fov = lerp(camera.fov,target_fov,delta * fov_lerp_distance_per_frame)

	#motion lines
	# print(clampf(velocity.length()-walk_speed,0,slide_speed * 2)/(slide_speed * 2)*4)
	motion_lines.material.set_shader_parameter("alpha",clampf(velocity.length()-walk_speed,0,slide_speed * 2)/ (slide_speed * 2) * float(is_on_floor()))
	motion_lines.material.set_shader_parameter("speedScale",clampf(velocity.length()-walk_speed,0,slide_speed * 2)/ (slide_speed * 2)*8 * float(is_on_floor())) 

	move_and_slide()

func _headbob(time,bob_amp : float,bob_freq : float) -> Vector3:
	var pos = Vector3.ZERO
	pos.y = sin(time * bob_freq) * bob_amp
	pos.x = cos(time * bob_freq/2) * bob_amp
	return pos

func is_moving() -> bool:
	return velocity != Vector3.ZERO

func change_state(new_state : state):
	if new_state != current_state:
		last_state = current_state
		current_state = new_state
		# match current_state:
		# 	state.AIR:
		# 		print("state is AIR")
		# 	state.WALK:
		# 		print("state is WALK")
		# 	state.SPRINT:	
		# 		print("state is SPRINT")
		# 	state.CROUCH:	
		# 		print("state is CROUCH")
		# 	state.SLIDE:
		# 		print("state is SLIDE")
		# 	state.IDLE:	
		# 		print("state is IDLE")
