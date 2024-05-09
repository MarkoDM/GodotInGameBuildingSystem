@tool
extends EditorScenePostImport

# This is an import script intendet to be attached during importing or reimporting assets
# It creates an empty parent node for every onject
# Than it saves that as a separate scene

var path: String = "" # Change path here or use default where asset is
var rootNode;

# Called right after the scene is imported and gets the root node.
func _post_import(scene: Node) -> Object:
	if (path == ""):
		path = get_source_path()
	print(path)
	create_scenes(scene)
	# Return the imported scene
	return scene

func set_owner(node, new_owner) -> void:
	node.owner = new_owner
	for child in node.get_children():
		set_owner(child, new_owner)
		
func create_scenes(node: Node) -> void:
	if node != null:
		for child in node.get_children():
#			if node is MeshInstance3D:
				# Save this MeshInstance to its own scene
			var packed_scene: PackedScene = PackedScene.new()
			# Create a new empty parent node
			var parentNode: Node3D = Node3D.new()
			parentNode.name = child.name
			# Remove the original node from its current parent
			if child.get_parent() != null:
				var originalParent = child.get_parent()
#				print("originalParent: ", originalParent.name)
				originalParent.remove_child(child)
			parentNode.add_child(child)
#			# Don't forget to set the new owner
			set_owner(child, parentNode)
			packed_scene.pack(parentNode)
			var scene_path = path + child.name.replace("-", "_").to_lower() + ".tscn"
			print(scene_path)
			ResourceSaver.save(packed_scene, scene_path)
			# Create a resource for it
			# Create an instance of the C# resource
			var visual = load(scene_path)
			var buildable_resource = preload("res://BuildingSystem/Scripts/Resources/BuildableResource.cs").new()
			# Set some properties
			buildable_resource.Name = child.name
			buildable_resource.Object3DModel = visual
			var resource_path = path + child.name.replace("-", "_").to_lower() + ".tres"
			ResourceSaver.save(buildable_resource, resource_path)
			

# Returns the path of the file being imported
func get_source_path() -> String:
	var path_segments: PackedStringArray = get_source_file().split("/")
	path_segments.resize(path_segments.size() - 1)
	return "/".join(path_segments) + "/"
