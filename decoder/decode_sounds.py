
import sys
from sbf_decoder import decode_sbf

# get folder name from command line

folder = sys.argv[1]

# get all relative file paths in the folder recursively

import os
files = [os.path.join(dp, f) for dp, dn, filenames in os.walk(folder) for f in filenames if os.path.splitext(f)[1] == '.sbf']

# decode all sbf files in the folder

for file in files:
    try:
        decode_sbf(file)
    except:
        print(f'failed to decode {file}')

