import os
from PIL import Image


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

        print(f"Found {graphics_count} graphics")

        graphics_headers = []

        for i in range(graphics_count):
            graphics_header_data = file.read(84)
            graphics_name = graphics_header_data[:48].decode("ascii").strip("\x00")
            graphics_start_address = int.from_bytes(graphics_header_data[48:52], byteorder="little", signed=False)
            shapbank_size = int.from_bytes(graphics_header_data[56:60], byteorder="little", signed=False)
            graphics_width = int.from_bytes(graphics_header_data[80:82], byteorder="little", signed=False)
            graphics_height = int.from_bytes(graphics_header_data[82:84], byteorder="little", signed=False)

            print(f"Decoding {graphics_name} ({graphics_width}x{graphics_height}) at +{graphics_start_address}")

            graphics_headers.append({
                "name": graphics_name,
                "start_address": graphics_start_address,
                "size": shapbank_size,
                "width": graphics_width,
                "height": graphics_height
            })

        for header in graphics_headers:
            file.seek(header["start_address"])

            shapbank_data = file.read(header["size"])

            graphics_count = int.from_bytes(shapbank_data[42:44], byteorder="little", signed=False)
            max_width = int.from_bytes(shapbank_data[44:46], byteorder="little", signed=False)
            max_width = int.from_bytes(shapbank_data[46:48], byteorder="little", signed=False)
            shapbank_size = int.from_bytes(shapbank_data[48:52], byteorder="little", signed=False)

            graphics_size_offset = 69
            graphics_offsets = []
            for i in range(graphics_count):
                offset = graphics_size_offset + 4 * i
                graphics_offsets.append(
                    int.from_bytes(shapbank_data[offset:offset + 4], byteorder="little", signed=False))

            x = 0
            for i in graphics_offsets:
                file.seek(header["start_address"] + i)
                graphic_size = int.from_bytes(file.read(4), byteorder="little", signed=False)
                file.read(2)
                width = int.from_bytes(file.read(2), byteorder="little", signed=False)
                file.read(2)
                height = int.from_bytes(file.read(2), byteorder="little", signed=False)
                file.read(38)

                graphic_data = file.read(graphic_size - 62 - 60)

                img = Image.frombytes(mode="RGB", size=(width + 4, height), data=graphic_data)

                if x == 0:
                    file_path = os.path.join(output_graphics_path, header["name"] + ".bmp")
                else:
                    file_path = os.path.join(output_graphics_path, header["name"] + "+" + str(x) + ".bmp")

                img.save(file_path)
                x += 1


if __name__ == "__main__":
    decode_graphics("/Users/nikolaosginos/Desktop/OpenGuild.Tools.Game/gfx/Gilde_add_on_german.gfx", "output/")
