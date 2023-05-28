import sys
from typing import List, Tuple

from construct import Struct, Byte, IfThenElse, Float32l, Int32ul, this, Const, GreedyBytes, Int8ul, Optional, Probe, \
    If, Peek, Pass, RepeatUntil, Array, Int16ul, CString, Debugger, Tell
import re

import matplotlib.pyplot as plt
from mpl_toolkits import mplot3d
import matplotlib.animation as animation


Vertex = Struct(
    "x" / Float32l,
    "y" / Float32l,
    "z" / Float32l
)


def HexConst(hex_str: str) -> Const:
    if len(hex_str) % 2 != 0:
        raise RuntimeError("Hex string must be of even size")
    return Const(bytes.fromhex(hex_str))


def OptionalTaggedValue(tag: str, value_type=Int32ul) -> Struct:
    return Struct(
        "has_value" / Optional(HexConst(tag)),
        "value" / If(this.has_value, value_type)
    )


AnimHeader = Struct(
    "magic" / Const(b"BGF\0"),
    HexConst("30"),
    "fsize_less_10" / Int32ul,
    HexConst("01"),
    "a" / Int16ul,
    HexConst("CDAB23"),
    "num_keys" / Int32ul,
    HexConst("33012400"),
    HexConst("37"),
    "b" / Int32ul,
    HexConst("36"),
    "groups_per_key" / Int32ul,
    HexConst("34"),
    "num_points" / Int32ul,
    "d" /  OptionalTaggedValue("29"),
    "e" / OptionalTaggedValue("2A"),
)


PointContainer = Struct(
    HexConst("18"),
    "id" / Int32ul,
    HexConst("19"),
    "count" / Int32ul,
    HexConst("21"),
    "vertices" / Array(this.count, Vertex),
    HexConst("28"),
)


Key = Struct(
    "models" / Array(this._._.header.groups_per_key, PointContainer),
)

AnimBody = Struct(
    # The name keys collides with a builtin name from somewhere
    "keys_" / Array(this._.header.num_keys, Key),
)

Footer = Struct(
    HexConst("2F")
)

Anim = "bgf" / Struct(
    "header" / AnimHeader,
    "body" / AnimBody,
    "footer" / Footer,
)

if len(sys.argv) != 2:
    print("Pass baf file as command line argument")
    sys.exit(1)
filename = sys.argv[1]
with open(filename, "rb") as file:
    # Read and parse the binary data
    parsed_data = Anim.parse_stream(file)
print(parsed_data)


def f(points: List["Point"]) -> Tuple[List[float], List[float], List[float]]:
    """
    Construct a list of x, y, z coordinates from a list of points (where every point has a x, y, z coordinate).
    :param points:
    :return:
    """
    x = []
    y = []
    z = []
    for pt in points:
        x.append(pt.x)
        y.append(pt.y)
        z.append(pt.z)
    return x, y, z


fig = plt.figure()
ax = plt.axes(projection='3d')
# Rotate for side view
ax.view_init(90)

# Find global min/max for plotting the animation
models_vert_list = [model.vertices for key in parsed_data.body.keys_ for model in key.models]
all_x_coords = [vert.x for model in models_vert_list for vert in model]
all_y_coords = [vert.y for model in models_vert_list for vert in model]
all_z_coords = [vert.z for model in models_vert_list for vert in model]

xmin = min(all_x_coords)
xmax = max(all_x_coords)
ymin = min(all_y_coords)
ymax = max(all_y_coords)
zmin = min(all_z_coords)
zmax = max(all_z_coords)


def anim_func(frame):
    ax.cla()
    for model in parsed_data.body.keys_[frame].models:
        x, y, z = f(model.vertices)
        ax.scatter3D(x, y, z, color="green")
    ax.set_xlim(xmin, xmax)
    ax.set_ylim(ymin, ymax)
    ax.set_zlim(zmin, zmax)


print(f"Frame count: {len(parsed_data.body.keys_)}")
anim = animation.FuncAnimation(fig, anim_func, frames=len(parsed_data.body.keys_), interval=300)
anim.save(f"{filename}.mp4")
plt.show()
