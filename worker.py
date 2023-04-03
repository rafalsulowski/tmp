from multiprocessing import Process
from multiprocessing.managers import BaseManager
from numpy import arange
from multiprocessing import Process, Lock, Array, cpu_count
from threading import Thread
import sys, math, time


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
lock = Lock()
processes = []

#1.Odczyt ilosci procesow do utworzenia
ncpus = queue.get()

#Funkcja do mnozenia, zapis i odczut danych z kolejki
def mnoz(workForProccess, matrix, vec, lock, solution):
    rowsNumber = len(matrix)
    columnsNumber = len(matrix[0])
    localValues = [0 * x for x in range(rowsNumber)]
    for i in range(workForProccess[0], workForProccess[1]):
        a = int(i / columnsNumber)
        b = int(i % columnsNumber)
        localValues[a] += matrix[a][b] * vec[b][0]
    lock.acquire()
    for i in range(len(localValues)):
        solution[i] += localValues[i]
    lock.release()


#funkcja do podzialu zakresu liczenia
def Divide(ncpus, dataNumber):
    divide = []
    step = int(dataNumber/ncpus)
    lastStep = dataNumber % ncpus
    for i in range(0, ncpus + 1):
        divide.append(i * step)
    if (lastStep != 0):
        divide[len(divide) - 1] += lastStep
    return divide


#odczyt danych do przetworzenia
print("Liczba procesow = ", ncpus)
while True:
    matrix = queue.get()
    print("Odczyt = ", matrix)
    if type(matrix) is tuple:
            solution = Array('d', range(len(matrix[1])))
            for i in range(len(matrix[1])):
                solution[i] = 0
            division = Divide(ncpus, len(matrix[1]) * len(matrix[1][0]))
            for num in range(ncpus):
                workForProccess = (division[num], division[num + 1])
                process = Process(target=mnoz, args=(workForProccess, matrix[1], matrix[2], lock, solution))
                processes.append(process)
                process.start()        
            for process in processes:
                process.join()
            queueOut.put((matrix[0], solution[:]))
    else:
        break