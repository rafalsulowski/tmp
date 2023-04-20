#include <stdlib.h>
#include <mpi.h>
#include <sys/time.h>
#include <stdio.h>

#define STEP 1000
#define LEFT 0
#define RIGHT 1
#define NUMBERPERPROCESS 1000

int main(int argc, char **argv)
{
    struct timeval time2;
    static struct timeval time1;
    gettimeofday(&time1, NULL);
    
    int source;
    int dest;
    int rank;
    int tag = 1;
    int numerOfTask;
    int m_Data[1000000];
    int inputBuffer[1000];
    int outputBuffer[1000];
    int onRS[2];
    int dimension[1];
    int periods[1] = {0}; 
    int reorder = 0;
    int coords[2];

    MPI_Request reqs[2];
    MPI_Status stats[2];
    MPI_Comm cartcomm;

    MPI_Init(&argc, &argv);
    MPI_Comm_size(MPI_COMM_WORLD, &numerOfTask);

    dimension[0] = numerOfTask;

    MPI_Cart_create(MPI_COMM_WORLD, 1, dimension, periods, reorder, &cartcomm);
    MPI_Comm_rank(cartcomm, &rank);
    MPI_Cart_coords(cartcomm, rank, 1, coords);
    MPI_Cart_shift(cartcomm, 0, 1, &onRS[LEFT], &onRS[RIGHT]);

    source = onRS[LEFT];
    dest = onRS[RIGHT];

    if (rank == 0)
    {
        for (int i = 0; i < NUMBERPERPROCESS * STEP; i++)
        {
            outputBuffer[i] = i;
        }
        for (int i = 0; i < NUMBERPERPROCESS; i++)
        {
            MPI_Isend(&outputBuffer[i * STEP], STEP, MPI_INT, dest, tag, MPI_COMM_WORLD, &reqs[1]);
            MPI_Wait(&reqs[1], &stats[1]);
        }
    }
    else
    {

        for (int i = 0; i < NUMBERPERPROCESS; i++)
        {
            MPI_Irecv(inputBuffer, STEP, MPI_INT, source, tag, MPI_COMM_WORLD, &reqs[0]);
            MPI_Wait(&reqs[0], &stats[0]);

            for (int j = 0; j < STEP; j++)
            {
                m_Data[i * STEP + j] = inputBuffer[j];
            }

            MPI_Isend(inputBuffer, STEP, MPI_INT, dest, tag, MPI_COMM_WORLD, &reqs[1]);
            MPI_Wait(&reqs[1], &stats[1]);
        }
    }

    if (rank == numerOfTask - 1)
    {
        long sum = 0;
        for (int i = 0; i < STEP * NUMBERPERPROCESS; i++)
        {
            sum += m_Data[i];
        }

        gettimeofday(&time2, NULL);
        int time = 1000 * (time2.tv_sec - time1.tv_sec) + (time2.tv_usec - time1.tv_usec) / 1000;
        printf("Suma elementow w procesie %d rowna sie %ld \n", rank, sum);
        printf("Dla liczby procesow %d czas programu to %d ms \n", numerOfTask, time);
    }

    MPI_Finalize();
}
