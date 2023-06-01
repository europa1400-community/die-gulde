import shutil
from dataclasses import dataclass
from pathlib import Path

import numpy as np
import pygltflib
import pygltflib.validator

from gilde_decoder.const import GLTF_EXTENSION
from gilde_decoder.data.bgf.bgf_file import BgfFile
from gilde_decoder.helpers import bytes_to_gltf_uri
from gilde_decoder.logger import logger


@dataclass
class GltfFile:
    name: str
    gltf: pygltflib.GLTF2
    texture_names: list[Path]

    def write(self, output_path: Path, search_path: Path) -> None:
        """Writes the object to the given output path."""

        if not output_path.exists():
            output_path.mkdir(parents=True)

        gltf_path = output_path / (self.name + GLTF_EXTENSION)

        self.gltf.save(gltf_path)
        self._copy_textures(search_path, output_path)

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
            "".join(map(either, texture.name)) for texture in self.texture_names
        ]
        texture_files_nested = [
            list(search_path.rglob(texname)) for texname in case_insensitive_file_names
        ]
        texture_files = [item for sublist in texture_files_nested for item in sublist]

        if len(texture_files) != len(self.texture_names):
            logger.warning(
                "Amount of texture files found differs "
                "from amount specified specified in bgf file."
            )

        for texture_file in texture_files:
            # copy the texture file to the output path and rename it to lowercase
            shutil.copy2(
                texture_file,
                output_path / texture_file.name.lower(),
            )

    @classmethod
    def from_bgf_file(cls, bgf_file: BgfFile) -> "GltfFile":
        gltf_object = cls.__new__(cls)

        gltf_object.name = bgf_file.path.stem

        gltf_object.gltf = pygltflib.GLTF2()

        gltf_object.gltf.scenes.append(pygltflib.Scene(nodes=[0]))

        gltf_object.gltf.nodes.extend(
            [
                pygltflib.Node(
                    mesh=0,
                )
            ]
        )

        mesh = pygltflib.Mesh()
        gltf_object.gltf.meshes.append(mesh)

        for i, (
            vertices,
            vertex_normals,
            indices,
            uv_coordinates,
            material_index,
        ) in enumerate(bgf_file.bgf_mapping_object.get_primitive_datas):
            gltf_object.add_gltf_data(
                vertices,
                pygltflib.ARRAY_BUFFER,
                pygltflib.FLOAT,
                pygltflib.VEC3,
                name=f"vertices_mat_{material_index}",
            )
            gltf_object.add_gltf_data(
                vertex_normals,
                pygltflib.ARRAY_BUFFER,
                pygltflib.FLOAT,
                pygltflib.VEC3,
                name=f"vertex_normals_mat_{material_index}",
            )
            gltf_object.add_gltf_data(
                indices,
                pygltflib.ELEMENT_ARRAY_BUFFER,
                pygltflib.UNSIGNED_SHORT,
                pygltflib.SCALAR,
                name=f"indices_mat_{material_index}",
            )
            gltf_object.add_gltf_data(
                uv_coordinates,
                pygltflib.ARRAY_BUFFER,
                pygltflib.FLOAT,
                pygltflib.VEC2,
                name=f"uv_coordinates_mat_{material_index}",
            )

            primitive = pygltflib.Primitive(
                attributes={
                    "POSITION": i * 4,
                    "NORMAL": i * 4 + 1,
                    "TEXCOORD_0": i * 4 + 3,
                },
                indices=i * 4 + 2,
                material=material_index,
                extras={
                    "material_index": material_index,
                },
            )
            mesh.primitives.append(primitive)

        gltf_object.texture_names = [
            Path(bgf_texture.name) for bgf_texture in bgf_file.bgf_textures
        ]

        for texture_name in bgf_file.bgf_footer.texture_names:
            gltf_object.gltf.images.append(pygltflib.Image(uri=texture_name))
            gltf_object.gltf.textures.append(
                pygltflib.Texture(
                    source=len(gltf_object.gltf.images) - 1,
                )
            )
            gltf_object.gltf.materials.append(
                pygltflib.Material(
                    pbrMetallicRoughness=pygltflib.PbrMetallicRoughness(
                        baseColorTexture=pygltflib.TextureInfo(
                            index=len(gltf_object.gltf.textures) - 1,
                        )
                    )
                )
            )

        pygltflib.validator.validate(gltf_object.gltf)

        return gltf_object

    def add_gltf_data(
        self,
        data: np.ndarray,
        target: int,
        component_type: int,
        type: str,
        name: str = "",
    ) -> tuple[pygltflib.Buffer, pygltflib.BufferView, pygltflib.Accessor]:
        data_bytes = data.tobytes()

        data_buffer = pygltflib.Buffer(
            byteLength=len(data_bytes),
            uri=bytes_to_gltf_uri(data_bytes),
            extras={
                "name": name,
            },
        )
        self.gltf.buffers.append(data_buffer)

        data_buffer_view = pygltflib.BufferView(
            buffer=len(self.gltf.buffers) - 1,
            byteLength=len(data_bytes),
            byteOffset=0,
            target=target,
            extras={
                "name": name,
            },
        )
        self.gltf.bufferViews.append(data_buffer_view)

        min: list[float] | None = None
        max: list[float] | None = None

        if type != pygltflib.SCALAR:
            min = [float(np.min(data[:, i])) for i in range(data.shape[1])]
            max = [float(np.max(data[:, i])) for i in range(data.shape[1])]

        data_accessor = pygltflib.Accessor(
            bufferView=len(self.gltf.bufferViews) - 1,
            byteOffset=0,
            componentType=component_type,
            count=len(data),
            type=type,
            min=min,
            max=max,
            extras={
                "name": name,
            },
        )
        self.gltf.accessors.append(data_accessor)

        return data_buffer, data_buffer_view, data_accessor
