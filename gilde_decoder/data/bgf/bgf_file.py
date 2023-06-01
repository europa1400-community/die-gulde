"""Module containing the Bgf class."""

from dataclasses import dataclass
from pathlib import Path

from gilde_decoder.data.bgf.bgf_footer import BgfFooter
from gilde_decoder.data.bgf.bgf_game_object import BgfGameObject
from gilde_decoder.data.bgf.bgf_header import BgfHeader
from gilde_decoder.data.bgf.bgf_mapping_object import BgfMappingObject
from gilde_decoder.data.bgf.bgf_texture import BgfTexture
from gilde_decoder.data.gltf.gltf_mesh import GltfMesh
from gilde_decoder.logger import logger


@dataclass
class BgfFile:
    """Class representing a bgf file."""

    path: Path
    bgf_header: BgfHeader
    bgf_textures: list[BgfTexture]
    bgf_game_objects: list[BgfGameObject]
    bgf_mapping_object: BgfMappingObject
    bgf_footer: BgfFooter

    def get_gltf_meshes(self) -> list[GltfMesh]:
        """Returns the model data in gLTF compatible format."""

        return [
            bgf_game_object.get_gltf_mesh()
            for bgf_game_object in self.bgf_game_objects
            if bgf_game_object.bgf_model is not None
        ]

    @classmethod
    def from_file(cls, bgf_path: Path) -> "BgfFile":
        """Reads the bgf file."""

        logger.info(f"Decoding {bgf_path}")

        bgf = cls.__new__(cls)
        bgf.path = bgf_path

        if not bgf.path.exists():
            raise FileNotFoundError(f"{bgf.path} does not exist")

        with open(bgf.path, "rb") as file:
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

            bgf.bgf_footer = BgfFooter.from_file(
                file, bgf_path, bgf.bgf_textures, bgf.bgf_game_objects
            )

        return bgf
