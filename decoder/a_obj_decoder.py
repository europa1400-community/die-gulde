# # A_Obj.dat-Dekoder
# size=65
# array=1

# type : u8
# *name : string[32]
# level : u8 # "freischalt-level" ?
# bauzeit : u32 # In Minuten
# prop_idx : u16[4] # wenn != 0, ist in prop_val ein wert gesetzt, ist im Bereich ~1-5
# prop_val : u16[4] # werte bei ~500±200
# thingy : u16   # ändert sich bei brettern, sonst 1
# preis : u16    # für erweiterungen ist das hier preis * 2, ab und zu auch mal Preis * 3 / 4
# %skip 2 # always zero
# value : u16 # 3 bei Währungen/Münzen, Rohstoffvorkommen, Potential, 1 bei "Benutzbarem", 2 bei Essbarem, nicht direkt nachvollziehbar
# %skip 2 # always zero
# %skip 21 # always zero
# # end of struct


import json
import struct

# Open the binary file in read mode
with open('A_Obj.dat', 'rb') as file:

    data = file.read(65)

    objects = []

    while data:
        object = {}

        object['object_type'] = struct.unpack('<B', data[:1])[0]
        object['object_name'] = data[1:33].decode('ascii').rstrip('\x00')
        object['object_level'] = struct.unpack('<B', data[33:34])[0]
        object['object_time'] = struct.unpack('<I', data[34:38])[0]
        object['object_prop_idx'] = struct.unpack('<HHHH', data[38:46])
        object['object_prop_val'] = struct.unpack('<HHHH', data[46:54])
        object['object_thingy'] = struct.unpack('<H', data[54:56])[0]
        object['object_price'] = struct.unpack('<H', data[56:58])[0]
        object['object_value'] = struct.unpack('<H', data[60:62])[0]
        
        if object['object_name'] == "":
            data = file.read(65)
            continue
        
        data = file.read(65)

        objects.append(object)

with open('objects.json', 'w', encoding='utf-8') as jsonfile:
    json.dump(objects, jsonfile, ensure_ascii=False, indent=4)
