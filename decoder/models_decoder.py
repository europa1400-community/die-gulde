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
            bgf_faces = obj["faces"]
            faces = [(face["a"], face["b"], face["c"]) for face in bgf_faces]
            texture = [(face["vertex1"], face["vertex2"], face["vertex3"]) for face in bgf_faces]
            normal = [face["vertex4"] for face in bgf_faces]
            obj_file_name = sanitize_filename(f'{obj["name"]}.obj')
            obj_file_path = os.path.join(obj_path, obj_file_name)
            convert_object(vertices, faces, texture, normal, obj_file_path)
    

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
                # print("Decoding group...")
                groups.append(decode_group(file))
                # print(f"Decoded group '{groups[-1]['name']}'")
            elif is_object(file):
                # print("Decoding object...")
                objects.append(decode_object(file))
                # print(f"Decoded object '{objects[-1]['name']}'")
            else:
                break
        bgf["objects"] = objects
        bgf["groups"] = groups
    
    return bgf


def decode_texture(file: BinaryIO) -> dict:
    assert is_value(file, 2, 0x0605, reset=False)

    id = int.from_bytes(file.read(1), byteorder='little', signed=False)

    assert is_value(file, 3, 0, reset=False)
    
    type = int.from_bytes(file.read(1), byteorder='little', signed=False)
    name = read_string(file)

    while not is_value(file, 1, 0x28, reset=False):
        pass

    return {
        'id': id,
        'type': type,
        'name': name,
    }


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
    face_count = int.from_bytes(file.read(2), byteorder='little', signed=False)
    assert is_value(file, 2, 0, reset=False)

    assert is_value(file, 1, 0x1B, reset=False)

    vertices = []
    for _ in range(vertex_count):
        vertices.append(decode_vertex(file))
    obj["vertices"] = vertices

    assert is_value(file, 1, 0x1C, reset=False)
    assert is_value(file, 1, 0x1D, reset=False)

    faces = []
    for _ in range(face_count):
        faces.append(decode_face(file))
    obj["faces"] = faces

    assert is_value(file, 2, 0x2828, reset=False)

    return obj


def decode_vertex(file: BinaryIO) -> tuple:
    x = struct.unpack('<f', file.read(4))[0]
    y = struct.unpack('<f', file.read(4))[0]
    z = struct.unpack('<f', file.read(4))[0]
    return (x, y, z)


def decode_face(file: BinaryIO) -> dict:
    face = {}

    face["a"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    face["b"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    face["c"] = int.from_bytes(file.read(4), byteorder='little', signed=False)
    
    if is_value(file, 1, 0x1E, reset=True):
        file.seek(1, os.SEEK_CUR)

    face["vertex1"] = decode_vertex(file)
    face["vertex2"] = decode_vertex(file)
    face["vertex3"] = decode_vertex(file)

    if is_value(file, 1, 0x1F, reset=True):
        file.seek(1, os.SEEK_CUR)
        face["vertex4"] = decode_vertex(file)

        assert is_value(file, 1, 0x20, reset=False)

        face["num1"] = int.from_bytes(file.read(1), byteorder='little', signed=False)

        if is_value(file, 1, 0x1D, reset=True):
            file.seek(1, os.SEEK_CUR)

    return face


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

    if not is_object(file):
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


def convert_object(vertices: list[tuple[int]], faces: list[tuple[int]], textures: list[tuple[int]], normals: tuple[int], output_path: str) -> None:
    with open(output_path, 'w') as file:
        # file.write("mtllib kaefig.mtl\n")
        file.write("g test\n")

        for (x, y, z) in vertices:
            file.write(f"v {x} {y} {z}\n")

        for (t1, t2, t3) in textures:
            file.write(f"vt {float_to_str(t1[0])} {float_to_str(t1[1])} {float_to_str(t1[2])}\n")
            file.write(f"vt {float_to_str(t2[0])} {float_to_str(t2[1])} {float_to_str(t2[2])}\n")
            file.write(f"vt {float_to_str(t3[0])} {float_to_str(t3[1])} {float_to_str(t3[2])}\n")

        for n in normals:
            file.write(f"vn {float_to_str(n[0])} {float_to_str(n[1])} {float_to_str(n[2])}\n")

        # file.write("usemtl kaefig\n")

        ti = 1
        ni = 1
        for (v1, v2, v3) in faces:
            # file.write(f"f {v3 + 1}//{ti+2}//{ni} {v2 + 1}//{ti+1}//{ni} {v1 + 1}//{ti}//{ni}\n")
            file.write(f"f {v3 + 1}//{ni} {v2 + 1}//{ni} {v1 + 1}//{ni}\n")
            # file.write(f"f {v3 + 1} {v2 + 1} {v1 + 1}\n")
            ti = ti + 3
            ni = ni + 1


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


# Using this helper function, to write the numbers without scientific notation with "e"
# https://stackoverflow.com/a/38983595
def float_to_str(f):
    float_string = repr(f)
    if 'e' in float_string:  # detect scientific notation
        digits, exp = float_string.split('e')
        digits = digits.replace('.', '').replace('-', '')
        exp = int(exp)
        zero_padding = '0' * (abs(int(exp)) - 1)  # minus 1 for decimal point in the sci notation
        sign = '-' if f < 0 else ''
        if exp > 0:
            float_string = '{}{}{}.0'.format(sign, digits, zero_padding)
        else:
            float_string = '{}0.{}{}'.format(sign, zero_padding, digits)
    return float_string


if __name__ == "__main__":
    main()