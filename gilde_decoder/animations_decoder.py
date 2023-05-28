import tkinter as tk
from dataclasses import dataclass
from pathlib import Path
from tkinter import filedialog

import construct as cs
import matplotlib.animation as animation
import matplotlib.pyplot as plt
from construct_typed import DataclassMixin, DataclassStruct, csfield

from gilde_decoder.const import (
    ANIMATIONS_BIN,
    BAF_DIR,
    BAF_EXCLUDE,
    BAF_EXTENSION,
    RESOURCES_DIR,
)
from gilde_decoder.data.animations_argument_parser import AnimationsArgumentsParser
from gilde_decoder.helpers import (
    HexConst,
    OptionalTaggedValue,
    extract_zipfile,
    get_files,
)
from gilde_decoder.logger import logger


@dataclass
class Vertex(DataclassMixin):
    x: float = csfield(cs.Float32l)
    y: float = csfield(cs.Float32l)
    z: float = csfield(cs.Float32l)


@dataclass
class AnimHeader(DataclassMixin):
    magic: bytes = csfield(cs.Const(b"BGF\0"))
    const_30: bytes = csfield(HexConst("30"))
    fsize_less_10: int = csfield(cs.Int32ul)
    const_01: bytes = csfield(HexConst("01"))
    a: int = csfield(cs.Int16ul)
    const_cdab23: bytes = csfield(HexConst("CDAB23"))
    num_keys: int = csfield(cs.Int32ul)
    const_33012400: bytes = csfield(HexConst("33012400"))
    const_37: bytes = csfield(HexConst("37"))
    b: int = csfield(cs.Int32ul)
    const_36: bytes = csfield(HexConst("36"))
    groups_per_key: int = csfield(cs.Int32ul)
    const_34: bytes = csfield(HexConst("34"))
    num_points: int = csfield(cs.Int32ul)
    d: int = csfield(OptionalTaggedValue("29"))
    e: int = csfield(OptionalTaggedValue("2A"))


@dataclass
class PointContainer(DataclassMixin):
    const_18: bytes = csfield(HexConst("18"))
    id: int = csfield(cs.Int32ul)
    const_19: bytes = csfield(HexConst("19"))
    count: int = csfield(cs.Int32ul)
    const_21: bytes = csfield(HexConst("21"))
    vertices: list[Vertex] = csfield(cs.Array(cs.this.count, DataclassStruct(Vertex)))
    const_28: bytes = csfield(HexConst("28"))


@dataclass
class Key(DataclassMixin):
    models: list[PointContainer] = csfield(
        cs.Array(cs.this._._.header.groups_per_key, DataclassStruct(PointContainer))
    )


@dataclass
class AnimBody(DataclassMixin):
    keys: list[Key] = csfield(cs.Array(cs.this._.header.num_keys, DataclassStruct(Key)))


@dataclass
class Footer(DataclassMixin):
    const_2f: bytes = csfield(HexConst("2F"))


@dataclass
class BafFile(DataclassMixin):
    header: AnimHeader = csfield(DataclassStruct(AnimHeader))
    body: AnimBody = csfield(DataclassStruct(AnimBody))
    footer: Footer = csfield(DataclassStruct(Footer))


def f(vertices: list[Vertex]) -> tuple[list[float], list[float], list[float]]:
    """
    Construct a list of x, y, z coordinates from a list of points (where every point has a x, y, z coordinate).
    :param points:
    :return:
    """
    x = []
    y = []
    z = []
    for pt in vertices:
        x.append(pt.x)
        y.append(pt.y)
        z.append(pt.z)
    return x, y, z


# def export_mp4(filename: str, parsed_data: BafFile):
#     fig = plt.figure()
#     ax = plt.axes(projection="3d")
#     # Rotate for side view
#     ax.view_init(90)

#     # Find global min/max for plotting the animation
#     models_vert_list = [
#         model.vertices for key in parsed_data.body.keys_ for model in key.models
#     ]
#     all_x_coords = [vert.x for model in models_vert_list for vert in model]
#     all_y_coords = [vert.y for model in models_vert_list for vert in model]
#     all_z_coords = [vert.z for model in models_vert_list for vert in model]

#     xmin = min(all_x_coords)
#     xmax = max(all_x_coords)
#     ymin = min(all_y_coords)
#     ymax = max(all_y_coords)
#     zmin = min(all_z_coords)
#     zmax = max(all_z_coords)

#     print(f"Frame count: {len(parsed_data.body.keys_)}")
#     anim = animation.FuncAnimation(
#         fig, export_mp4, frames=len(parsed_data.body.keys_), interval=300
#     )
#     anim.save(f"{filename}.mp4")
#     plt.show()
#     ax.cla()
#     for model in parsed_data.body.keys_[frame].models:
#         x, y, z = f(model.vertices)
#         ax.scatter3D(x, y, z, color="green")
#     ax.set_xlim(xmin, xmax)
#     ax.set_ylim(ymin, ymax)
#     ax.set_zlim(zmin, zmax)


class AnimationsDecoder:
    game_path: Path
    output_path: Path
    baf_dir: Path | None
    baf_file_path: Path | None

    def __init__(
        self, game_path: Path, output_path: Path, baf_file: Path | None = None
    ):
        if not game_path.exists():
            raise FileNotFoundError(f"Game path {game_path} does not exist.")

        self.game_path = game_path

        resources_dir = self.game_path / RESOURCES_DIR

        if not resources_dir.exists():
            raise FileNotFoundError(
                f"Input path {self.game_path} does not "
                "contain a valid resources directory."
            )

        if baf_file:
            self.baf_file_path = baf_file
            self.baf_dir = None
            return

        animations_bin_path = resources_dir / ANIMATIONS_BIN

        if not animations_bin_path.exists():
            raise FileNotFoundError("animations.bin file not found.")

        self.baf_dir = output_path / BAF_DIR
        self.baf_file_path = None

        if not self.baf_dir.exists():
            self.baf_dir.mkdir(parents=True)

        extract_zipfile(animations_bin_path, self.baf_dir)

    def decode(self) -> None:
        if self.baf_file_path:
            baf_file = DataclassStruct(BafFile).parse_file(self.baf_file_path)
            print(baf_file)

        elif self.baf_dir:
            baf_paths = get_files(
                self.baf_dir, extension=BAF_EXTENSION, exclude=BAF_EXCLUDE
            )

            for baf_path in baf_paths:
                baf_file = DataclassStruct(BafFile).parse_file(baf_path)
                print(baf_file)

        else:
            raise ValueError("No baf file or directory provided.")


def main() -> None:
    print("Starting...")
    parser = AnimationsArgumentsParser()
    args = parser.parse_args()

    if not args.game_path:
        root = tk.Tk()
        root.withdraw()

        args.game_path = Path(
            filedialog.askdirectory(
                title="Select the game directory",
            )
        )

        root.destroy()

    logger.info(f"Game path: {args.game_path}")
    logger.info(f"Output path: {args.output_path}")

    if args.file:
        logger.info(f"Decoding single baf file: {args.file}")

    animations_decoder = AnimationsDecoder(args.game_path, args.output_path, args.file)
    animations_decoder.decode()

    # export_mp4()


if __name__ == "__main__":
    main()
