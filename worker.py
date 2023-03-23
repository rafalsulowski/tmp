from multiprocessing import Process
from multiprocessing.managers import BaseManager
from numpy import arange
from threading import Thread

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


def create_Process(in_q, out_q, n_process):
    processes = [Process(target=mnoz, args=(in_q, out_q)) for _ in range(0, n_process)]
    for process in range(0, nmyProcess):
       processes[process].start()
    for process in range(0, nmyProcess):
      processes[process].join()


#Funkcja do mnozenia, zapis i odczut danych z kolejki
def mnoz(in_q, out_q):
    dane = in_q.get()
    
    # if dane is None:
    #     queueOut.task_done()

    # A = dane[0]
    # X = dane[1]

    # nrows = len(A)
    # ncols = len(A[0])
    # y = []
    # for i in arange(nrows):
    #     s = 0
    #     for c in range(0, ncols):
    #         s += A[i][c] * X[c][0]
    #     y.append(s)
    
    # queueOut.put(y)
    # queueOut.task_done()


#1.Odczyt ilosci procesow do utworzenia
nmyProcess = queue.get()
print(f"Liczba procesow {nmyProcess}, typ{type(nmyProcess)}\n\n")


#2.Uruchomienie procesow
t = Thread(target=create_Process, args=(queue, queueOut, nmyProcess))
t.start()
t.join()

