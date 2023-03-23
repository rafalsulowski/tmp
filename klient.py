from multiprocessing.managers import BaseManager
from numpy import arange
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
fnameA = sys.argv[2] if len(sys.argv) > 2 else "A.dat"
fnameX = sys.argv[3] if len(sys.argv) > 3 else "X.dat"

#1.Wczytanie macierzy i wektora do pamieci
A = read(fnameA)
X = read(fnameX)

#2. Kalkulacja zakresow liczenia procesow
print(f"liczba procesow = {ncpus}\n\n")
print(f"len = {len(X[0:10])}")
nRows = len(X)
indexPerProcess = math.floor(nRows / ncpus)

#3.Zapis zakresow i danych do in_queue
queue.put(6)
queue.put([X[0:10], A[0:10]])
queue.put([X[10:20], A[0:10]])
queue.put([X[20:30], A[0:10]])
queue.put([X[30:40], A[0:10]])
queue.put([X[40:50], A[0:10]])
queue.put([X[50:60], A[0:10]])

#4.Oczekiwanie na wyniki
print("Oczekiwanie na wyniki...\n")
for x in range(1, 6):
    result = queueOut.get()
    print(f"wynik {x}: {result}\n\n")