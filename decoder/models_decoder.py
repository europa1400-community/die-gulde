import fnmatch
import os
import glob
import struct
import tkinter as tk
from tkinter import filedialog
import argparse
from typing import BinaryIO
import zipfile
import trimesh
import vedo


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
    
    bgf_path = os.path.join(args.output, "bgf")

    if not os.path.exists(bgf_path):
        os.makedirs(bgf_path)

    with zipfile.ZipFile(objects_bin_path, 'r') as zip_ref:
        zip_ref.extractall(bgf_path)

    kaefig_bgf_path = os.path.join(bgf_path, "Accessoires", "Deko", "kaefig.bgf")

    decode_bgf(kaefig_bgf_path, args.output)

    # file_paths = []
    # for root, directories, files in os.walk(bgf_path):
    #     for filename in fnmatch.filter(files, '*.bgf'):
    #         filepath = os.path.join(root, filename)
    #         file_paths.append(filepath)

    # analysis_hex_path = os.path.join(args.output, 'bgf_header_analysis_hex.txt')
    # analysis_dec_path = os.path.join(args.output, 'bgf_header_analysis_dec.txt')

    # if os.path.exists(analysis_hex_path):
    #     os.remove(analysis_hex_path)
    # if os.path.exists(analysis_dec_path):
    #     os.remove(analysis_dec_path)

    # for bgf_path in file_paths:
    #     decode_bgf(bgf_path, args.output)


def decode_bgf(input_path: str, output_path: str):
    with open(input_path, 'rb') as file:
        bgf_header = BgfHeader.from_file(file)
        texture_header = file.read(50)
        object_header = file.read(28)

        vertices = []
        for _ in range(24):
            vertex = Vertex.from_file(file)
            vertices.append(vertex)

        file.seek(2, os.SEEK_CUR)

        polygons = []
        for _ in range(10):
            polygon = Polygon.from_file(file)
            polygons.append(polygon)

        file.seek(29, os.SEEK_CUR)

        file.seek(bgf_header.addr1, os.SEEK_SET)

        vertices2 = []
        for _ in range(32):
            vertex = Vertex.from_file(file)
            vertices2.append(vertex)

        file.seek(4, os.SEEK_CUR)

        assert file.tell() == 0x5B8

        special_polygons = []
        for _ in range(10):
            special_polygon = SpecialPolygon.from_file(file)
            special_polygons.append(special_polygon)

        footer = file.read(20)

        assert file.tell() == 0x7B6

    object_path = os.path.join(output_path, 'object.obj')
    special_object_path = os.path.join(output_path, 'special_object.obj')

    # convert_object(vertices=[vertex.as_tuple for vertex in vertices], faces=[polygon.faces for polygon in polygons], output_path=object_path)
    # show_object(object_path)
    # convert_object(
    #     vertices=[vertex.as_tuple for vertex in vertices2], 
    #     faces=[polygon.faces for polygon in special_polygons], 
    #     output_path=special_object_path
    # )
    # show_object(special_object_path)


class BgfHeader:
    def __init__(self, addr1: int, data: bytes):
        self.__addr1 = addr1
        self.data = data

    @property
    def addr1(self):
        return self.size() + self.__addr1 + 1

    @classmethod
    def from_bytes(cls, data: bytes):
        return cls(
            struct.unpack('<I', data[5:9])[0],
            data
        )
    
    @classmethod
    def from_file(cls, file: BinaryIO):
        return cls.from_bytes(file.read(cls.size()))
    
    @classmethod
    def size(self):
        return 29


class Vertex:
    def __init__(self, x: float, y: float, z: float):
        self.x = x
        self.y = y
        self.z = z

    @property
    def as_tuple(self) -> tuple[float]:
        return (self.x, self.y, self.z)

    @classmethod
    def from_bytes(cls, data: bytes):
        return cls(
            struct.unpack('<f', data[0:4])[0],
            struct.unpack('<f', data[4:8])[0],
            struct.unpack('<f', data[8:12])[0],
        )
    
    @classmethod
    def from_file(cls, file: BinaryIO):
        return cls.from_bytes(file.read(cls.size()))
    
    @classmethod
    def size(self):
        return 12


class Polygon:
    def __init__(self, faces: tuple[int], vertices: list[Vertex], footer: bytes):
        if len(faces) != 3:
            raise ValueError('Polygon must have exactly 3 faces')
        
        if len(vertices) != 3:
            raise ValueError('Polygon must have exactly 3 vertices')
        
        self.vertices = vertices
        self.faces = faces
        self.footer = footer

    @classmethod
    def from_bytes(cls, data: bytes):
        return cls(
            (
                struct.unpack('<I', data[0:4])[0],
                struct.unpack('<I', data[4:8])[0],
                struct.unpack('<I', data[8:12])[0],
            ),
            [
                Vertex.from_bytes(data[13:25]),
                Vertex.from_bytes(data[25:37]),
                Vertex.from_bytes(data[37:49]),
            ],
            data[49:],
        )
    
    @classmethod
    def from_file(cls, file: BinaryIO):
        return cls.from_bytes(file.read(cls.size()))
    
    @classmethod
    def size(self):
        return 65


class SpecialPolygon:
    def __init__(self, faces: tuple[int], vertices: list[Vertex]):
        if len(faces) != 3:
            raise ValueError('Polygon must have exactly 3 indices')
        
        if len(vertices) != 3:
            raise ValueError('Polygon must have exactly 3 vertices')
        
        self.vertices = vertices
        self.faces = faces

    @classmethod
    def from_bytes(cls, data: bytes):
        return cls(
            (
                struct.unpack('<I', data[0:4])[0],
                struct.unpack('<I', data[4:8])[0],
                struct.unpack('<I', data[8:12])[0],
            ),
            [
                Vertex.from_bytes(data[12:24]),
                Vertex.from_bytes(data[24:36]),
                Vertex.from_bytes(data[36:48]),
            ],
        )
    
    @classmethod
    def from_file(cls, file: BinaryIO):
        return cls.from_bytes(file.read(cls.size()))
    
    @classmethod
    def size(self):
        return 49


class Bgf:
    def __init__(
        self,
        header: BgfHeader,
    ):
        self.header = header

    @classmethod
    def from_bytes(cls, data: bytes):
        return cls(
            BgfHeader.from_bytes(data[:BgfHeader.size()]),
        )

    @classmethod
    def from_file(cls, file: BinaryIO):
        return cls.from_bytes(file.read())


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

    
if __name__ == "__main__":
    main()
