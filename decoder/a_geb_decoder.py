# # A_Geb.dat-Dekoder
# size=589 # Datei besteht aus Blöcken dieser Größe, kein Header
# array=1 # Und ist ein Array

# gebäudetype : u8
# *name : string[32]

# %org 0x243
# bauzeit : u32 # In Jahren
# gebäudelevel : u8
# preis : u32 # ist das wirklich der preis? Wenn ja, wie skaliert? (Baupreisniveau)


import json
import struct

# Open the binary file in read mode
with open('A_Geb.dat', 'rb') as file:

    data = file.read(589)

    buildings = []

    while data:
        building = {}

        building_type = struct.unpack('<B', data[:1])[0]
        building_name = data[1:33].decode('ascii').rstrip('\x00')
        building_time = struct.unpack('<I', data[-10:-6])[0]
        building_level = struct.unpack('<B', data[583:584])[0]
        building_price = struct.unpack('<I', data[584:588])[0]
        
        if building_type == 0:
            data = file.read(589)
            continue

        building['building_type'] = str(building_type)
        building['building_name'] = str(building_name)
        building['building_time'] = str(building_time)
        building['building_level'] = str(building_level)
        building['building_price'] = str(building_price)
        
        data = file.read(589)

        buildings.append(building)
with open('buildings.json', 'w', encoding='utf-8') as jsonfile:
    json.dump(buildings, jsonfile, ensure_ascii=False, indent=4)
