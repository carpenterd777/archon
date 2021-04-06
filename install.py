#!/usr/bin/env python3

"""
Installs archon on a Linux Ubuntu environment.
Run this from the zip folder containing the other
related files.
"""

from os import chmod, getcwd
from os.path import join
from pathlib import Path
from shutil import copyfile
from stat import S_IRWXU

cwd = getcwd()
home = str(Path.home())
localbin = join(home, ".local/bin")

print("Copying executable into ~/.local/bin")
copyfile(join(cwd, "archon"), join(localbin, "archon"))
print("Copying pdb into ~/.local/bin")
copyfile(join(cwd, "Archon.pdb"), join(localbin, "Archon.pdb"))
print("Enabling archon to be ran")
chmod(join(localbin, "archon"), S_IRWXU)