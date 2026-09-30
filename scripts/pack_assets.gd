extends SceneTree

var packer := PCKPacker.new()
var included: Dictionary = {}
var target := "res://assets/CraftTheSpire.pck"

func _initialize() -> void:
    var args := OS.get_cmdline_user_args()
    if args.size() > 0:
        target = args[0]
    if packer.pck_start(target) != OK:
        fail("Cannot create pack: " + target)
        return
    if not add_directory("res://CraftTheSpire"):
        return
    if packer.flush() != OK:
        fail("PCK flush failed")
        return
    print("Craft the Spire assets packed: ", included.size(), " files into ", target)
    quit(0)

func add_file(path: String) -> bool:
    if included.has(path):
        return true
    if not FileAccess.file_exists(path):
        fail("Missing imported resource: " + path)
        return false
    if packer.add_file(path, path) != OK:
        fail("Cannot pack " + path)
        return false
    included[path] = true
    return true

func add_directory(path: String) -> bool:
    var dir := DirAccess.open(path)
    if dir == null:
        fail("Cannot open " + path)
        return false
    var names: Array[String] = []
    dir.list_dir_begin()
    var name := dir.get_next()
    while name != "":
        if not name.begins_with("."):
            names.append(name)
        name = dir.get_next()
    dir.list_dir_end()
    names.sort()
    for entry in names:
        var full := path.path_join(entry)
        if DirAccess.dir_exists_absolute(full):
            if not add_directory(full):
                return false
        elif entry.ends_with(".json") or entry.ends_with(".png"):
            if not add_file(full):
                return false
            if entry.ends_with(".png"):
                var import_path := full + ".import"
                if not add_file(import_path):
                    return false
                var config := ConfigFile.new()
                if config.load(import_path) != OK:
                    fail("Invalid import metadata " + import_path)
                    return false
                for imported in config.get_value("deps", "dest_files", []):
                    if not add_file(imported):
                        return false
    return true

func fail(message: String) -> void:
    push_error(message)
    quit(1)
