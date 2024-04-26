extends CharacterBody3D

var speed
const WALK_SPEED = 5.0
const SPRINT_SPEED = 8.0
const SLIDE_SPEED= 10.0
const CROUCH_SPEED = 3.0
const JUMP_VELOCITY = 4.5
const SENSITIVITY = 0.003
const RUNNING_FRICTION = 7.0
const AIR_FRICTION = 3.0

#bob variables
const BOB_FREQ = 2.0
const BOB_AMP = 0.08
var t_bob = 0.0

#fov variables
const BASE_FOV = 75.0
const FOV_CHANGE = 1.5
const FOV_LERP_DISTANCE_PER_FRAME = 8.0

#crouch\slide Variables
const CROUCH_SCALE : Vector3 = Vector3(1,.75,1)
const WALK_SCALE : Vector3 = Vector3(1,1,1)

# Energy Drain variables.
const SPRINT_ENERGY_DRAIN = 2.5

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = 9.8
@onready var head = $Head
@onready var camera = $Head/Camera3D
@onready var collider : CollisionShape3D = $CollisionShape3D 
@onready var energy_bar : ProgressBar = $Control/EnergyBar

func _ready():
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _input(event):
	if event is InputEventMouseMotion:
		head.rotate_y(-event.relative.x * SENSITIVITY)	
		camera.rotate_x(-event.relative.y * SENSITIVITY)
		camera.rotation.x = clamp(camera.rotation.x, deg_to_rad(-40),deg_to_rad(60))

func _physics_process(delta):
	# Add the gravity.
	if not is_on_floor():
		velocity.y -= gravity * delta

	# Handle jump.
	if Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = JUMP_VELOCITY
	# Handle Sprint.
	if Input.is_action_pressed("sprint"):
		speed = SPRINT_SPEED
		collider.scale = WALK_SCALE
		if Input.is_action_pressed("crouch"):	
			collider.scale = CROUCH_SCALE
			speed = SLIDE_SPEED
	elif Input.is_action_pressed("crouch"):
		collider.scale = CROUCH_SCALE
		speed = CROUCH_SPEED
	else:
		collider.scale = WALK_SCALE
		speed = WALK_SPEED

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var input_dir = Input.get_vector("left", "right", "up", "down")
	var direction = (head.transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if is_on_floor():
		if direction:
			velocity.x = direction.x * speed
			velocity.z = direction.z * speed
		else:
			velocity.x = lerp(velocity.x, direction.x * speed, delta * RUNNING_FRICTION)
			velocity.z = lerp(velocity.z, direction.z * speed, delta * RUNNING_FRICTION)
	else:
		velocity.x = lerp(velocity.x, direction.x * speed, delta * AIR_FRICTION)
		velocity.z = lerp(velocity.z, direction.z * speed, delta * AIR_FRICTION)

	#Head bob
	t_bob += delta * velocity.length() * float(is_on_floor())
	camera.transform.origin = _headbob(t_bob)

	#FOV
	var velocity_clamped = clamp(velocity.length(), 0.5, SPRINT_SPEED * 2)
	var target_fov = BASE_FOV + FOV_CHANGE * velocity_clamped
	camera.fov = lerp(camera.fov,target_fov,delta * FOV_LERP_DISTANCE_PER_FRAME)

	move_and_slide()

func _headbob(time) -> Vector3:
	var pos = Vector3.ZERO
	pos.y = sin(time * BOB_FREQ) * BOB_AMP
	pos.x = cos(time * BOB_FREQ/2) * BOB_AMP
	return pos
