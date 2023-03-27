import fnmatch
import os
import re
import struct
import tkinter as tk
from tkinter import filedialog
import argparse
from typing import Any, BinaryIO
import zipfile
import vedo


STRING_CODEC = 'latin-1'

REDUCED_FOOTER_FILES = [
    "ob_DREIFACHGALGEN.bgf",
    "ob_DREIFACKREUZ.bgf",
    "ob_EXEKUTIONSKANONESTOPFER.bgf",
]

EXCLUDE_PATHS = []

def main():
    parser = argparse.ArgumentParser(description='gilde-decoder')
    parser.add_argument('-i', '--input', help='input path')
    parser.add_argument('-o', '--output', help='output path')

    args = parser.parse_args()

    if not args.input:
        root = tk.Tk()
        root.withdraw()
        args.input = filedialog.askdirectory()
        root.destroy()
        args.input = args.input.replace('/', '\\')

    if not args.output:
        args.output = os.path.join(os.getcwd(), 'output')

    if not os.path.exists(args.input):
        print('input path does not exist')
        return

    if not os.path.exists(args.output):
        os.mkdir(args.output)
        
    resources_path = os.path.join(args.input, "Resources")
    objects_bin_path = os.path.join(resources_path, "objects.bin")

    if not os.path.exists(objects_bin_path):
        print('objects.bin not found')
        return
    
    bgf_base_path = os.path.join(args.output, "bgf")

    if not os.path.exists(bgf_base_path):
        os.makedirs(bgf_base_path)

    with zipfile.ZipFile(objects_bin_path, 'r') as zip_ref:
        zip_ref.extractall(bgf_base_path)

    file_paths = []
    for root, directories, files in os.walk(bgf_base_path):
        for filename in fnmatch.filter(files, '*.bgf'):
            filepath = os.path.join(root, filename)
            if any(filepath.endswith(exclude_path) for exclude_path in EXCLUDE_PATHS):
                continue
            file_paths.append(filepath)

    bgfs = []
    for bgf_base_path in file_paths:
        print(f'Decoding {bgf_base_path}')
        bgf = decode_bgf(bgf_base_path)
        bgfs.append(bgf)

    bgf_base_path = os.path.join(args.output, "bgf")
    obj_base_path = os.path.join(args.output, "obj")

    if not os.path.exists(obj_base_path):
        os.makedirs(obj_base_path)

    for bgf in bgfs:
        print(f'Converting {bgf["path"]}')

        relative_path = subtract_path(bgf_base_path, bgf["path"])
        obj_path = os.path.join(obj_base_path, relative_path)
        obj_path = os.path.splitext(obj_path)[0]

        if not os.path.exists(obj_path):
            os.makedirs(obj_path)

        vertices = [vertex_mapping[0] for vertex_mapping in bgf["mapping_object"]["vertex_mappings"]]
        faces = [polygon_mapping["face"] for polygon_mapping in bgf["mapping_object"]["polygon_mappings"]]
        normals = [polygon_mapping["v3"] for polygon_mapping in bgf["mapping_object"]["polygon_mappings"]]

        obj_file_name = sanitize_filename(f'combined.obj')
        obj_file_path = os.path.join(obj_path, obj_file_name)
        convert_object(vertices, faces, normals, obj_file_path)
    
def decode_bgf(input_path: str) -> dict:
    bgf = {
        "path": input_path,
    }

    with open(input_path, 'rb') as file:
        bgf["bgf_header"] = decode_bgf_header(file)

        textures = []
        while is_texture(file):
            textures.append(decode_texture(file))
        bgf["textures"] = textures

        game_objects = []
        while is_game_object(file):
            game_objects.append(decode_game_object(file))
        bgf["game_objects"] = game_objects

        bgf["mapping_object"] = decode_mapping_object(file)

        assert is_valid_footer(bgf, file)
    
    return bgf

# Helper methods

def read_string(file: BinaryIO) -> str:
    value = ""
    buffer = file.read(1)
    while buffer != b'\x00':
        value += buffer.decode(STRING_CODEC)
        buffer = file.read(1)
    return value

def is_value(
    file: BinaryIO, 
    length: int, 
    value: int | float, 
    reset: bool
) -> bool:
    if length not in (1, 2, 3, 4):
        raise ValueError("Length must be between 1 and 4")
    
    if isinstance(value, float) and length != 4:
        raise ValueError("Length must be 4 for float values")

    if isinstance(value, float):
        fmt = '<f'
    elif length == 1:
        fmt = '<B'
    elif length == 2:
        fmt = '<H'
    elif length == 3:
        fmt = None
    else:
        fmt = '<I'

    initial_pos = file.tell()

    data = file.read(length)
    
    if len(data) < length:
        if reset:
            file.seek(initial_pos)
        return False
    
    if length == 3:
        unpacked_value = int.from_bytes(data, byteorder='little', signed=False)
    else:
        unpacked_value = struct.unpack(fmt, data)[0]

    is_equal = unpacked_value == value

    if reset:
        file.seek(initial_pos)

    return is_equal

def skip_optional(file: BinaryIO, value: bytes, padding: int) -> bool:
    read_value = file.read(len(value))
    file.seek(-len(value), os.SEEK_CUR)
    
    if read_value != value:
        return False

    file.seek(padding, os.SEEK_CUR)
    return True

def skip_required(file: BinaryIO, value: bytes, padding: int) -> None:
    read_value = file.read(len(value))
    file.seek(-len(value), os.SEEK_CUR)
    
    assert read_value == value, f"Expected {value}, got {read_value}"

    file.seek(padding, os.SEEK_CUR)

def skip_zero(file: BinaryIO, length: int) -> None:
    for _ in range(length):
        assert int.from_bytes(file.read(1), byteorder='little', signed=False) == 0, f"Expected 0, got {int.from_bytes(file.read(1), byteorder='little', signed=False)}"

def skip_until(file: BinaryIO, value: int, length: int) -> None:
    while not is_value(file, length, value, reset=True):
        file.seek(1, os.SEEK_CUR)
    file.seek(1, os.SEEK_CUR)

def is_latin_1(value: int) -> bool:
    return 0x20 <= value <= 0x7E

def find_address_of_byte_pattern(pattern: bytes, data: bytes) -> int:
    if not pattern or not data:
        return []

    pattern_length = len(pattern)
    data_length = len(data)
    occurrences = []

    for i in range(data_length - pattern_length + 1):
        if (i == 0 or not is_latin_1(data[i - 1])) and data[i:i + pattern_length] == pattern and data[i + pattern_length] == 0:
            occurrences.append(i)

    return occurrences

def subtract_path(base_path, target_path):
    base_path = os.path.normpath(base_path)  # Normalize the path to remove redundant separators
    target_path = os.path.normpath(target_path)

    # Make sure target_path starts with base_path
    if not target_path.startswith(base_path):
        raise ValueError("Target path is not a subpath of the base path.")

    relative_path = target_path[len(base_path):]  # Remove the base_path from target_path

    if relative_path.startswith(os.path.sep):
        relative_path = relative_path[len(os.path.sep):]  # Remove the leading path separator, if any

    return relative_path

def sanitize_filename(path, replacement='_'):
    # Define a set of illegal characters for Windows and Unix-based systems
    illegal_characters = r'<>:"/\|?*'
    if os.name == 'nt':  # Windows
        illegal_characters += r'\\'  # Add backslash as an illegal character on Windows

    # Replace illegal characters with the replacement character
    sanitized_path = re.sub(f'[{re.escape(illegal_characters)}]', replacement, path)

    return sanitized_path

# Checker functions

def is_texture(file: BinaryIO) -> bool:
    initial_pos = file.tell()

    is_texture = True

    if not is_value(file, 2, 0x0605, reset=False):
        is_texture = False

    file.seek(initial_pos, os.SEEK_SET)

    return is_texture

def is_game_object(file: BinaryIO) -> bool:
    initial_pos = file.tell()

    is_obj = True

    if is_value(file, 1, 0x28, reset=True):
        file.seek(1, os.SEEK_CUR)

    if not is_value(file, 2, 0x1514, reset=False):
        is_obj = False

    file.seek(initial_pos, os.SEEK_SET)

    return is_obj

# Low level structs

def decode_vertex(file: BinaryIO) -> tuple[float, float, float]:
    x = struct.unpack('<f', file.read(4))[0]
    y = struct.unpack('<f', file.read(4))[0]
    z = struct.unpack('<f', file.read(4))[0]
    return (x, y, z)

def decode_vertex_mapping(file: BinaryIO) -> tuple[tuple[float, float, float], tuple[float, float, float]]:
    vertex1 = decode_vertex(file)
    vertex2 = decode_vertex(file)
    return (vertex1, vertex2)

def decode_face(file: BinaryIO) -> tuple[int, int, int]:
    a = int.from_bytes(file.read(4), byteorder='little', signed=False)
    b = int.from_bytes(file.read(4), byteorder='little', signed=False)
    c = int.from_bytes(file.read(4), byteorder='little', signed=False)
    return (a, b, c)

def decode_polygon(file: BinaryIO) -> dict:
    polygon = {}

    polygon["face"] = decode_face(file)

    skip_optional(file, b"\x1E", 1)

    polygon["v1"] = decode_vertex(file)
    polygon["v2"] = decode_vertex(file)
    polygon["v3"] = decode_vertex(file)

    has_normal = skip_optional(file, b"\x1F", 1)

    if has_normal:
        polygon["normal"] = decode_vertex(file)

        has_texture = skip_optional(file, b"\x20", 1)

        if has_texture:
            polygon["texture_index"] = int.from_bytes(file.read(1), byteorder='little', signed=False)

        skip_optional(file, b"\x1D", 1)

    return polygon

def decode_polygon_mapping(file: BinaryIO) -> dict:
    polygon_mapping = {}

    polygon_mapping["face"] = decode_face(file)
    polygon_mapping["v1"] = decode_vertex(file)
    polygon_mapping["v2"] = decode_vertex(file)
    polygon_mapping["v3"] = decode_vertex(file)
    polygon_mapping["texture_index"] = int.from_bytes(file.read(1), byteorder='little', signed=False)

    return polygon_mapping

# High level structs

def decode_bgf_header(file: BinaryIO) -> dict:
    bgf_header = {}

    bgf_header["name"] = read_string(file)

    skip_required(file, b"\x2E", 1)
    bgf_header["mapping_address"] = int.from_bytes(file.read(4), byteorder='little', signed=False)

    skip_required(file, b"\x01\x01", 2)
    bgf_header["num1"] = int.from_bytes(file.read(1), byteorder='little', signed=False)

    skip_required(file, b"\xCD\xAB\x02", 3)
    bgf_header["num2"] = int.from_bytes(file.read(1), byteorder='little', signed=False)

    has_anim = skip_optional(file, b"\x37", 1)
    if has_anim:
        bgf_header["anim_count"] = int.from_bytes(file.read(2), byteorder='little', signed=False)
        skip_zero(file, 2)

    skip_required(file, b"\x03\x04", 2)
    bgf_header["texture_count"] = int.from_bytes(file.read(2), byteorder='little', signed=False)
    skip_zero(file, 2)

    return bgf_header

def decode_texture(file: BinaryIO) -> dict:
    texture = {}
    
    skip_required(file, b"\x05\x06", 2)
    texture["id"] = int.from_bytes(file.read(2), byteorder='little', signed=False)
    skip_zero(file, 2)

    skip_optional(file, b"\x07", 1)
    skip_optional(file, b"\x08", 1)
    texture["name"] = read_string(file)

    has_appendix_1 = skip_optional(file, b"\x08", 1)
    has_appendix_2 = skip_optional(file, b"\x09", 1)

    if has_appendix_1 or has_appendix_2:
        texture["appendix"] = read_string(file)
        texture["appendix_type"] = 1 if has_appendix_1 else 2

    skip_until(file, 0x28, 1)

    return texture

def decode_model(file: BinaryIO) -> dict:
    model = {}

    skip_required(file, b"\x19", 1)

    model["vertex_count"] = int.from_bytes(file.read(2), byteorder='little', signed=False)

    skip_zero(file, 2)

    skip_required(file, b"\x1A", 1)

    model["polygon_count"] = int.from_bytes(file.read(2), byteorder='little', signed=False)

    skip_zero(file, 2)

    skip_required(file, b"\x1B", 1)

    vertices = []
    for _ in range(model["vertex_count"]):
        vertex = decode_vertex(file)
        vertices.append(vertex)
    model["vertices"] = vertices

    skip_required(file, b"\x1C\x1D", 2)

    polygons = []
    for _ in range(model["polygon_count"]):
        polygon = decode_polygon(file)
        polygons.append(polygon)
    model["polygons"] = polygons

    return model

def decode_anim_data(file: BinaryIO) -> dict:
    anim_data = {}
    
    skip_required(file, b"\x38", 1)

    anim_data["name"] = read_string(file)

    skip_required(file, b"\x39", 1)

    anim_data["x1"] = struct.unpack("<f", file.read(4))[0]
    anim_data["y1"] = struct.unpack("<f", file.read(4))[0]
    anim_data["z1"] = struct.unpack("<f", file.read(4))[0]
    anim_data["val"] = int.from_bytes(file.read(1), byteorder='little', signed=False)
    anim_data["x2"] = struct.unpack("<f", file.read(4))[0]
    anim_data["y2"] = struct.unpack("<f", file.read(4))[0]
    anim_data["z3"] = struct.unpack("<f", file.read(4))[0]

    return anim_data

def decode_game_object(file: BinaryIO) -> dict:
    obj = {}

    skip_optional(file, b"\x28", 1)
    skip_optional(file, b"\x14\x15", 2)

    obj["name"] = read_string(file)

    skip_optional(file, b"\x16\x01", 5)
    has_model = skip_optional(file, b"\x17\x18", 6)

    if has_model:
        obj["model"] = decode_model(file)

    skip_optional(file, b"\x28", 1)
    skip_optional(file, b"\x28", 1)
    skip_optional(file, b"\x28", 1)
    has_anim_data = skip_optional(file, b"\x37", 1)

    obj["anim_count"] = 0
    anim_datas = []
    if has_anim_data:
        obj["anim_count"] = int.from_bytes(file.read(2), byteorder='little', signed=False)
        skip_zero(file, 2)
        for _ in range(obj["anim_count"]):
            anim_datas.append(decode_anim_data(file))
    obj["anim_datas"] = anim_datas

    return obj

def decode_mapping_object(file: BinaryIO) -> dict:
    mapping_object = {}
    
    skip_required(file, b"\x2F\x2D", 2)

    mapping_object["num1"] = int.from_bytes(file.read(1), byteorder='little', signed=False)
    mapping_object["num2"] = int.from_bytes(file.read(2), byteorder='little', signed=False)
    _ = int.from_bytes(file.read(1), byteorder='little', signed=False)
    mapping_object["num3"] = int.from_bytes(file.read(2), byteorder='little', signed=False)

    skip_required(file, b"\xB5\xFA", 2)
    mapping_object["num4"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    mapping_object["vertex_mapping_count"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    mapping_object["polygon_mapping_count"] = int.from_bytes(file.read(4), byteorder='little', signed=False)

    vertex_mappings = []
    for _ in range(mapping_object["vertex_mapping_count"]):
        vertex_mapping = decode_vertex_mapping(file)
        vertex_mappings.append(vertex_mapping)
    mapping_object["vertex_mappings"] = vertex_mappings

    box_vertex_mappings = []
    for _ in range(8):
        box_vertex_mapping = decode_vertex_mapping(file)
        box_vertex_mappings.append(box_vertex_mapping)
    mapping_object["box_vertex_mappings"] = box_vertex_mappings

    mapping_object["some_float"] = struct.unpack("<f", file.read(4))[0]

    polygon_mappings = []
    for _ in range(mapping_object["polygon_mapping_count"]):
        polygon_mapping = decode_polygon_mapping(file)
        polygon_mappings.append(polygon_mapping)
    mapping_object["polygon_mappings"] = polygon_mappings

    return mapping_object

# Footer structs

def is_valid_footer(bgf: dict, file: BinaryIO) -> bool:
    initial_position = file.tell()
    footer = file.read()

    literal_count = 0
    footer_texture_count = 0
    footer_anim_count = 0

    texture_bytes_found = []
    for texture in bgf["textures"]:
        texture_name = texture["name"].split(".")[0]

        texture_name_bytes = texture_name.encode(STRING_CODEC)

        if "appendix" in texture:
            appendix = texture["appendix"].split(".")[0]
            appendix_bytes = appendix.encode(STRING_CODEC)

            if texture["appendix_type"] == 1:
                texture_name_bytes += b'\x00' + appendix_bytes
            elif texture["appendix_type"] == 2:
                texture_name_bytes += b'\x00\x00' + appendix_bytes
            else:
                raise Exception("Unknown appendix type")
            
            texture_name += appendix

        if texture_name_bytes in texture_bytes_found:
            continue
        
        relative_positions = find_address_of_byte_pattern(texture_name_bytes, footer)
        
        if len(relative_positions) == 0:
            continue
        
        texture_bytes_found.append(texture_name_bytes)
        literal_count += len(texture_name * len(relative_positions))
        footer_texture_count += len(relative_positions)

    file.seek(initial_position + footer_texture_count * 9 + literal_count + 4)

    game_objects_with_anim_data = [obj for obj in bgf["game_objects"] if len(obj["anim_datas"]) > 0]
    anim_literal_count = 0
    footer_anim_count = 0
    for obj in game_objects_with_anim_data:
        for _ in obj["anim_datas"]:
            value = read_string(file)
            file.seek(24, os.SEEK_CUR)
            anim_literal_count += len(value)
            footer_anim_count += 1

    expected_non_literal_count = footer_texture_count * 9 + footer_anim_count * 25 + 5

    is_reduced_footer_length = any([bgf["path"].endswith(reduced_footer_files) for reduced_footer_files in REDUCED_FOOTER_FILES])
    if is_reduced_footer_length:
        expected_non_literal_count -= 4

    non_literal_count = len(footer) - literal_count - anim_literal_count

    is_valid = non_literal_count == expected_non_literal_count

    if not is_valid:
        print(f"Got {non_literal_count} non-literal bytes instead of {expected_non_literal_count}")

    return is_valid

def decode_anim_footer(file: BinaryIO) -> dict:
    anim_footer = {}
    
    anim_footer["name"] = read_string(file)

    file.seek(27, os.SEEK_CUR)

    return anim_footer

# Obj Conversion

def convert_object(vertices: list[tuple[int]], faces: list[tuple[int]], normals: list[tuple[float]], output_path: str) -> None:
    with open(output_path, 'w') as file:
        file.write("g test\n")

        for (x, y, z) in vertices:
            file.write(f"v {x} {y} {z}\n")

        assert len(faces) == len(normals)

        for i in range(len(normals)):
            (x, y, z) = normals[i]
            file.write(f"vn {x} {y} {z}\n")

        for i in range(len(faces)):
            (v1, v2, v3) = faces[i]
            file.write(f"f {v1 + 1}\\\\{i + 1} {v2 + 1}\\\\{i + 1} {v3 + 1}\\\\{i + 1}\n")

def show_object(path: str) -> None:
    mesh = vedo.Mesh(path)
    mesh.show()

if __name__ == "__main__":
    main()