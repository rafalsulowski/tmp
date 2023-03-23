from multiprocessing.managers import BaseManager
import queue
import sys


class QueueManager(BaseManager):
    pass

port = 50000
password = "abc"
address = "127.0.0.1"

def main():
    in_queue = queue.Queue()
    out_queue = queue.Queue()
    QueueManager.register('in_queue', callable=lambda:in_queue)
    QueueManager.register('out_queue', callable=lambda:out_queue)
    manager = QueueManager(address=(address, port), authkey=password.encode('utf-8'))
    server = manager.get_server()
    server.serve_forever()


if __name__ == '__main__':
    main(*sys.argv[1:])
