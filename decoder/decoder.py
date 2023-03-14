import os
import tkinter as tk
from tkinter import filedialog
import argparse
from buildings_decoder import decode_buildings
from sounds_decoder import decode_sounds
from objects_decoder import decode_objects


def main():
    parser = argparse.ArgumentParser(description='gilde-decoder')
    parser.add_argument('-i', '--input', help='input path')
    parser.add_argument('-o', '--output', help='output path')

    args = parser.parse_args()

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

    sfx_path = os.path.join(args.input, 'sfx')
    data_path = os.path.join(args.input, 'Data')
    buildings_path = os.path.join(data_path, 'A_Geb.dat')
    objects_path = os.path.join(data_path, 'A_Obj.dat')

    sfx_path = os.path.join(args.input, 'sfx')
    gfx_path = os.path.join(args.input, 'gfx')
    gilde_gfx_path = os.path.join(gfx_path, 'Gilde_add_on_german.gfx')

    if os.path.exists(sfx_path):
        print("Decoding sounds...")
        decode_sounds(sfx_path, args.output)
    else:
        print("No sfx folder found")

    if os.path.exists(buildings_path):
        print("Decoding buildings...")
        decode_buildings(buildings_path, args.output)
    else:
        print("No buildings file found")
    
    if os.path.exists(objects_path):
        print("Decoding objects...")
        decode_objects(objects_path, args.output)
    else:
        print("No objects file found")


if __name__ == '__main__':
    main()
