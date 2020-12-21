import os
import sys

f = open("random.txt", "r")
layout = f.read()
f.close()

layout = layout.split("\n")
o = []

for line in layout:
	o.append(line.split(" "))

for line in o:
	print(line)

towers = []

for line in o:
	x = [i for i, j in enumerate(line) if j == "."]
	towers.append(x)

print(towers)

time = 0

for row in range(0, len(towers)):
	for tower in towers[row]:
		for i in range(-3, 4):
			for j in range(-3, 4):

				if i == 0 and j == 0:
					continue

				if (row + i) < 0 or (row + i) > 8 or (tower + j) < 0 or (tower + j) > 8:
					continue

				if o[row+i][tower+j] == "O":
					time += 1

print(time)