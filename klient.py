from multiprocessing.managers import BaseManager
from numpy import arange
import time
import sys, math


class QueueManager(BaseManager): 
    pass

port = 50000
password = "abc"
address = "127.0.0.1"

QueueManager.register('in_queue')
QueueManager.register('out_queue')
m = QueueManager(address=(address, port), authkey=password.encode('utf-8'))
m.connect()
queue = m.in_queue()
queueOut = m.out_queue()


def read(fname):
	f = open(fname, "r")
	nr = int(f.readline()) #rows
	nc = int(f.readline()) #cols

	A = [[0] * nc for x in arange(nr)]
	r = 0
	c = 0
	for i in range(0,nr*nc):
		A[r][c] = float(f.readline())
		c += 1
		if c == nc:
			c = 0
			r += 1

	return A


ncpus = int(sys.argv[1]) if len(sys.argv) > 1 else 2
queue.put(ncpus) #przeslanie inforamcaji ile procesow uruchomic
fnameA = sys.argv[2] if len(sys.argv) > 2 else "A.dat"
fnameX = sys.argv[3] if len(sys.argv) > 3 else "X.dat"


#1.Wczytanie macierzy i wektora do pamieci
A = read(fnameA)
X = read(fnameX)


start = time.time()
#2. Kalkulacja zakresow liczenia procesow
print(f"liczba procesow = {ncpus}\n\n")
nRows = len(A)
step = math.floor(nRows / ncpus)
lastStep = nRows & ncpus


#podzial po wierszach 
tab = []
for i in range(0, ncpus + 1):
	tab.append(i * step)
	if (lastStep != 0):
		tab[len(tab) - 1] += lastStep


#wyspanie podzialu do koljeki
for i in range (0, ncpus):
    queue.put((i, A[tab[i]:tab[i+1]], X))


#oczekiwanie na wyniki
tmpResult = [0 * x for x in range(ncpus)]
i = 0
time.sleep(2)
while i != ncpus:
        result = queueOut.get()
        if type(result) is tuple:
           tmpResult[result[0]] = result[1]
           i += 1 

#zebranie roziwazan z poszczegolnych wynikow czastkowych
solution = []
for result in tmpResult:
	solution.extend(result)

end = time.time()

#wypisanie wyniku
print("Result = ", solution)
print(f"Czas wykonania dla {ncpus} wynosi: {end - start - 1}\n\n")