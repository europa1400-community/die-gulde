"""Module containing the BgfModel class."""

from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.data.bgf.bgf_polygon import BgfPolygon
from gilde_decoder.data.bgf.bgf_texture_mapping import BgfTextureMapping
from gilde_decoder.data.face import Face
from gilde_decoder.data.vertex import Vertex
from gilde_decoder.helpers import skip_required, skip_zero


@dataclass
class BgfModel:
    """Class representing a model in a bgf file."""

    vertex_count: int
    polygon_count: int
    vertices: list[Vertex]
    polygons: list[BgfPolygon]

    @property
    def faces(self) -> list[Face]:
        """Returns the faces of the model."""

        return [polygon.face for polygon in self.polygons]

    @property
    def normals(self) -> list[Vertex]:
        """Returns the normals of the model."""

        return [polygon.normal for polygon in self.polygons]

    @property
    def texture_mappings(self) -> list[BgfTextureMapping]:
        """Returns the texture mappings of the model."""

        return [polygon.texture_mapping for polygon in self.polygons]

    @property
    def texture_indices(self) -> list[int]:
        """Returns the texture indices of the model."""

        return [polygon.texture_index for polygon in self.polygons]

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
            polygon = BgfPolygon.from_file(file)
            bgf_model.polygons.append(polygon)

        return bgf_model
