import struct

import pygltflib

gltf = pygltflib.GLTF2.load("output/gltf/Accessoires/Deko/Kaefig/Kaefig.gltf")

buffer = gltf.buffers[0]
data: bytes = gltf.get_data_from_buffer_uri(buffer.uri)

print(data)

vertex_num = 24
index_num = 30
normals_num = 10

# vertices where encoded like this:
# vertices = np.array(
#     [[vertex.x, vertex.z, -vertex.y] for vertex in bgf_model.vertices],
#     dtype=np.float32,
# )

# vertices_bytes = vertices.tobytes()

# decode vertices to float32
vertices_bytes = data[0 : vertex_num * 3 * 4]

vertices = struct.unpack("<" + "f" * (vertex_num * 3), vertices_bytes)

print(vertices)

# decode indices to uint16
indices_bytes = data[vertex_num * 3 * 4 : vertex_num * 3 * 4 + index_num * 2]

indices = struct.unpack("<" + "H" * index_num, indices_bytes)

print(indices)

# decode normals to float32
normals_bytes = data[
    vertex_num * 3 * 4
    + index_num * 2 : vertex_num * 3 * 4
    + index_num * 2
    + normals_num * 3 * 4
]

normals = struct.unpack("<" + "f" * (normals_num * 3), normals_bytes)

print(normals)

pass
