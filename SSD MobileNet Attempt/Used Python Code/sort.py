from pyutil import filereplace

with open('../GTSDBDataset/gt.txt', 'r') as f:
    filedata = f.read()

x = 0
y = 0

while (x < 1027):
    if (x < 10):
        string = "0000" + str(x) + ".ppm"
    elif (x > 9 and x < 100):
        string = "000" + str(x) + ".ppm"
    elif (x > 99 and x < 1000):
        string = "00" + str(x) + ".ppm"
    elif (x > 999):
        string = "0" + str(x) + ".ppm"

    if (y < 10):
        string2 = "0000" + str(y) + ".ppm"
    elif (y > 9 and y < 100):
        string2 = "000" + str(y) + ".ppm"
    elif (y > 99 and y < 1000):
        string2 = "00" + str(y) + ".ppm"

    if (filedata.find(string) != -1):
        filereplace("../GTSDBDataset/gt.txt", string, string2)
        y += 1

    x += 1

