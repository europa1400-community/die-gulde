"""Module containing the WavefrontObject class."""
import shutil
from dataclasses import dataclass
from pathlib import Path

from gilde_decoder.const import MTL_EXTENSION, OBJ_EXTENSION, WAVEFRONT_ENCODING
from gilde_decoder.data.bgf.bgf_file import BgfFile
from gilde_decoder.data.bgf.bgf_model import BgfModel
from gilde_decoder.data.wavefront.wavefront_model import WavefrontModel
from gilde_decoder.logger import logger


@dataclass
class WavefrontObject:
    """Class representing a wavefront object."""

    name: str
    wavefront_models: list[WavefrontModel]
    materials: list[str]
    textures: list[str]

    def write(self, output_path: Path, search_path: Path) -> None:
        """Writes the object to the given output path."""

        if not output_path.exists():
            output_path.mkdir(parents=True)

        obj_path = output_path / (self.name + OBJ_EXTENSION)
        mtl_path = output_path / (self.name + MTL_EXTENSION)

        self._write_obj(obj_path)
        self._write_mtl(mtl_path)
        self._copy_textures(search_path, output_path)

    def _write_obj(self, output_path: Path) -> None:
        """Writes the object file."""

        mtl_file_name = self.name + MTL_EXTENSION

        with open(output_path, "w", encoding=WAVEFRONT_ENCODING) as obj_file:
            obj_file.write(f"mtllib {mtl_file_name}\n")

            # Indexing in obj files is global an not per group/object.
            # But the indices in the bgf files are per model. This means we have
            # to shift the indices of later models by the number of points
            # (vertices, texture points) of the earlier models.
            face_offset = 0
            tex_offset = 0
            for wavefront_model_index, wavefront_model in enumerate(
                self.wavefront_models
            ):
                obj_file.write(f"o group{wavefront_model_index}\n")

                for vertex in wavefront_model.vertices:
                    # Rotate by -90 degrees around the x axis to correct model orientation.
                    # This is done by applying the following rotation matrix:
                    # R_x(-90) = | 1  0         0         |
                    #            | 0  cos(-90)  -sin(-90) |
                    #            | 0  sin(-90)  cos(-90)  |
                    # The cos(-90) and sin(-90) are whole numbers, so the
                    # calculation is simplified to
                    # v_x' = v_x
                    # v_y' = v_z
                    # v_z' = -v_y
                    obj_file.write(f"v {vertex.x} {vertex.z} {-vertex.y}\n")

                for normal in wavefront_model.normals:
                    obj_file.write(f"vn {normal.x} {normal.z} {-normal.y}\n")

                for texture_mapping in wavefront_model.texture_mappings:
                    obj_file.write(
                        f"vt {texture_mapping.a.u} {texture_mapping.a.v} 0\n"
                    )
                    obj_file.write(
                        f"vt {texture_mapping.b.u} {texture_mapping.b.v} 0\n"
                    )
                    obj_file.write(
                        f"vt {texture_mapping.c.u} {texture_mapping.c.v} 0\n"
                    )

                for face_index, (face, material) in enumerate(
                    zip(wavefront_model.faces, wavefront_model.materials)
                ):
                    obj_file.write(f"usemtl {material}\n")
                    obj_file.write(
                        f"f {face.a + 1 + face_offset}/{face_index * 3 + 1 + tex_offset}/{face_index + 1} "
                        + f"{face.b + 1 + face_offset}/{face_index * 3 + 2 + tex_offset}/{face_index + 1} "
                        + f"{face.c + 1 + face_offset}/{face_index * 3 + 3 + tex_offset}/{face_index + 1}\n"
                    )
                face_offset += len(wavefront_model.vertices)
                # Times 3 since each face has 3 texture mappings.
                tex_offset += len(wavefront_model.texture_mappings * 3)

    def _write_mtl(self, output_path: Path) -> None:
        """Writes the material file."""

        with open(output_path, "w", encoding=WAVEFRONT_ENCODING) as file:
            for material, texture in zip(self.materials, self.textures):
                file.write(f"newmtl {material}\n")
                file.write("Ka 1.0 1.0 1.0\n")
                file.write("Kd 1.0 1.0 1.0\n")
                file.write("Ks 0.0 0.0 0.0\n")
                file.write(f"map_Kd {texture}\n")

    def _copy_textures(self, search_path: Path, output_path: Path) -> None:
        """
        Copies the textures from the bgf file
        to the obj file directory for the 3d file viewer.
        @param bgf_textures: The decoded textures from the bgf file.
        @param texture_search_path: The path to search for the textures.
        Should contain the content of the textures.bin file.
        @param output_path: The path to copy the textures to.
        Should be the same as the obj file path
        since 3D viewers search for textures there.
        """

        def either(c):
            return "[%s%s]" % (c.lower(), c.upper()) if c.isalpha() else c

        case_insensitive_file_names = [
            "".join(map(either, texture)) for texture in self.textures
        ]
        texture_files_nested = [
            list(search_path.rglob(texname)) for texname in case_insensitive_file_names
        ]
        texture_files = [item for sublist in texture_files_nested for item in sublist]

        if len(texture_files) != len(self.textures):
            logger.warning(
                "Amount of texture files found differs "
                "from amount specified specified in bgf file."
            )

        for texture_file in texture_files:
            shutil.copy(texture_file, output_path)

    @classmethod
    def from_bgf_file(cls, bgf_file: BgfFile) -> "WavefrontObject":
        """Converts a bgf dict to a wavefront obj file.
        @bgf_file The bgf file to convert
        @texture_search_path Path where the textures will be searched.
        If given, textures will be copied next to the obj/mtl
        files so that 3D viewers can find them.
        """

        wavefront_object = cls.__new__(cls)
        wavefront_object.name = bgf_file.path.stem

        bgf_models: list[BgfModel] = [
            bgf_game_object.bgf_model
            for bgf_game_object in bgf_file.bgf_game_objects
            if bgf_game_object.bgf_model is not None
        ]

        if not bgf_models:
            raise ValueError("No models found in bgf file.")

        wavefront_models: list[WavefrontModel] = []
        for bgf_model in bgf_models:
            try:
                bgf_model_materials: list[str] = [
                    bgf_file.bgf_textures[texture_index].material_name
                    for texture_index in bgf_model.texture_indices
                ]

                wavefront_model = WavefrontModel.from_bgf_model(
                    bgf_model,
                    bgf_model_materials,
                )

                wavefront_models.append(wavefront_model)
            except (ValueError, IndexError) as e:
                logger.error(f"Error while converting {bgf_file.path}: {e}")

        wavefront_object.textures = [
            bgf_texture.name for bgf_texture in bgf_file.bgf_textures
        ]
        wavefront_object.materials = [
            bgf_texture.material_name for bgf_texture in bgf_file.bgf_textures
        ]
        wavefront_object.wavefront_models = wavefront_models

        return wavefront_object
