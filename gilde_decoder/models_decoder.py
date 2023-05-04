"""Module for decoding and converting the models of the game."""
import fnmatch
import os
import shutil
import struct
import tkinter as tk
import zipfile
from dataclasses import dataclass
from pathlib import Path
from tkinter import filedialog
from typing import BinaryIO, Optional

import vedo
from tap import Tap

from gilde_decoder.const import (
    MODELS_EXCLUDE_PATHS,
    MODELS_REDUCED_FOOTER_FILES,
    MODELS_STRING_CODEC,
)
from gilde_decoder.helpers import (
    find_address_of_byte_pattern,
    is_value,
    read_string,
    sanitize_filename,
    skip_optional,
    skip_required,
    skip_until,
    skip_zero,
    subtract_path,
)
from gilde_decoder.logger import logger


@dataclass
class BgfHeader:
    """Class representing the header of a bgf file."""

    name: str
    mapping_address: int
    num1: int
    num2: int
    anim_count: int
    texture_count: int

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfHeader":
        """Reads the header from a bgf file."""

        bgf_header = cls.__new__(cls)

        bgf_header.name = read_string(file)

        skip_required(file, b"\x2E", 1)
        bgf_header.mapping_address = int.from_bytes(
            file.read(4), byteorder="little", signed=False
        )

        skip_required(file, b"\x01\x01", 2)
        bgf_header.num1 = int.from_bytes(file.read(1), byteorder="little", signed=False)

        skip_required(file, b"\xCD\xAB\x02", 3)
        bgf_header.num2 = int.from_bytes(file.read(1), byteorder="little", signed=False)

        has_anim = skip_optional(file, b"\x37", 1)
        if has_anim:
            bgf_header.anim_count = int.from_bytes(
                file.read(2), byteorder="little", signed=False
            )
            skip_zero(file, 2)

        skip_required(file, b"\x03\x04", 2)
        bgf_header.texture_count = int.from_bytes(
            file.read(2), byteorder="little", signed=False
        )
        skip_zero(file, 2)

        return bgf_header


@dataclass
class BgfTexture:
    """Class representing a texture in a bgf file."""

    id: int
    name: str
    appendix: str = ""
    appendix_type: int = 0

    @classmethod
    def is_texture(cls, file: BinaryIO) -> bool:
        """Checks if the current position in the file is a texture."""

        initial_pos = file.tell()

        is_texture = True

        if not is_value(file, 2, 0x0605, reset=False):
            is_texture = False

        file.seek(initial_pos, os.SEEK_SET)

        return is_texture

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfTexture":
        """Reads a texture from a bgf file."""

        bgf_texture = cls.__new__(cls)

        skip_required(file, b"\x05\x06", 2)
        bgf_texture.id = int.from_bytes(file.read(2), byteorder="little", signed=False)
        skip_zero(file, 2)

        skip_optional(file, b"\x07", 1)
        skip_optional(file, b"\x08", 1)
        bgf_texture.name = read_string(file)

        has_appendix_1 = skip_optional(file, b"\x08", 1)
        has_appendix_2 = skip_optional(file, b"\x09", 1)

        if has_appendix_1 or has_appendix_2:
            bgf_texture.appendix = read_string(file)
            bgf_texture.appendix_type = 1 if has_appendix_1 else 2

        skip_until(file, 0x28, 1)

        return bgf_texture


@dataclass
class Vertex:
    """Class representing a vertex in a bgf file."""

    x: float
    y: float
    z: float

    @classmethod
    def from_file(cls, file: BinaryIO) -> "Vertex":
        """Reads a vertex from a bgf file."""

        vertex = cls.__new__(cls)

        vertex.x = struct.unpack("<f", file.read(4))[0]
        vertex.y = struct.unpack("<f", file.read(4))[0]
        vertex.z = struct.unpack("<f", file.read(4))[0]

        return vertex


@dataclass
class Face:
    """Class representing a face in a bgf file."""

    a: int
    b: int
    c: int

    @classmethod
    def from_file(cls, file: BinaryIO) -> "Face":
        """Reads a face from a bgf file."""

        face = cls.__new__(cls)

        face.a = int.from_bytes(file.read(4), byteorder="little", signed=False)
        face.b = int.from_bytes(file.read(4), byteorder="little", signed=False)
        face.c = int.from_bytes(file.read(4), byteorder="little", signed=False)

        return face


@dataclass
class Polygon:
    """Class representing a polygon in a bgf file."""

    face: Face
    vertex_x: Vertex
    vertex_y: Vertex
    vertex_n: Vertex
    normal: Vertex
    texture_index: int

    @classmethod
    def from_file(cls, file: BinaryIO) -> "Polygon":
        """Reads a polygon from a bgf file."""

        polygon = cls.__new__(cls)

        polygon.face = Face.from_file(file)

        skip_optional(file, b"\x1E", 1)

        polygon.vertex_x = Vertex.from_file(file)
        polygon.vertex_y = Vertex.from_file(file)
        polygon.vertex_n = Vertex.from_file(file)

        has_normal = skip_optional(file, b"\x1F", 1)

        if has_normal:
            polygon.normal = Vertex.from_file(file)

            has_texture = skip_optional(file, b"\x20", 1)

            if has_texture:
                polygon.texture_index = int.from_bytes(
                    file.read(1), byteorder="little", signed=False
                )

            skip_optional(file, b"\x1D", 1)

        return polygon


@dataclass
class BgfModel:
    """Class representing a model in a bgf file."""

    vertex_count: int
    polygon_count: int
    vertices: list[Vertex]
    polygons: list[Polygon]

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfModel":
        """Reads a model from a bgf file."""

        bgf_model = cls.__new__(cls)

        skip_required(file, b"\x19", 1)

        bgf_model.vertex_count = int.from_bytes(
            file.read(2), byteorder="little", signed=False
        )

        skip_zero(file, 2)

        skip_required(file, b"\x1A", 1)

        bgf_model.polygon_count = int.from_bytes(
            file.read(2), byteorder="little", signed=False
        )

        skip_zero(file, 2)

        skip_required(file, b"\x1B", 1)

        bgf_model.vertices = []
        for _ in range(bgf_model.vertex_count):
            vertex = Vertex.from_file(file)
            bgf_model.vertices.append(vertex)

        skip_required(file, b"\x1C\x1D", 2)

        bgf_model.polygons = []
        for _ in range(bgf_model.polygon_count):
            polygon = Polygon.from_file(file)
            bgf_model.polygons.append(polygon)

        return bgf_model


@dataclass
class BgfAnimData:
    """Class representing an animation data in a bgf file."""

    name: str
    vertex1: Vertex
    val: int
    vertex2: Vertex

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfAnimData":
        """Reads an animation data from a bgf file."""

        bgf_anim_data = cls.__new__(cls)

        skip_required(file, b"\x38", 1)

        bgf_anim_data.name = read_string(file)

        skip_required(file, b"\x39", 1)

        bgf_anim_data.vertex1 = Vertex.from_file(file)
        bgf_anim_data.val = int.from_bytes(
            file.read(1), byteorder="little", signed=False
        )
        bgf_anim_data.vertex2 = Vertex.from_file(file)

        return bgf_anim_data


@dataclass
class BgfGameObject:
    """Class representing a game object in a bgf file."""

    name: str
    anim_count: int
    bgf_anim_datas: list[BgfAnimData]
    bgf_model: Optional[BgfModel] = None

    @classmethod
    def is_game_object(cls, file: BinaryIO) -> bool:
        """Checks if the current position in the file is a game object."""

        initial_pos = file.tell()

        is_obj = True

        if is_value(file, 1, 0x28, reset=True):
            file.seek(1, os.SEEK_CUR)

        if not is_value(file, 2, 0x1514, reset=False):
            is_obj = False

        file.seek(initial_pos, os.SEEK_SET)

        return is_obj

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfGameObject":
        """Reads a game object from a bgf file."""

        bgf_game_object = cls.__new__(cls)

        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x14\x15", 2)

        bgf_game_object.name = read_string(file)

        skip_optional(file, b"\x16\x01", 5)
        has_model = skip_optional(file, b"\x17\x18", 6)

        if has_model:
            bgf_game_object.bgf_model = BgfModel.from_file(file)

        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x28", 1)
        has_anim_data = skip_optional(file, b"\x37", 1)

        bgf_game_object.anim_count = 0

        bgf_game_object.bgf_anim_datas = []
        if has_anim_data:
            bgf_game_object.anim_count = int.from_bytes(
                file.read(2), byteorder="little", signed=False
            )

            skip_zero(file, 2)

            for _ in range(bgf_game_object.anim_count):
                bgf_anim_data = BgfAnimData.from_file(file)
                bgf_game_object.bgf_anim_datas.append(bgf_anim_data)

        return bgf_game_object


@dataclass
class VertexMapping:
    """Class representing a vertex mapping in a bgf file."""

    vertex1: Vertex
    vertex2: Vertex

    @classmethod
    def from_file(cls, file: BinaryIO) -> "VertexMapping":
        """Reads a vertex mapping from a bgf file."""

        vertex_mapping = cls.__new__(cls)

        vertex_mapping.vertex1 = Vertex.from_file(file)
        vertex_mapping.vertex2 = Vertex.from_file(file)

        return vertex_mapping


@dataclass
class PolygonMapping:
    """Class representing a polygon mapping in a bgf file."""

    face: Face
    v1: Vertex
    v2: Vertex
    v3: Vertex
    texture_index: int

    @classmethod
    def from_file(cls, file: BinaryIO) -> "PolygonMapping":
        """Reads a polygon mapping from a bgf file."""

        polygon_mapping = cls.__new__(cls)

        polygon_mapping.face = Face.from_file(file)
        polygon_mapping.v1 = Vertex.from_file(file)
        polygon_mapping.v2 = Vertex.from_file(file)
        polygon_mapping.v3 = Vertex.from_file(file)
        polygon_mapping.texture_index = int.from_bytes(
            file.read(1), byteorder="little", signed=False
        )

        return polygon_mapping


@dataclass
class BgfMappingObject:
    """Class representing a mapping object in a bgf file."""

    num1: int
    num2: int
    num3: int
    num4: int
    vertex_mapping_count: int
    polygon_mapping_count: int
    vertex_mappings: list[VertexMapping]
    box_vertex_mappings: list[VertexMapping]
    some_float: float
    polygon_mappings: list[PolygonMapping]

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfMappingObject":
        """Reads a mapping object from a bgf file."""

        bgf_mapping_object = cls.__new__(cls)

        skip_required(file, b"\x2F\x2D", 2)

        bgf_mapping_object.num1 = int.from_bytes(
            file.read(1), byteorder="little", signed=False
        )

        bgf_mapping_object.num2 = int.from_bytes(
            file.read(2), byteorder="little", signed=False
        )

        _ = int.from_bytes(file.read(1), byteorder="little", signed=False)

        bgf_mapping_object.num3 = int.from_bytes(
            file.read(2), byteorder="little", signed=False
        )

        skip_required(file, b"\xB5\xFA", 2)

        bgf_mapping_object.num4 = int.from_bytes(
            file.read(4), byteorder="little", signed=False
        )

        bgf_mapping_object.vertex_mapping_count = int.from_bytes(
            file.read(4), byteorder="little", signed=False
        )

        bgf_mapping_object.polygon_mapping_count = int.from_bytes(
            file.read(4), byteorder="little", signed=False
        )

        bgf_mapping_object.vertex_mappings = []
        for _ in range(bgf_mapping_object.vertex_mapping_count):
            vertex_mapping = VertexMapping.from_file(file)
            bgf_mapping_object.vertex_mappings.append(vertex_mapping)

        bgf_mapping_object.box_vertex_mappings = []
        for _ in range(8):
            box_vertex_mapping = VertexMapping.from_file(file)
            bgf_mapping_object.box_vertex_mappings.append(box_vertex_mapping)

        bgf_mapping_object.some_float = struct.unpack("<f", file.read(4))[0]

        bgf_mapping_object.polygon_mappings = []
        for _ in range(bgf_mapping_object.polygon_mapping_count):
            polygon_mapping = PolygonMapping.from_file(file)
            bgf_mapping_object.polygon_mappings.append(polygon_mapping)

        return bgf_mapping_object


@dataclass
class Bgf:
    """Class representing a bgf file."""

    path: str
    bgf_header: BgfHeader
    bgf_textures: list[BgfTexture]
    bgf_game_objects: list[BgfGameObject]
    bgf_mapping_object: BgfMappingObject

    def validate_footer(self, file: BinaryIO) -> bool:
        """Validates the footer of a bgf file."""

        initial_position = file.tell()
        footer = file.read()

        literal_count = 0
        footer_texture_count = 0
        footer_anim_count = 0

        texture_bytes_found = []
        for bgf_texture in self.bgf_textures:
            texture_name = bgf_texture.name.split(".")[0]

            texture_name_bytes = texture_name.encode(MODELS_STRING_CODEC)

            if bgf_texture.appendix:
                appendix = bgf_texture.appendix.split(".")[0]
                appendix_bytes = appendix.encode(MODELS_STRING_CODEC)

                if bgf_texture.appendix_type == 1:
                    texture_name_bytes += b"\x00" + appendix_bytes
                elif bgf_texture.appendix_type == 2:
                    texture_name_bytes += b"\x00\x00" + appendix_bytes
                else:
                    raise ValueError("Unknown appendix type")

                texture_name += appendix

            if texture_name_bytes in texture_bytes_found:
                continue

            relative_positions = find_address_of_byte_pattern(
                texture_name_bytes, footer
            )

            if len(relative_positions) == 0:
                continue

            texture_bytes_found.append(texture_name_bytes)
            literal_count += len(texture_name * len(relative_positions))
            footer_texture_count += len(relative_positions)

        file.seek(initial_position + footer_texture_count * 9 + literal_count + 4)

        game_objects_with_anim_data = [
            bgf_game_object
            for bgf_game_object in self.bgf_game_objects
            if len(bgf_game_object.bgf_anim_datas) > 0
        ]
        anim_literal_count = 0
        footer_anim_count = 0
        for bgf_game_object in game_objects_with_anim_data:
            for _ in bgf_game_object.bgf_anim_datas:
                value = read_string(file)
                file.seek(24, os.SEEK_CUR)
                anim_literal_count += len(value)
                footer_anim_count += 1

        expected_non_literal_count = (
            footer_texture_count * 9 + footer_anim_count * 25 + 5
        )

        is_reduced_footer_length = any(
            self.path.endswith(reduced_footer_files)
            for reduced_footer_files in MODELS_REDUCED_FOOTER_FILES
        )
        if is_reduced_footer_length:
            expected_non_literal_count -= 4

        non_literal_count = len(footer) - literal_count - anim_literal_count

        is_valid = non_literal_count == expected_non_literal_count

        if not is_valid:
            print(
                f"Got {non_literal_count} non-literal bytes instead of {expected_non_literal_count}"
            )

        return is_valid

    @classmethod
    def from_file(cls, bgf_path: str) -> "Bgf":
        """Reads the bgf file."""

        logger.info(f"Decoding {bgf_path}")

        bgf = cls.__new__(cls)
        bgf.path = bgf_path

        if not os.path.exists(bgf_path):
            raise ValueError(f"File {bgf_path} does not exist.")

        with open(bgf_path, "rb") as file:
            bgf.bgf_header = BgfHeader.from_file(file)

            bgf.bgf_textures = []
            while BgfTexture.is_texture(file):
                bgf_texture = BgfTexture.from_file(file)
                bgf.bgf_textures.append(bgf_texture)

            bgf.bgf_game_objects = []
            while BgfGameObject.is_game_object(file):
                bgf_game_object = BgfGameObject.from_file(file)
                bgf.bgf_game_objects.append(bgf_game_object)

            bgf.bgf_mapping_object = BgfMappingObject.from_file(file)

            assert bgf.validate_footer(file)

        return bgf


class ModelsArgumentParser(Tap):
    """Argument parser for the models_decoder module."""

    input: str
    output: str
    texture_search_path: Optional[Path]

    def configure(self) -> None:
        """Configure the argument parser."""

        self.add_argument(
            "-i",
            "--input",
            help="Input path. Can either be the objects.bin file, in which case all bgf "
            "files will be extracted and decoded or a single bgf file, in which case "
            "only this file will be decoded.",
            default="",
        )
        self.add_argument(
            "-o",
            "--output",
            help="Output path where the extracted obj files will be stored. If not "
            "specified, the output folder will be created in the current working "
            "directory.",
            default=os.path.join(os.getcwd(), "output"),
        )
        self.add_argument(
            "-t",
            "--texture-search-path",
            help="Path where the textures will be searched. If given, textures "
                 "referenced in the bgf file will be copied next to the obj/mtl "
                 "files so that 3D viewers can find them.",
            type=self._path_converter,
            default=None,
        )

    @staticmethod
    def _path_converter(path: str) -> Path:
        return Path(path)


def main() -> None:
    """Main function of the models_decoder module."""

    parser = ModelsArgumentParser()
    args = parser.parse_args()

    if not args.input:
        root = tk.Tk()
        root.withdraw()
        args.input = filedialog.askopenfilename(
            title="open objects.bin or bgf file",
            filetypes=[("objects.bin", "*.bin"), ("bgf", "*.bgf")],
            initialfile="objects.bin",
        )
        root.destroy()
        args.input = args.input.replace("/", "\\")

    if not os.path.exists(args.input):
        raise ValueError(f"Input path {args.input} does not exist.")

    if Path(args.input).name == "objects.bin":
        bgf_paths = extract_bgfs(args.input, args.output)
        bgfs = decode_bgfs(bgf_paths)
    else:
        bgfs = [Bgf.from_file(args.input)]
    for bgf in bgfs:
        convert_bgf(bgf, args.output, args.texture_search_path)


def extract_bgfs(bin_path: str, output_path: str) -> list[str]:
    """Extracts all bgf files from the objects.bin file.
    @bin_path Path to the objects.bin file
    @output_path Output path where the bgf directory will be created
    @return List of extracted bgf file paths
    """

    bgf_base_path = os.path.join(output_path, "bgf")

    if not os.path.exists(bgf_base_path):
        os.makedirs(bgf_base_path)

    with zipfile.ZipFile(bin_path, "r") as zip_file:
        zip_file.extractall(bgf_base_path)

    bgf_paths: list[str] = []
    for root, _, files in os.walk(bgf_base_path):
        for filename in fnmatch.filter(files, "*.bgf"):
            filepath = os.path.join(root, filename)
            if any(
                filepath.endswith(exclude_path) for exclude_path in MODELS_EXCLUDE_PATHS
            ):
                continue
            bgf_paths.append(filepath)

    return bgf_paths


def decode_bgfs(bgf_paths: list[str]) -> list[Bgf]:
    """Decodes a list of bgf files and converts them to wavefront obj files.
    @file_paths List of bgf file paths
    @return List of decoded bgf dicts
    """

    bgfs: list[Bgf] = []

    for bgf_path in bgf_paths:
        bgf = Bgf.from_file(bgf_path)
        bgfs.append(bgf)

    return bgfs


def convert_bgf(bgf: Bgf, output_path: str, texture_search_path: Optional[Path]) -> None:
    """Converts a bgf dict to a wavefront obj file.
    @bgf Bgf object
    @output_path Output path where the obj directory will be created
    @texture_search_path Path where the textures will be searched. If given, textures will be copied next to the obj/mtl
    files so that 3D viewers can find them.
    """

    try:
        logger.info(f"Converting {bgf.path}")

        obj_base_path = os.path.join(output_path, "obj")
        relative_path = subtract_path(output_path, bgf.path)
        obj_path_str = os.path.join(obj_base_path, relative_path)
        obj_path = Path(os.path.splitext(obj_path_str)[0])

        if not os.path.exists(obj_path):
            os.makedirs(obj_path)

        vertices = [
            vertex_mapping.vertex1
            for vertex_mapping in bgf.bgf_mapping_object.vertex_mappings
        ]
        faces = [
            polygon_mapping.face
            for polygon_mapping in bgf.bgf_mapping_object.polygon_mappings
        ]
        normals = [
            polygon_mapping.v3
            for polygon_mapping in bgf.bgf_mapping_object.polygon_mappings
        ]
        num_models = len([x for x in bgf.bgf_game_objects if x.bgf_model is not None])
        if num_models > 1:
            logger.warning(
                f"Found {num_models} models in {bgf.path}, expected 1. "
                + "Converting only the first model."
            )

        # Some bgf files have multiple game objects, of which not all are models.
        bgf_game_object = next(
            x for x in bgf.bgf_game_objects if x.bgf_model is not None
        )
        if bgf_game_object.bgf_model is None:
            raise ValueError("No model found in bgf file.")

        polygons = bgf_game_object.bgf_model.polygons

        texture_mappings: list[tuple[Vertex, Vertex, Vertex]] = [
            (
                Vertex(polygon.vertex_x.x, polygon.vertex_y.x, polygon.vertex_n.x),
                Vertex(polygon.vertex_x.y, polygon.vertex_y.y, polygon.vertex_n.y),
                Vertex(polygon.vertex_x.z, polygon.vertex_y.z, polygon.vertex_n.z),
            )
            for polygon in polygons
        ]

        texture_indices = [polygon.texture_index for polygon in polygons]
        materials = [
            material_name_from_texture_file_name(bgf.bgf_textures[idx].name)
            for idx in texture_indices
        ]

        # Get filename from bgf path so every obj/mtl file has the same name as the bgf file
        filename = os.path.splitext(os.path.basename(bgf.path))[0]
        obj_file_name = sanitize_filename(f"{filename}.obj")
        mtl_file_name = sanitize_filename(f"{filename}.mtl")
        obj_file_path = os.path.join(obj_path, obj_file_name)
        mtl_file_path = os.path.join(obj_path, mtl_file_name)

        convert_object(
            vertices,
            faces,
            normals,
            texture_mappings,
            obj_file_path,
            mtl_file_name,
            materials,
        )
        write_mtl(bgf.bgf_textures, mtl_file_path)
        if texture_search_path is not None:
            copy_textures(bgf.bgf_textures, texture_search_path, obj_path)
    except IndexError as e:
        logger.error(f"Failed to convert {bgf.path}: {e}")


# Footer structs


def decode_anim_footer(file: BinaryIO) -> dict:
    """Decodes the animation part of the bgf file footer."""

    anim_footer = {}

    anim_footer["name"] = read_string(file)

    file.seek(27, os.SEEK_CUR)

    return anim_footer


# Obj Conversion


def convert_object(
    vertices: list[Vertex],
    faces: list[Face],
    normals: list[Vertex],
    texture_mappings: list[tuple[Vertex, Vertex, Vertex]],
    output_path: str,
    mtl_file_name: str,
    materials: list[str],
) -> None:
    """Converts object data from a bgf file to an obj file."""

    if len(faces) != len(normals):
        logger.warning(
            "Number of faces and normals don't match, aborting obj conversion"
        )
        return

    if len(faces) != len(materials):
        logger.warning(
            "Number of faces and materials don't match, aborting obj conversion"
        )
        return

    with open(output_path, "w", encoding="utf-8") as file:
        file.write("g test\n")
        file.write(f"mtllib {mtl_file_name}\n")

        for vertex in vertices:
            file.write(f"v {vertex.x} {vertex.y} {vertex.z}\n")

        for normal in normals:
            file.write(f"vn {normal.x} {normal.y} {normal.z}\n")

        for texture_mapping in texture_mappings:
            for vertex in texture_mapping:
                # Since this is a 2D texture, the z coordinate (w) is always 0
                file.write(f"vt {vertex.x} {vertex.y} 0\n")

        # Write faces (their vertices, their texture coordinates and their normals)
        for i, (face, material) in enumerate(zip(faces, materials)):
            file.write(f"usemtl {material}\n")
            file.write(
                f"f {face.a + 1}/{i * 3 + 1}/{i + 1} "
                + f"{face.b + 1}/{i * 3 + 2}/{i + 1} "
                + f"{face.c + 1}/{i * 3 + 3}/{i + 1}\n"
            )


def material_name_from_texture_file_name(texture_file_name: str) -> str:
    """Returns the material name from a texture file name."""

    material_name = texture_file_name.split(".")[0]
    return material_name


def write_mtl(bgf_textures: list[BgfTexture], output_path: str) -> None:
    """Writes the mtl file for the obj file."""

    with open(output_path, "w", encoding="utf-8") as file:
        for bgf_texture in bgf_textures:
            material_name = material_name_from_texture_file_name(bgf_texture.name)
            file.write(f"newmtl {material_name}\n")
            file.write("Ka 1.0 1.0 1.0\n")
            file.write("Kd 1.0 1.0 1.0\n")
            file.write("Ks 0.0 0.0 0.0\n")
            file.write(f"map_Kd {bgf_texture.name}\n")


def copy_textures(bgf_textures: list[BgfTexture], texture_search_path: Path, output_path: Path) -> None:
    """
    Copies the textures from the bgf file to the obj file directory for the 3d file viewer.
    @param bgf_textures: The decoded textures from the bgf file.
    @param texture_search_path: The path to search for the textures. Should contain the content of the textures.bin file.
    @param output_path: The path to copy the textures to. Should be the same as the obj file path since 3D viewers search for textures there.
    """

    def either(c):
        return '[%s%s]' % (c.lower(), c.upper()) if c.isalpha() else c

    texture_file_names = [tex.name for tex in bgf_textures]
    case_insensitive_file_names = ["".join(map(either, texname)) for texname in texture_file_names]
    texture_files_nested = [list(texture_search_path.rglob(texname)) for texname in case_insensitive_file_names]
    # Flatten lists return by rglob
    texture_files = [item for sublist in texture_files_nested for item in sublist]
    if len(texture_files) != len(bgf_textures):
        logger.warning("Amount of texture files found differs from amount specified specified in bgf file")
    for tf in texture_files:
        shutil.copy(tf, output_path)


def show_object(path: str) -> None:
    """Shows an object in a 3D viewer."""

    mesh = vedo.Mesh(path)
    mesh.show()


if __name__ == "__main__":
    main()
