from dataclasses import dataclass
from pathlib import Path

import numpy as np
import pygltflib
import pygltflib.validator

from gilde_decoder.animations_decoder import BafFile
from gilde_decoder.const import GLTF_EXTENSION
from gilde_decoder.data.bgf.bgf_file import BgfFile
from gilde_decoder.helpers import (
    bitmap_to_gltf_uri,
    bytes_to_gltf_uri,
    find_texture_path,
)


@dataclass
class GltfFile:
    name: str
    gltf: pygltflib.GLTF2
    texture_names: list[str]

    def write(self, output_path: Path, search_path: Path) -> None:
        """Writes the object to the given output path."""

        if not output_path.exists():
            output_path.mkdir(parents=True)

        gltf_path = output_path / (self.name + GLTF_EXTENSION)

        self.gltf.save(gltf_path)
        # self._copy_textures(search_path, output_path)

    def get_texture_uri(self, texture_name: str, search_path: Path) -> None:
        """Returns the uri of the texture."""

        texture_path = search_path / texture_name
        return bytes_to_gltf_uri(texture_path.read_bytes())

    @classmethod
    def from_bgf_file(
        cls,
        bgf_file: BgfFile,
        search_path: Path,
        baf_file: BafFile | None = None,
    ) -> "GltfFile":
        gltf_object = cls.__new__(cls)

        gltf_object.name = bgf_file.path.stem
        gltf_object.gltf = pygltflib.GLTF2()

        gltf_mesh = bgf_file.get_gltf_mesh(baf_file)

        scene = pygltflib.Scene(nodes=[0])
        gltf_object.gltf.scenes.append(scene)

        primitives: list[pygltflib.Primitive] = []
        mesh = pygltflib.Mesh(
            primitives=primitives,
        )
        gltf_object.gltf.meshes.append(mesh)

        node = pygltflib.Node(
            mesh=0,
        )
        gltf_object.gltf.nodes.append(node)

        for i, gltf_primitive in enumerate(gltf_mesh.primitives):
            primitive = pygltflib.Primitive(
                attributes={
                    "POSITION": len(gltf_object.gltf.accessors) + 1,
                    "NORMAL": len(gltf_object.gltf.accessors) + 2,
                    "TEXCOORD_0": len(gltf_object.gltf.accessors) + 3,
                },
                indices=len(gltf_object.gltf.accessors),
                material=gltf_primitive.texture_index,
            )
            primitives.append(primitive)

            gltf_object.add_gltf_data(
                data=gltf_primitive.indices,
                buffer_type=pygltflib.ELEMENT_ARRAY_BUFFER,
                data_type=pygltflib.UNSIGNED_INT,
                data_format=pygltflib.SCALAR,
                name=f"indices_{i}",
                minmax=False,
            )

            gltf_object.add_gltf_data(
                data=gltf_primitive.vertices,
                buffer_type=pygltflib.ARRAY_BUFFER,
                data_type=pygltflib.FLOAT,
                data_format=pygltflib.VEC3,
                name=f"vertices_{i}",
            )

            gltf_object.add_gltf_data(
                data=gltf_primitive.normals,
                buffer_type=pygltflib.ARRAY_BUFFER,
                data_type=pygltflib.FLOAT,
                data_format=pygltflib.VEC3,
                name=f"vertex_normals_{i}",
            )

            gltf_object.add_gltf_data(
                data=gltf_primitive.uvs,
                buffer_type=pygltflib.ARRAY_BUFFER,
                data_type=pygltflib.FLOAT,
                data_format=pygltflib.VEC2,
                name=f"uv_coordinates_{i}",
            )

            if gltf_primitive.vertices_per_key is not None:
                for j, anim_vertices in enumerate(gltf_primitive.vertices_per_key):
                    relative_anim_vertices = anim_vertices - gltf_primitive.vertices
                    gltf_object.add_gltf_data(
                        data=relative_anim_vertices,
                        buffer_type=pygltflib.ARRAY_BUFFER,
                        data_type=pygltflib.FLOAT,
                        data_format=pygltflib.VEC3,
                        name=f"vertices_{i}_{j}",
                    )

                    primitive.targets.append(
                        pygltflib.Attributes(
                            POSITION=len(gltf_object.gltf.accessors) - 1,
                        )
                    )

        gltf_object.texture_names = bgf_file.bgf_footer.texture_names

        missing_texture_indices: list[int] = []

        for i, texture_name in enumerate(gltf_object.texture_names):
            texture_path = find_texture_path(texture_name, search_path)

            if texture_path is None:
                missing_texture_indices.append(i)
                continue

            texture_uri = bitmap_to_gltf_uri(texture_path)

            gltf_object.gltf.images.append(
                pygltflib.Image(
                    uri=texture_uri,
                )
            )
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
                        ),
                        metallicFactor=0.0,
                        roughnessFactor=1.0,
                    ),
                    doubleSided=True,
                    alphaMode=pygltflib.MASK,
                )
            )

        # for primitive in primitives:
        #     for i in missing_texture_indices:
        #         if i < primitive.material:
        #             primitive.material -= 1

        if baf_file is not None:
            keyframe_count = baf_file.keyframe_count_cut

            time_values = np.array(
                [float(keyframe) for keyframe in range(keyframe_count)],
                dtype=np.float32,
            )
            gltf_object.add_gltf_data(
                data=time_values,
                data_type=pygltflib.FLOAT,
                data_format=pygltflib.SCALAR,
                name="time_values",
            )
            time_values_id = len(gltf_object.gltf.accessors) - 1

            weight_values = []
            for i in range(keyframe_count):
                weight_values.append([0.0] * keyframe_count)
                weight_values[-1][i] = 1.0
            weight_values = np.array(weight_values).flatten()

            gltf_object.add_gltf_data(
                data=weight_values,
                data_type=pygltflib.FLOAT,
                data_format=pygltflib.SCALAR,
                name="weight_values",
                minmax=False,
            )
            weight_values_id = len(gltf_object.gltf.accessors) - 1

            animation = pygltflib.Animation(
                name="animation",
                samplers=[
                    pygltflib.AnimationSampler(
                        input=time_values_id,
                        interpolation=pygltflib.ANIM_LINEAR,
                        output=weight_values_id,
                    ),
                ],
                channels=[
                    pygltflib.AnimationChannel(
                        sampler=0,
                        target=pygltflib.AnimationChannelTarget(
                            node=0,
                            path=pygltflib.WEIGHTS,
                        ),
                    ),
                ],
            )
            gltf_object.gltf.animations.append(animation)

        return gltf_object

    def add_gltf_data(
        self,
        data: np.ndarray,
        data_type: int,
        data_format: str,
        name: str = "",
        minmax: bool = True,
        buffer_type: int | None = None,
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
            target=buffer_type,
            extras={
                "name": name,
            },
        )
        self.gltf.bufferViews.append(data_buffer_view)

        min: list[float] | None = None
        max: list[float] | None = None

        if minmax:
            if data.ndim == 1:
                min = [float(np.min(data))]
                max = [float(np.max(data))]
            else:
                min = [float(np.min(data[:, i])) for i in range(data.shape[1])]
                max = [float(np.max(data[:, i])) for i in range(data.shape[1])]

        data_accessor = pygltflib.Accessor(
            bufferView=len(self.gltf.bufferViews) - 1,
            byteOffset=0,
            componentType=data_type,
            count=len(data),
            type=data_format,
            min=min,
            max=max,
            extras={
                "name": name,
            },
        )
        self.gltf.accessors.append(data_accessor)

        return data_buffer, data_buffer_view, data_accessor
