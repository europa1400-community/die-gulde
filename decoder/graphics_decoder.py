import os
import tkinter as tk
from tkinter import filedialog
import argparse
from PIL import Image


def main():
    parser = argparse.ArgumentParser(description='gilde-decoder')
    parser.add_argument('-i', '--input', help='input path')
    parser.add_argument('-o', '--output', help='output path')
    parser.add_argument('-r', '--read', help='read path')

    args = parser.parse_args()

    if args.read:
        read_graphic(args.read)
        return

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

    gfx_path = os.path.join(args.input, "gfx")
    gfx_file_path = os.path.join(gfx_path, "Gilde_add_on_german.gfx")

    decode_graphics(gfx_file_path, args.output)


def decode_graphics(input_path: str, output_path: str):
    if input_path is None or output_path is None:
        raise ValueError("Input and output paths must be specified")

    if not os.path.exists(input_path):
        raise FileNotFoundError("Input file not found")

    output_graphics_path = os.path.join(output_path, "graphics")

    if not os.path.exists(output_graphics_path):
        os.mkdir(output_graphics_path)

    with open(input_path, "rb") as file:
        header_data = file.read(4)

        graphics_count = int.from_bytes(header_data, byteorder="little", signed=False)

        graphics_headers = []

        for i in range(graphics_count):
            graphics_header_data = file.read(84)
            graphics_name = graphics_header_data[:48].decode("ascii").strip("\x00")
            graphics_start_address = int.from_bytes(graphics_header_data[48:52], byteorder="little", signed=False)
            shapbank_size = int.from_bytes(graphics_header_data[56:60], byteorder="little", signed=False)
            graphics_width = int.from_bytes(graphics_header_data[80:82], byteorder="little", signed=False)
            graphics_height = int.from_bytes(graphics_header_data[82:84], byteorder="little", signed=False)

            graphics_headers.append({
                "name": graphics_name,
                "start_address": graphics_start_address,
                "size": shapbank_size,
                "width": graphics_width,
                "height": graphics_height
            })

        for graphic_header in graphics_headers:

            try:
                file.seek(graphic_header["start_address"])

                shapbank_data = file.read(graphic_header["size"])

                graphics_count = int.from_bytes(shapbank_data[42:44], byteorder="little", signed=False)

                # What is this information used for?
                max_width = int.from_bytes(shapbank_data[44:46], byteorder="little", signed=False)
                max_height = int.from_bytes(shapbank_data[46:48], byteorder="little", signed=False)

                # Shapbank size is stored twice? In the file header and in the Shapbank header
                shapbank_size = int.from_bytes(shapbank_data[48:52], byteorder="little", signed=False)

                graphics_size_offset = 69
                graphics_offsets = []

                for i in range(graphics_count):
                    offset = graphics_size_offset + 4 * i
                    graphics_offsets.append(
                        int.from_bytes(shapbank_data[offset:offset + 4], byteorder="little", signed=False))

                x = 0
                for i in graphics_offsets:
                    graphic_data = bytearray()

                    file.seek(graphic_header["start_address"] + i)
                    graphic_size = int.from_bytes(file.read(4), byteorder="little", signed=False)
                    num1 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    graphic_width = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    num2 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    graphic_height = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    num3 = int.from_bytes(file.read(2), byteorder="little", signed=False) # Always 0x0002
                    num4 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    num5 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    graphic_width_2 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    graphic_height_2 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    num6 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    num7 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    num8 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    num9 = int.from_bytes(file.read(2), byteorder="little", signed=False)
                    file.seek(8, os.SEEK_CUR)
                    num10 = int.from_bytes(file.read(4), byteorder="little", signed=False)
                    graphic_size_without_footer = int.from_bytes(file.read(4), byteorder="little", signed=False)
                    num12 = int.from_bytes(file.read(4), byteorder="little", signed=False)

                    for _ in range(graphic_height):
                        block_count = int.from_bytes(file.read(4), byteorder="little", signed=False)

                        if block_count == 0:
                            continue

                        for _ in range(block_count):
                            transparency_byte_count = int.from_bytes(file.read(4), byteorder="little", signed=False)
                            actual_pixel_count = int.from_bytes(file.read(4), byteorder="little", signed=False)

                            block_data = file.read(actual_pixel_count * 3)

                            if transparency_byte_count > 0:
                                graphic_data += bytearray(transparency_byte_count)

                            graphic_data += block_data

                    img = Image.frombytes(mode="RGB", size=(graphic_width, graphic_height), data=bytes(graphic_data))

                    if x == 0:
                        file_path = os.path.join(output_graphics_path, graphic_header["name"] + ".bmp")
                    else:
                        file_path = os.path.join(output_graphics_path, graphic_header["name"] + "+" + str(x) + ".bmp")

                    img.save(file_path)
                    x += 1
            except:
                print("Failed to extract graphic " + graphic_header["name"])


def read_graphic(path: str):
    if path is None:
        raise ValueError("Input path must be specified")
    
    with open(path, "r") as file:
        width = len(file.readline().split(" ")) / 3
        file.seek(0)

        height = len(file.readlines())
        file.seek(0)

        image_str = file.read()

    image_str = " ".join(image_str.split())
    image_str = image_str.strip()

    image_bytes = bytearray.fromhex(image_str)
    img = Image.frombytes(mode="RGB", size=(int(width), int(height)), data=bytes(image_bytes))
    img_path = os.path.splitext(path)[0] + ".bmp"

    img.save(img_path)


if __name__ == "__main__":
    main()
