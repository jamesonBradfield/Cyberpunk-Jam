extends MeshInstance3D

@onready var crt_viewport : SubViewport = $CrtViewport
@onready var icon_viewport : SubViewport = $IconViewport

func _ready() -> void:
	get_active_material(0).albedo_texture = crt_viewport.get_texture()
	get_active_material(0).next_pass.next_pass.albedo_texture = icon_viewport.get_texture()
	get_active_material(0).next_pass.next_pass.emission_texture = icon_viewport.get_texture()

func set_icon_used(albedo_color : Color,emission_color : Color) -> void:
	get_active_material(0).next_pass.next_pass.albedo_color = albedo_color 
	get_active_material(0).next_pass.next_pass.emission_color = emission_color 


func _on_progress_bar_value_changed(value):
	pass # Replace with function body.
