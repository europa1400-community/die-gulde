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

EXCLUDE_PATHS = [
    "ub_BAUMSTAMM_BJOERN.bgf",
    "ob_Nekromantenturm.bgf",
    "ob_GEDICHTLESUNG_TRIBUENE.bgf",
    "ob_LEIER.bgf",
    "Diener_KREATUR.bgf",
    "Friedhofsgehilfin_FRAU3.bgf",
    "Nekromant_MEISTER.bgf",
    "Wahrsagerin_FRAU2.bgf",
    "Wissenschaftler_KUTTE2.bgf",
    "ZIGEUNERMEISTER_MANN3.bgf",
    "Character\Abt_KUTTE.bgf",
]


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
            if "_DYNAMIC" in filepath:
                continue
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
        
        for obj in bgf["objects"]:
            vertices = obj["vertices"]
            bgf_polygons = obj["polygons"]
            faces = [(polygon["a"], polygon["b"], polygon["c"]) for polygon in bgf_polygons]
            obj_file_name = sanitize_filename(f'{obj["name"]}.obj')
            obj_file_path = os.path.join(obj_path, obj_file_name)
            convert_object(vertices, faces, obj_file_path)
    

def decode_bgf(input_path: str) -> dict:
    bgf = {
        "path": input_path,
    }

    with open(input_path, 'rb') as file:
        if not read_string(file) == "BGF":
            raise Exception("Not a BGF file")

        assert is_value(file, 1, 0x2E, reset=False)
        
        bgf["address"] = int.from_bytes(file.read(4), byteorder='little', signed=False)

        assert is_value(file, 2, 0x0101, reset=False)

        file.seek(1, os.SEEK_CUR)

        assert is_value(file, 3, 0x02ABCD, reset=False)

        file.seek(1, os.SEEK_CUR)

        if is_value(file, 1, 0x37, reset=True):
            file.seek(5, os.SEEK_CUR)

        assert is_value(file, 2, 0x0403, reset=False)

        texture_count = int.from_bytes(file.read(1), byteorder='little', signed=False)

        assert is_value(file, 3, 0, reset=False)

        textures = []
        for _ in range(texture_count):
            textures.append(decode_texture(file))
        bgf["textures"] = textures

        objects = []
        groups = []
        while True:
            if is_group(file):
                groups.append(decode_group(file))
            elif is_object(file):
                objects.append(decode_object(file))
            else:
                break
        bgf["objects"] = objects
        bgf["groups"] = groups

        assert is_value(file, 1, 0x28, reset=False)

        if is_value(file, 1, 0x37, reset=True):
            file.seek(5, os.SEEK_CUR)

        if is_value(file, 2, 0x1514, reset=True):
            file.seek(2, os.SEEK_CUR)
            name = read_string(file)

            assert is_value(file, 2, 0x0116, reset=False)
            file.seek(4, os.SEEK_CUR)

        if is_value(file, 1, 0x37, reset=True):
            file.seek(5, os.SEEK_CUR)

        assert is_value(file, 2, 0x2D2F, reset=False)

        file.seek(6, os.SEEK_CUR)

        assert is_value(file, 2, 0xFAB5, reset=False)

        some_count = int.from_bytes(file.read(4), byteorder='little', signed=False)
        vertex_mapping_count = int.from_bytes(file.read(4), byteorder='little', signed=False)
        polygon_mapping_count = int.from_bytes(file.read(4), byteorder='little', signed=False)

        vertex_mappings = []
        for _ in range(vertex_mapping_count):
            x = struct.unpack('<f', file.read(4))[0]
            y = struct.unpack('<f', file.read(4))[0]
            z = struct.unpack('<f', file.read(4))[0]
            alpha = struct.unpack('<f', file.read(4))[0]
            beta = struct.unpack('<f', file.read(4))[0]
            gamma = struct.unpack('<f', file.read(4))[0]
            vertex_mappings.append({
                'x': x,
                'y': y,
                'z': z,
                'alpha': alpha,
                'beta': beta,
                'gamma': gamma,
            })
        bgf["vertex_mappings"] = vertex_mappings

        special_vertex_mappings = []
        for _ in range(8):
            x = struct.unpack('<f', file.read(4))[0]
            y = struct.unpack('<f', file.read(4))[0]
            z = struct.unpack('<f', file.read(4))[0]
            alpha = struct.unpack('<f', file.read(4))[0]
            beta = struct.unpack('<f', file.read(4))[0]
            gamma = struct.unpack('<f', file.read(4))[0]
            special_vertex_mappings.append({
                'x': x,
                'y': y,
                'z': z,
                'alpha': alpha,
                'beta': beta,
                'gamma': gamma,
            })
        bgf["special_vertex_mappings"] = special_vertex_mappings

        some_float = struct.unpack('<f', file.read(4))[0]

        polygon_mappings = []
        for _ in range(polygon_mapping_count):
            polygon_mapping = decode_special_polygon(file)
            polygon_mappings.append(polygon_mapping)
        bgf["polygon_mappings"] = polygon_mappings

        assert is_valid_footer(bgf, file)
    
    return bgf


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


def is_latin_1(value: int) -> bool:
    return 0x20 <= value <= 0x7E


def is_valid_footer(bgf: dict, file: BinaryIO) -> bool:
    footer = file.read()

    literal_count = 0
    footer_texture_count = 0

    texture_bytes_found = []
    for texture in bgf["textures"]:
        texture_name = texture["name"].split(".")[0]
        texture_name_bytes = texture_name.encode(STRING_CODEC)

        if "name2" in texture:
            appendix = texture["name2"].split(".")[0]
            appendix_bytes = appendix.encode(STRING_CODEC)

            if texture["appendix_type"] == 0x08:
                texture_name_bytes += b'\x00' + appendix_bytes
            elif texture["appendix_type"] == 0x09:
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


    expected_non_literal_count = footer_texture_count * 9 + 5

    is_reduced_footer_length = any([bgf["path"].endswith(reduced_footer_files) for reduced_footer_files in REDUCED_FOOTER_FILES])
    if is_reduced_footer_length:
        expected_non_literal_count -= 4

    non_literal_count = len(footer) - literal_count

    is_valid = non_literal_count == expected_non_literal_count

    if not is_valid:
        print(f"Got {non_literal_count} non-literal bytes instead of {expected_non_literal_count}")

    return is_valid


def decode_texture(file: BinaryIO) -> dict:
    texture = {}

    assert is_value(file, 2, 0x0605, reset=False)

    texture["id"] = int.from_bytes(file.read(1), byteorder='little', signed=False)

    assert is_value(file, 3, 0, reset=False)
    
    texture["type"] = int.from_bytes(file.read(1), byteorder='little', signed=False)
    texture["name"] = read_string(file)

    if is_value(file, 1, 0x09, reset=True) or is_value(file, 1, 0x08, reset=True):
        appendix_type = int.from_bytes(file.read(1), byteorder='little', signed=False)
        texture["appendix_type"] = appendix_type
        texture["name2"] = read_string(file)

    while not is_value(file, 1, 0x28, reset=False):
        pass

    return texture


def decode_group(file: BinaryIO) -> dict:
    assert is_value(file, 1, 0x28, reset=False)

    grp = {}

    assert is_value(file, 2, 0x1514, reset=False)

    grp["name"] = read_string(file)

    assert is_value(file, 2, 0x0116, reset=False)
    assert is_value(file, 3, 0, reset=False)

    return grp


def decode_object(file: BinaryIO) -> dict:
    assert is_value(file, 1, 0x28, reset=False)

    obj = {}

    if is_value(file, 1, 0x37, reset=True):
        file.seek(5, os.SEEK_CUR)

    assert is_value(file, 2, 0x1514, reset=False)

    obj["name"] = read_string(file)

    assert is_value(file, 2, 0x0116, reset=False)
    assert is_value(file, 3, 0, reset=False)

    assert is_value(file, 2, 0x1817, reset=False)        
    assert is_value(file, 4, 0, reset=False)

    assert is_value(file, 1, 0x19, reset=False)
    vertex_count = int.from_bytes(file.read(2), byteorder='little', signed=False)
    assert is_value(file, 2, 0, reset=False)

    assert is_value(file, 1, 0x1A, reset=False)
    polygon_count = int.from_bytes(file.read(2), byteorder='little', signed=False)
    assert is_value(file, 2, 0, reset=False)

    assert is_value(file, 1, 0x1B, reset=False)

    vertices = []
    for _ in range(vertex_count):
        vertices.append(decode_vertex(file))
    obj["vertices"] = vertices

    assert is_value(file, 1, 0x1C, reset=False)
    assert is_value(file, 1, 0x1D, reset=False)

    polygons = []
    for _ in range(polygon_count):
        polygons.append(decode_polygon(file))
    obj["polygons"] = polygons

    assert is_value(file, 2, 0x2828, reset=False)

    return obj


def decode_vertex(file: BinaryIO) -> tuple:
    x = struct.unpack('<f', file.read(4))[0]
    y = struct.unpack('<f', file.read(4))[0]
    z = struct.unpack('<f', file.read(4))[0]
    return (x, y, z)


def decode_polygon(file: BinaryIO) -> dict:
    polygon = {}

    polygon["a"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    polygon["b"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    polygon["c"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    
    if is_value(file, 1, 0x1E, reset=True):
        file.seek(1, os.SEEK_CUR)

    polygon["vertex1"] = decode_vertex(file)
    polygon["vertex2"] = decode_vertex(file)
    polygon["vertex3"] = decode_vertex(file)

    is_1F = is_value(file, 1, 0x1F, reset=True)

    if is_1F:
        file.seek(1, os.SEEK_CUR)
        polygon["vertex4"] = decode_vertex(file)

        assert is_value(file, 1, 0x20, reset=False)

    polygon["texture_index"] = int.from_bytes(file.read(1), byteorder='little', signed=False)

    if is_1F and is_value(file, 1, 0x1D, reset=True):
        file.seek(1, os.SEEK_CUR)

    return polygon


def decode_special_polygon(file: BinaryIO) -> dict:
    polygon = {}

    polygon["a"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    polygon["b"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    polygon["c"] = int.from_bytes(file.read(4), byteorder='little', signed=False)

    polygon["vertex1"] = decode_vertex(file)
    polygon["vertex2"] = decode_vertex(file)
    polygon["vertex3"] = decode_vertex(file)

    polygon["texture_index"] = int.from_bytes(file.read(1), byteorder='little', signed=False)

    return polygon


def read_string(file: BinaryIO) -> str:
    value = ""
    buffer = file.read(1)
    while buffer != b'\x00':
        value += buffer.decode(STRING_CODEC)
        buffer = file.read(1)
    return value


def is_group(file: BinaryIO) -> bool:
    initial_pos = file.tell()

    is_grp = True

    if not is_value(file, 1, 0x28, reset=False):
        is_grp = False

    if not is_value(file, 2, 0x1514, reset=False):
        is_grp = False

    _ = read_string(file)

    if not is_value(file, 2, 0x0116, reset=False):
        is_grp = False

    if not is_value(file, 3, 0, reset=False):
        is_grp = False

    if not is_value(file, 3, 0x151428, reset=False):
        is_grp = False

    file.seek(initial_pos, os.SEEK_SET)

    return is_grp


def is_object(file: BinaryIO) -> bool:
    initial_pos = file.tell()

    is_obj = True

    if not is_value(file, 1, 0x28, reset=False):
        is_obj = False

    if not is_value(file, 2, 0x1514, reset=False):
        is_obj = False

    _ = read_string(file)

    if not is_value(file, 2, 0x0116, reset=False):
        is_obj = False

    if not is_value(file, 3, 0, reset=False):
        is_obj = False

    if not is_value(file, 2, 0x1817, reset=False):
        is_obj = False

    file.seek(initial_pos, os.SEEK_SET)

    return is_obj


def is_value(
    file: BinaryIO, 
    length: int, 
    value: Any, 
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


def convert_object(vertices: list[tuple[int]], faces: list[tuple[int]], output_path: str) -> None:
    with open(output_path, 'w') as file:
        file.write("g test\n")

        for (x, y, z) in vertices:
            file.write(f"v {x} {y} {z}\n")

        for (v1, v2, v3) in faces:
            file.write(f"f {v1 + 1} {v2 + 1} {v3 + 1}\n")


def show_object(path: str) -> None:
    mesh = vedo.Mesh(path)
    mesh.show()


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


if __name__ == "__main__":
    main()